using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Numerics;
using LiveSplit.ComponentUtil;
using System.ComponentModel;

namespace LiveSplit.Dishonored
{
    class GameData : MemoryWatcherList
    {
        public MemoryWatcher<Vector3> PlayerPos { get; }
        public MemoryWatcher<bool> IsLoading { get; }
        public MemoryWatcher<int> CurrentLevel { get; }
        public StringWatcher CurrentBikMovie { get; }
        public MemoryWatcher<bool> CutsceneActive { get; }
        public MemoryWatcher<int> MissionStatsScreenFlags { get; }

        public FakeMemoryWatcher<bool> MissionStatsScreenActive => new FakeMemoryWatcher<bool>(
            (MissionStatsScreenFlags.Old & 1) != 0,
            (MissionStatsScreenFlags.Current & 1) != 0);

        public int StringTableBase { get; }

        public GameData(GameVersion version)
        {
            if (version == GameVersion.v12)
            {
                PlayerPos = new MemoryWatcher<Vector3>(new DeepPointer(0xFCCBDC, 0xC4));
                CurrentLevel = new MemoryWatcher<int>(new DeepPointer(0xFB7838, 0x2C0, 0x314, 0, 0x38));
                CurrentBikMovie = new StringWatcher(new DeepPointer(0xFC6AD4, 0x48, 0), 64);
                CutsceneActive = new MemoryWatcher<bool>(new DeepPointer(0xFB51CC, 0x744));
                MissionStatsScreenFlags = new MemoryWatcher<int>(new DeepPointer(0xFDEB08, 0x24, 0x41C, 0x2E0, 0xC4));
                StringTableBase = 0xFA3624;
                IsLoading = new MemoryWatcher<bool>(new DeepPointer("binkw32.dll", 0x312F4));
            }
            else if (version == GameVersion.v14)
            {
                PlayerPos = new MemoryWatcher<Vector3>(new DeepPointer(0x1052DE8, 0xC4));
                CurrentLevel = new MemoryWatcher<int>(new DeepPointer(0x103D878, 0x2C0, 0x314, 0, 0x38));
                CurrentBikMovie = new StringWatcher(new DeepPointer(0x104CB18, 0x48, 0), 64);
                CutsceneActive = new MemoryWatcher<bool>(new DeepPointer(0x103B20C, 0x744));
                MissionStatsScreenFlags = new MemoryWatcher<int>(new DeepPointer(0x1065184, 0x24, 0x41C, 0x2F4, 0xC4));
                StringTableBase = 0x1029664;
                IsLoading = new MemoryWatcher<bool>(new DeepPointer("binkw32.dll", 0x312F4));
            }
            else if (version == GameVersion.v15)
            {
                PlayerPos = new MemoryWatcher<Vector3>(new DeepPointer(0x105F628, 0xC4));
                CurrentLevel = new MemoryWatcher<int>(new DeepPointer(0x1049888, 0x2C0, 0x314, 0, 0x38));
                CurrentBikMovie = new StringWatcher(new DeepPointer(0x1058B28, 0x48, 0), 64);  // or 0x105BE80
                CutsceneActive = new MemoryWatcher<bool>(new DeepPointer(0x104721C, 0x744));
                MissionStatsScreenFlags = new MemoryWatcher<int>(new DeepPointer(0x10719C4, 0x24, 0x41C, 0x2F4, 0xC4)); // or 0x1071CE4
                StringTableBase = 0x1035674; // diff by 0xC010
                IsLoading = new MemoryWatcher<bool>(new DeepPointer("binkw32.dll", 0x312F4));
            }
            else if (version == GameVersion.EGS)
            {
                PlayerPos = new MemoryWatcher<Vector3>(new DeepPointer(0x1815310, 0xB0));
                CurrentLevel = new MemoryWatcher<int>(new DeepPointer(0x1815310, 0x1A0, 0x5B8));
                CurrentBikMovie = new StringWatcher(new DeepPointer(0x1810348, 0x48, 0), 64);
                CutsceneActive = new MemoryWatcher<bool>(new DeepPointer(0x1802D88, 0x9EC));
                MissionStatsScreenFlags = new MemoryWatcher<int>(new DeepPointer(0x18292F8, 0x3C, 0x550, 0x420, 0x110));
                StringTableBase = 0x17F4270;
                IsLoading = new MemoryWatcher<bool>(new DeepPointer("binkw64.dll", 0x31494));
            }

            CurrentLevel.FailAction = MemoryWatcher.ReadFailAction.SetZeroOrNull;

            AddRange(GetType().GetProperties()
                .Where(p => !p.GetIndexParameters().Any())
                .Select(p => p.GetValue(this, null) as MemoryWatcher)
                .Where(p => p != null));
        }
    }

    class GameMemory : IDisposable
    {
        public enum AreaCompletionType
        {
            None,
            IntroEnd,
            MissionEnd,
            PrisonEscape,
            OutsidersDream,
            Weepers,
            DLC06IntroEnd,
        }

        public enum Level
        {
            None,
            MainMenu,
            Intro,
            Prison,
            Sewers,
            PubMorning,
            PubDay,
            PubDusk,
            PubNight,
            OutersiderDream,
            CampbellStreets,
            CampbellOverseer,
            CampbellBack,
            CatStreets,
            CatBrothel,
            Bridge1,
            Bridge2,
            Bridge3,
            Bridge4,
            BoyleExterior,
            BoyleInterior,
            TowerReturnYard,
            TowerReturnInterior,
            FloodedIntro,
            FloodedStreets,
            FloodedRefinery,
            FloodedAssassins,
            FloodedGate,
            FloodedSewers,
            Loyalists,
            KingsparrowIsland,
            KingsparrowLighthouse,
            Outro,
        }

        public enum Movie
        {
            None,
            Intro,
            DishonoredLogo,
            Loading,
            LoadingSaving,
            LoadingIntro,
            LoadingPrison,
            LoadingSewers,
            LoadingPub,
            LoadingVoid,
            LoadingStreets,
            LoadingBridge,
            LoadingBridge2,
            LoadingBoyle,
            LoadingTowerReturn,
            LoadingTowerReturn2,
            LoadingFlooded,
            LoadingAssassinsHQ,
            LoadingStreetsSewers,
            LoadingKingsparrow,
            LoadingOutro,
            Credits,
        }

        public event EventHandler OnFirstLevelLoading;
        public event EventHandler OnPlayerGainedControl;
        public event EventHandler OnLoadStarted;
        public delegate void LoadFinishedHandler(object sender, Level level, Level previousLevel, Movie movie, Vector3 pos);
        public event LoadFinishedHandler OnLoadFinished;
        public event EventHandler OnPlayerLostControl;
        public delegate void AreaCompletedEventHandler(object sender, AreaCompletionType type);
        public event AreaCompletedEventHandler OnAreaCompleted;
        public delegate void PostMoviePlayerPositionChangedHandler(object sender, Movie movie, Vector3 pos);
        public event PostMoviePlayerPositionChangedHandler OnPostMoviePlayerPositionChanged;
        public delegate void PostCutscenePlayerPositionChangedHandler(object sender, Level level, Vector3 pos);
        public event PostCutscenePlayerPositionChangedHandler OnPostCutscenePlayerPositionChanged;
        public delegate void PostLoadPlayerPositionChangedHandler(object sender, Level level, Level previousLevel, Vector3 pos);
        public event PostLoadPlayerPositionChangedHandler OnPostLoadPlayerPositionChanged;

        private readonly List<int> _ignorePIDs;

        private GameData _data;
        private Process _process;
        private bool _loadingStarted;
        private bool _oncePerLevelFlag;
        private Level _previousLevel;

        private bool _worldSpeedInjected;
        private int _scanAttempts;
        private IntPtr _worldSpeedPtr;
        private IntPtr _setWorldSpeedPtr;
        private IntPtr _injectedFuncPtr;
        private IntPtr _copyWorldSpeedPtr;
        private int _overwriteBytes;

        private readonly Timer _movieTimer;
        private readonly Timer _cutsceneTimer;
        private readonly Timer _loadTimer;

        private enum ExpectedDllSizes
        {
            DishonoredExe12 = 18219008,
            DishonoredExe14Reloaded = 18862080,
            DishonoredExe14Steam = 19427328,
            DishonoredExe15 = 18903040,
            DishonoredExeEGS = 27553792,
            BinkW32Dll = 241664,
            BinkW64Dll = 364544,
        }

        private readonly Dictionary<string, AreaCompletionType> _areaCompletions = new Dictionary<string, AreaCompletionType>
        {
            ["LoadingSewers|L_Prison_"]            = AreaCompletionType.PrisonEscape,
            ["LoadingStreets|L_Pub_Dusk_"]         = AreaCompletionType.OutsidersDream,
            ["LoadingStreets|L_Pub_Day_"]          = AreaCompletionType.Weepers,
            ["LoadingDLC06Slaughter|DLC06_Tower_"] = AreaCompletionType.DLC06IntroEnd,
        };

        private readonly Dictionary<string, Level> _levels = new Dictionary<string, Level>
        {
            ["Dishonored_MainMenu"] = Level.MainMenu,
            ["l_tower_"] = Level.Intro,
            ["L_Prison_"] = Level.Prison,
            ["L_PrsnSewer_"] = Level.Sewers,
            ["L_Pub_Morning_"] = Level.PubMorning,
            ["L_Pub_Day_"] = Level.PubDay,
            ["L_Pub_Dusk_"] = Level.PubDusk,
            ["L_Pub_Night_"] = Level.PubNight,
            ["L_OutsiderDream_"] = Level.OutersiderDream,
            ["L_Streets1_"] = Level.CampbellStreets,
            ["L_Ovrsr_Back_"] = Level.CampbellBack,
            ["L_Ovrsr_"] = Level.CampbellOverseer,
            ["L_Streets2_"] = Level.CatStreets,
            ["L_Brothel_"] = Level.CatBrothel,
            ["L_Bridge_Part1a_"] = Level.Bridge1,
            ["L_Bridge_Part1b_"] = Level.Bridge2,
            ["L_Bridge_Part1c_"] = Level.Bridge3,
            ["L_Bridge_Part2_"] = Level.Bridge4,
            ["L_Boyle_Ext_"] = Level.BoyleExterior,
            ["L_Boyle_Int_"] = Level.BoyleInterior,
            ["L_TowerRtrn_Yard_"] = Level.TowerReturnYard,
            ["L_TowerRtrn_Int_"] = Level.TowerReturnInterior,
            ["L_Flooded_FIntro_"] = Level.FloodedIntro,
            ["L_Flooded_FStreets_"] = Level.FloodedStreets,
            ["L_Flooded_FRefinery_"] = Level.FloodedRefinery,
            ["L_Flooded_FAssassins_"] = Level.FloodedAssassins,
            ["L_Flooded_FGate_"] = Level.FloodedGate,
            ["L_Streetsewer_"] = Level.FloodedSewers,
            ["L_Pub_Assault_"] = Level.Loyalists,
            ["L_Isl_"] = Level.KingsparrowIsland,
            ["L_LightH_"] = Level.KingsparrowLighthouse,
            ["L_Out_"] = Level.Outro,
        };

        private readonly Dictionary<string, Movie> _movies = new Dictionary<string, Movie>
        {
            ["INTRO_LOC"] = Movie.Intro,
            ["Dishonored"] = Movie.DishonoredLogo,
            ["Loading"] = Movie.Loading,
            ["LoadingSavingNotification"] = Movie.LoadingSaving,
            ["LoadingEmpressTower"] = Movie.LoadingIntro,
            ["LoadingPrison"] = Movie.LoadingPrison,
            ["LoadingSewers"] = Movie.LoadingSewers,
            ["LoadingHUB"] = Movie.LoadingPub,
            ["LoadingVoid"] = Movie.LoadingVoid,
            ["LoadingStreets"] = Movie.LoadingStreets,
            ["LoadingBridge"] = Movie.LoadingBridge,
            ["LoadingBridge2"] = Movie.LoadingBridge2,
            ["LoadingBoyle"] = Movie.LoadingBoyle,
            ["LoadingTowerReturn"] = Movie.LoadingTowerReturn,
            ["LoadingTowerReturn2"] = Movie.LoadingTowerReturn2,
            ["LoadingFlooded"] = Movie.LoadingFlooded,
            ["LoadingAssassinsHQ"] = Movie.LoadingAssassinsHQ,
            ["LoadingStreetsSewers"] = Movie.LoadingStreetsSewers,
            ["LoadingLighthouse"] = Movie.LoadingKingsparrow,
            ["LoadingOutro"] = Movie.LoadingOutro,
            ["Credits"] = Movie.Credits,
        };

        public GameMemory()
        {
            _ignorePIDs = new List<int>();
            _movieTimer = new Timer { Enabled = false };
            _movieTimer.Tick += Timer_OnTick;
            _cutsceneTimer = new Timer { Enabled = false };
            _cutsceneTimer.Tick += Timer_OnTick;
            _loadTimer = new Timer { Enabled = false };
            _loadTimer.Tick += Timer_OnTick;
        }

        public void Update()
        {
            if (_process == null || _process.HasExited)
            {
                if (!TryGetGameProcess())
                    return;
            }
            
            if (!_worldSpeedInjected)
                TryInjection();

            TimedTraceListener.Instance.UpdateCount++;

            _data.UpdateAll(_process);

            var currentMovie = _data.CurrentBikMovie.Current ?? "";
            var currentLevelStr = GetEngineStringByID(_data.CurrentLevel.Current) ?? "";
            var combinedStr = currentMovie + "|" + currentLevelStr;
            var level = _levels.Where(l => currentLevelStr.ToLower().StartsWith(l.Key.ToLower())).Select(l => l.Value).FirstOrDefault();
            _movies.TryGetValue(currentMovie, out var movie);
            var pos = _data.PlayerPos.Current;

            if (_data.CurrentBikMovie.Changed && _data.CurrentBikMovie.Old != String.Empty)
            {
                Debug.WriteLine($"Movie Changed - {_data.CurrentBikMovie.Old} -> {_data.CurrentBikMovie.Current} pos={pos}");

                // special case for Intro End split because two movies play back-to-back
                // which can cause isLoading to not detect changes
                if (_data.CurrentBikMovie.Current == "LoadingPrison" && _data.CurrentBikMovie.Old == "Dishonored")
                {
                    _loadingStarted = true;

                    OnLoadStarted?.Invoke(this, EventArgs.Empty);
                    OnAreaCompleted?.Invoke(this, AreaCompletionType.IntroEnd);
                }
            }

            if (_data.CurrentLevel.Changed)
            {
                Debug.WriteLine($"Level Changed - {_data.CurrentLevel.Old} -> {_data.CurrentLevel.Current} '{currentLevelStr}' pos={pos}");

                if (currentLevelStr == "DLC06_Tower_P" || currentLevelStr == "L_DLC07_BaseIntro_P")
                    OnFirstLevelLoading?.Invoke(this, EventArgs.Empty);

                _oncePerLevelFlag = true;
            }

            if (_data.IsLoading.Changed)
            {
                if (_data.IsLoading.Current)
                {
                    Debug.WriteLine($"Load Start - {combinedStr} pos={pos}");

                    // ignore the intro sequence and the dishonored logo screen
                    if (currentMovie != "INTRO_LOC" && currentMovie != "Dishonored")
                    {
                        _loadingStarted = true;
                        _previousLevel = level;
                        OnLoadStarted?.Invoke(this, EventArgs.Empty);
                    }

                    AreaCompletionType completionType = _areaCompletions.Where(c => combinedStr.ToLower().StartsWith(c.Key.ToLower())).Select(c => c.Value).FirstOrDefault();
                    if (completionType != AreaCompletionType.None)
                        OnAreaCompleted?.Invoke(this, completionType);

                    _movieTimer.Stop();
                    _cutsceneTimer.Stop();
                    _loadTimer.Stop();
                }
                else
                {
                    Debug.WriteLine($"Load End - {combinedStr} pos={pos}");

                    if (_loadingStarted)
                    {
                        _loadingStarted = false;
                        OnLoadFinished?.Invoke(this, level, _previousLevel, movie, pos);
                    }

                    if (((currentMovie == "LoadingEmpressTower" || currentMovie == "INTRO_LOC") && currentLevelStr == "l_tower_p")
                        || (currentMovie == "Loading" || currentMovie == "LoadingDLC06Tower") && currentLevelStr == "DLC06_Tower_P") // KoD
                    {
                        OnPlayerGainedControl?.Invoke(this, EventArgs.Empty);
                    }

                    if ((currentMovie == "Loading" || currentMovie == "LoadingDLC07Intro")
                        && currentLevelStr == "L_DLC07_BaseIntro_P" && _data.PlayerPos.Current.X == -1831.55188f)
                    {
                        OnPlayerGainedControl?.Invoke(this, EventArgs.Empty);
                        _oncePerLevelFlag = false;
                    }

                    if (currentMovie.StartsWith("Loading"))
                    {
                        _loadTimer.Interval = 5000;
                        _loadTimer.Start();
                    }
                    else if (movie != Movie.None)
                    {
                        _movieTimer.Interval = 8000;
                        _movieTimer.Start();
                    }
                }
            }

            if (_data.PlayerPos.Changed && _loadingStarted &&
                _data.PlayerPos.Old.X == 0.0f && Math.Abs(pos.X-9826.25f) < 0.25f)
            {
                OnFirstLevelLoading?.Invoke(this, EventArgs.Empty);
            }

            if (_data.CutsceneActive.Changed)
            {
                Debug.WriteLine($"In-Game Cutscene {(_data.CutsceneActive.Current ? "Start" : "End")} - {combinedStr} pos={pos}");

                if (_data.CutsceneActive.Current && currentLevelStr.StartsWith("L_LightH_"))
                {
                    OnPlayerLostControl?.Invoke(this, EventArgs.Empty);
                }
                else if (!_data.CutsceneActive.Current && currentLevelStr == "L_DLC07_BaseIntro_P" && _oncePerLevelFlag)
                {
                    _oncePerLevelFlag = false;
                    OnPlayerGainedControl?.Invoke(this, EventArgs.Empty);
                }

                if (_data.CutsceneActive.Current)
                {
                    _cutsceneTimer.Interval = 5000;
                    _cutsceneTimer.Start();
                }
            }

            if (_data.MissionStatsScreenFlags.Changed && _data.MissionStatsScreenActive.Current)
            {
                Debug.WriteLine($"Mission End pos={pos}");
                OnAreaCompleted?.Invoke(this, AreaCompletionType.MissionEnd);
            }

            if (_worldSpeedInjected && (_movieTimer.Enabled || _cutsceneTimer.Enabled || _loadTimer.Enabled) && (_data.PlayerPos.Changed))
            {
                if (_movieTimer.Enabled)
                {
                    OnPostMoviePlayerPositionChanged?.Invoke(this, movie, pos);
                }
                if (_cutsceneTimer.Enabled)
                {
                    OnPostCutscenePlayerPositionChanged?.Invoke(this, level, pos);
                }
                if (_loadTimer.Enabled)
                {
                    OnPostLoadPlayerPositionChanged?.Invoke(this, level, _previousLevel, pos);
                }
            }
        }

        bool TryGetGameProcess()
        {
            _worldSpeedInjected = false;
            _scanAttempts = 5;

            Process game = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.ToLower() == "dishonored"
                && !p.HasExited && !_ignorePIDs.Contains(p.Id));
            if (game == null)
                return false;

            ProcessModuleWow64Safe binkw = game.ModulesWow64Safe().FirstOrDefault(p => p.ModuleName.ToLower() == "binkw32.dll" || p.ModuleName.ToLower() == "binkw64.dll");
            if (binkw == null)
                return false;

            if (binkw.ModuleMemorySize != (int)ExpectedDllSizes.BinkW32Dll && binkw.ModuleMemorySize != (int)ExpectedDllSizes.BinkW64Dll)
            {
                _ignorePIDs.Add(game.Id);
                MessageBox.Show("binkw32.dll was not the expected version.", "LiveSplit.Dishonored",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            GameVersion version;
            switch (game.MainModuleWow64Safe().ModuleMemorySize)
            {
                case (int)ExpectedDllSizes.DishonoredExe12:
                    version = GameVersion.v12;
                    break;
                case (int)ExpectedDllSizes.DishonoredExe14Reloaded:
                case (int)ExpectedDllSizes.DishonoredExe14Steam:
                    version = GameVersion.v14;
                    break;
                case (int)ExpectedDllSizes.DishonoredExe15:
                    version = GameVersion.v15;
                    break;
                case (int)ExpectedDllSizes.DishonoredExeEGS:
                    version = GameVersion.EGS;
                    break;
                default:
                    Debug.WriteLine($"Unknown game of size {game.MainModuleWow64Safe().ModuleMemorySize} found");
                    _ignorePIDs.Add(game.Id);
                    MessageBox.Show("Unexpected game version. Dishonored Steam (1.2, 1.4, or 1.5) or EGS is required.",
                        "LiveSplit.Dishonored", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
            }

            Debug.WriteLine("game version " + version);
            _data = new GameData(version);
            _process = game;

            return true;
        }

        bool TryInjection()
        {
            // scanning 1.4 doesn't seem to work immediately
            _scanAttempts--;
            if (_scanAttempts < 0)
                return false;

            SigScanTarget target;
            if (_process.Is64Bit())
            {
                target = new SigScanTarget("F3 44 0F 11 9F 8C 04 00 00 F3 44 0F 11 AF 88 04 00 00");
                _overwriteBytes = 18;
            }
            else
            {
                target = new SigScanTarget("F3 0F 11 86 AC 03 00 00 F3 0F 11 8E B8 03 00 00");
                _overwriteBytes = 8;
            }

            var ptr = IntPtr.Zero;
            foreach (var page in _process.MemoryPages(true))
            {
                var scanner = new SignatureScanner(_process, page.BaseAddress, (int)page.RegionSize);
                ptr = scanner.Scan(target);
                if (ptr != IntPtr.Zero)
                    break;
            }
            if (ptr == IntPtr.Zero)
            {
                Debug.WriteLine($"Unable to find world speed pointer, trying {_scanAttempts} more times");
                return false;
            }

            _worldSpeedPtr = ptr;
            var returnHerePtr = _worldSpeedPtr + _overwriteBytes;
            Debug.WriteLine($"worldSpeedPtr={_worldSpeedPtr.ToString("X")}");
            Debug.WriteLine($"returnHerePtr={returnHerePtr.ToString("X")}");

            _injectedFuncPtr = _process.AllocateMemory(128);
            Debug.WriteLine($"injectedFuncPtr={_injectedFuncPtr.ToString("X")}");

            _setWorldSpeedPtr = _process.AllocateMemory(sizeof(float));
            _process.WriteBytes(_setWorldSpeedPtr, BitConverter.GetBytes(1f));
            Debug.WriteLine($"setWorldSpeedPtr={_setWorldSpeedPtr.ToString("X")}");

            var worldSpeedDetourBytes = new List<byte>();
            if (_process.Is64Bit())
            {
                var worldSpeedOffset = (uint)_setWorldSpeedPtr - (uint)_injectedFuncPtr;
                worldSpeedDetourBytes.AddRange(new byte[] {
                    0xF3, 0x44, 0x0F, 0x10, 0x1D,                           // movss xmm11,[setWorldSpeed]
                });
                worldSpeedDetourBytes.AddRange(BitConverter.GetBytes(worldSpeedOffset - 9));
                worldSpeedDetourBytes.AddRange(new byte[] {
                    0xF3, 0x44, 0x0F, 0x10, 0x2D,                           // movss xmm13,[setWorldSpeed]
                });
                worldSpeedDetourBytes.AddRange(BitConverter.GetBytes(worldSpeedOffset - 18));
                worldSpeedDetourBytes.AddRange(new byte[] {
                    0xF3, 0x44, 0x0F, 0x11, 0x9F, 0x8C, 0x04, 0x00, 0x00,   // movss[rdi + 0000048C],xmm11
                    0xF3, 0x44, 0x0F, 0x11, 0xAF, 0x88, 0x04, 0x00, 0x00,   // movss[rdi + 00000488],xmm13
                });
            }
            else
            {
                var setWorldSpeedPtrBytes = BitConverter.GetBytes((uint)_setWorldSpeedPtr);
                worldSpeedDetourBytes.AddRange(new byte[] {
                    0xF3, 0x0F, 0x10, 0x05,                         // movss xmm0,[setWorldSpeed]
                });
                worldSpeedDetourBytes.AddRange(setWorldSpeedPtrBytes);
                worldSpeedDetourBytes.AddRange(new byte[] {
                    0xF3, 0x0F, 0x10, 0x15,                         // movss xmm2,[setWorldSpeed]
                });
                worldSpeedDetourBytes.AddRange(setWorldSpeedPtrBytes);
                worldSpeedDetourBytes.AddRange(new byte[] {
                    0xF3, 0x0F, 0x11, 0x86, 0xAC, 0x03, 0x00, 0x00, // movss [esi+000003AC],xmm0
                });
            }

            var jumpOffset = worldSpeedDetourBytes.Count;

            _process.Suspend();
            try
            {
                _copyWorldSpeedPtr = _process.WriteDetour(_worldSpeedPtr, _overwriteBytes, _injectedFuncPtr);
                Debug.WriteLine($"copyWorldSpeedPtr={_copyWorldSpeedPtr.ToString("X")}");
                _process.WriteBytes(_injectedFuncPtr, worldSpeedDetourBytes.ToArray());
                _process.WriteJumpInstruction(_injectedFuncPtr + jumpOffset, returnHerePtr);
                Debug.WriteLine("injection successful");
                _worldSpeedInjected = true;
            }
            catch
            {
                FreeMemory();
                throw;
            }
            finally
            {
                _process.Resume();
            }

            return true;
        }

        void FreeMemory()
        {
            if (_process == null || _process.HasExited)
                return;

            Debug.WriteLine("Freeing memory");
            if (_setWorldSpeedPtr != IntPtr.Zero)
                _process.FreeMemory(_setWorldSpeedPtr);
            if (_injectedFuncPtr != IntPtr.Zero)
                _process.FreeMemory(_injectedFuncPtr);
            if (_copyWorldSpeedPtr != IntPtr.Zero)
                _process.FreeMemory(_copyWorldSpeedPtr);
        }

        public void Dispose()
        {
            _movieTimer?.Dispose();
            _cutsceneTimer?.Dispose();
            _loadTimer?.Dispose();

            if (_process == null || _process.HasExited)
                return;

            _process.Suspend();
            try
            {
                if (_copyWorldSpeedPtr != IntPtr.Zero)
                {
                    var bytes = _process.ReadBytes(_copyWorldSpeedPtr, _overwriteBytes);
                    _process.WriteBytes(_worldSpeedPtr, bytes);
                }
            }
            finally
            {
                _process.Resume();
                FreeMemory();
            }
        }

        public void SetWorldSpeed(float value)
        {
            if (_process == null || _process.HasExited || _setWorldSpeedPtr == IntPtr.Zero)
                return;

            _process.WriteBytes(_setWorldSpeedPtr, BitConverter.GetBytes(value));
        }

        string GetEngineStringByID(int id)
        {
            DeepPointer ptr;
            if (_process.Is64Bit())
            {
                ptr = new DeepPointer(_data.StringTableBase, (id * 8), 0x14);
            }
            else
            {
                ptr = new DeepPointer(_data.StringTableBase, (id * 4), 0x10);
            }
            return ptr.DerefString(_process, 32);
        }

        void Timer_OnTick(object sender, EventArgs e)
        {
            ((Timer)sender).Stop();
        }
    }

    enum GameVersion
    {
        v12,
        v14,
        v15,
        EGS,
    }

    class FakeMemoryWatcher<T>
    {
        public T Current { get; set; }
        public T Old { get; set; }

        public FakeMemoryWatcher(T old, T current)
        {
            Old = old;
            Current = current;
        }
    }

    // Copied from LiveSplit.ComponentUtil.ExtensionMethods and adjusted
    // It uses a bad jmp command for 64 bit processes that corrupts the registers :(
    public static class ExtensionMethods
    {
        private static bool WriteJumpOrCall(Process process, IntPtr addr, IntPtr dest, bool call)
        {
            bool is64Bit = process.Is64Bit();
            int size = (is64Bit ? 14 : 5);
            List<byte> list = new List<byte>(size);
            if (is64Bit)
            {
                list.AddRange(new byte[6] { 0xFF, 0x25, 0x00, 0x00, 0x00, 0x00 });
                list.AddRange(BitConverter.GetBytes((long)dest));
            }
            else
            {
                int value = (int)dest - (int)(addr + size);
                list.AddRange(new byte[1] { (byte)(call ? 232 : 233) });
                list.AddRange(BitConverter.GetBytes(value));
            }

            process.VirtualProtect(addr, size, MemPageProtect.PAGE_EXECUTE_READWRITE, out var oldProtect);
            bool result = process.WriteBytes(addr, list.ToArray());
            process.VirtualProtect(addr, size, oldProtect);
            return result;
        }

        public static bool WriteJumpInstruction(this Process process, IntPtr addr, IntPtr dest)
        {
            return WriteJumpOrCall(process, addr, dest, call: false);
        }

        public static bool WriteCallInstruction(this Process process, IntPtr addr, IntPtr dest)
        {
            return WriteJumpOrCall(process, addr, dest, call: true);
        }

        public static IntPtr WriteDetour(this Process process, IntPtr src, int overwrittenBytes, IntPtr dest)
        {
            int jmpSize = (process.Is64Bit() ? 14 : 5);
            if (overwrittenBytes < jmpSize)
            {
                throw new ArgumentOutOfRangeException("overwrittenBytes", $"must be >= length of jmp instruction ({jmpSize})");
            }

            IntPtr intPtr;
            if ((intPtr = process.AllocateMemory(jmpSize + overwrittenBytes)) == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            try
            {
                byte[] array = process.ReadBytes(src, overwrittenBytes);
                if (array == null)
                {
                    throw new Win32Exception();
                }

                if (!process.WriteBytes(intPtr, array))
                {
                    throw new Win32Exception();
                }

                if (!process.WriteJumpInstruction(intPtr + overwrittenBytes, src + overwrittenBytes))
                {
                    throw new Win32Exception();
                }

                if (!process.WriteJumpInstruction(src, dest))
                {
                    throw new Win32Exception();
                }

                int remainingBytes = overwrittenBytes - jmpSize;
                if (remainingBytes > 0)
                {
                    byte[] array2 = Enumerable.Repeat((byte)144, remainingBytes).ToArray();
                    if (!process.VirtualProtect(src + jmpSize, array2.Length, MemPageProtect.PAGE_EXECUTE_READWRITE, out var oldProtect))
                    {
                        throw new Win32Exception();
                    }

                    if (!process.WriteBytes(src + jmpSize, array2))
                    {
                        throw new Win32Exception();
                    }

                    process.VirtualProtect(src + jmpSize, array2.Length, oldProtect);
                    return intPtr;
                }

                return intPtr;
            }
            catch
            {
                process.FreeMemory(intPtr);
                throw;
            }
        }
    }
}
