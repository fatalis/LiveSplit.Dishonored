using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using LiveSplit.ComponentUtil;

namespace LiveSplit.Dishonored
{
    class GameData : MemoryWatcherList
    {
        public MemoryWatcher<float> PlayerPosX { get; }
        public MemoryWatcher<float> PlayerPosY { get; }
        public MemoryWatcher<float> PlayerPosZ { get; }
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
                PlayerPosX = new MemoryWatcher<float>(new DeepPointer(0xFCCBDC, 0xC4));
                PlayerPosY = new MemoryWatcher<float>(new DeepPointer(0xFCCBDC, 0xC8));
                PlayerPosZ = new MemoryWatcher<float>(new DeepPointer(0xFCCBDC, 0xCC));
                CurrentLevel = new MemoryWatcher<int>(new DeepPointer(0xFB7838, 0x2c0, 0x314, 0, 0x38));
                CurrentBikMovie = new StringWatcher(new DeepPointer(0xFC6AD4, 0x48, 0), 64);
                CutsceneActive = new MemoryWatcher<bool>(new DeepPointer(0xFB51CC, 0x744));
                MissionStatsScreenFlags = new MemoryWatcher<int>(new DeepPointer(0xFDEB08, 0x24, 0x41C, 0x2E0, 0xC4));
                StringTableBase = 0xFA3624;
            }
            else if (version == GameVersion.v14)
            {
                PlayerPosX = new MemoryWatcher<float>(new DeepPointer(0x1052DE8, 0xC4));
                PlayerPosY = new MemoryWatcher<float>(new DeepPointer(0x1052DE8, 0xC8));
                PlayerPosZ = new MemoryWatcher<float>(new DeepPointer(0x1052DE8, 0xCC));
                CurrentLevel = new MemoryWatcher<int>(new DeepPointer(0x103D878, 0x2c0, 0x314, 0, 0x38));
                CurrentBikMovie = new StringWatcher(new DeepPointer(0x104CB18, 0x48, 0), 64);
                CutsceneActive = new MemoryWatcher<bool>(new DeepPointer(0x103B20C, 0x744));
                MissionStatsScreenFlags = new MemoryWatcher<int>(new DeepPointer(0x1065184, 0x24, 0x41C, 0x2F4, 0xC4));
                StringTableBase = 0x1029664;
            }

            CurrentLevel.FailAction = MemoryWatcher.ReadFailAction.SetZeroOrNull;

            IsLoading = new MemoryWatcher<bool>(new DeepPointer("binkw32.dll", 0x312F4));

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
            Weepers
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
        public delegate void LoadFinishedHandler(object sender, Level level, Level previousLevel, Movie movie, float x, float y, float z);
        public event LoadFinishedHandler OnLoadFinished;
        public event EventHandler OnPlayerLostControl;
        public delegate void AreaCompletedEventHandler(object sender, AreaCompletionType type);
        public event AreaCompletedEventHandler OnAreaCompleted;
        public delegate void PostMoviePlayerPositionChangedHandler(object sender, Movie movie, float x, float y, float z);
        public event PostMoviePlayerPositionChangedHandler OnPostMoviePlayerPositionChanged;
        public delegate void PostCutscenePlayerPositionChangedHandler(object sender, Level level, int count, float x, float y, float z);
        public event PostCutscenePlayerPositionChangedHandler OnPostCutscenePlayerPositionChanged;
        public delegate void PostLoadPlayerPositionChangedHandler(object sender, Level level, Level previousLevel, float x, float y, float z);
        public event PostLoadPlayerPositionChangedHandler OnPostLoadPlayerPositionChanged;

        private List<int> _ignorePIDs;

        private GameData _data;
        private Process _process;
        private bool _loadingStarted;
        private bool _oncePerLevelFlag;
        private Level _previousLevel;
        private int _cutsceneCount;

        private IntPtr _worldSpeedPtr;
        private IntPtr _setWorldSpeedPtr;
        private IntPtr _injectedFuncPtr;
        private IntPtr _copyWorldSpeedPtr;
        private int _overwriteBytes = 10;

        private Timer _movieTimer;
        private Timer _cutsceneTimer;
        private Timer _loadTimer;

        private enum ExpectedDllSizes
        {
            DishonoredExe12 = 18219008,
            DishonoredExe14Reloaded = 18862080,
            DishonoredExe14Steam = 19427328,
            BinkW32Dll = 241664
        }

        private Dictionary<string, AreaCompletionType> _areaCompletions = new Dictionary<string, AreaCompletionType>
        {
            ["LoadingSewers|L_Prison_"]    = AreaCompletionType.PrisonEscape,
            ["LoadingStreets|L_Pub_Dusk_"] = AreaCompletionType.OutsidersDream,
            ["LoadingStreets|L_Pub_Day_"]  = AreaCompletionType.Weepers,
        };

        private Dictionary<string, Level> _levels = new Dictionary<string, Level>
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

        private Dictionary<string, Movie> _movies = new Dictionary<string, Movie>
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
            _movieTimer.Tick += movieTimer_OnTick;
            _cutsceneTimer = new Timer { Enabled = false };
            _cutsceneTimer.Tick += cutsceneTimer_OnTick;
            _loadTimer = new Timer { Enabled = false };
            _loadTimer.Tick += loadTimer_OnTick;
        }

        public void Update(bool logCoords)
        {
            if (_process == null || _process.HasExited)
            {
                if (!TryGetGameProcess())
                    return;
            }

            TimedTraceListener.Instance.UpdateCount++;

            _data.UpdateAll(_process);

            var currentMovie = _data.CurrentBikMovie.Current;
            var currentLevelStr = GetEngineStringByID(_data.CurrentLevel.Current);
            var combinedStr = currentMovie + "|" + currentLevelStr;
            var level = _levels.Where(l => currentLevelStr.ToLower().StartsWith(l.Key.ToLower())).Select(l => l.Value).FirstOrDefault();
            _movies.TryGetValue(currentMovie, out var movie);
            var x = _data.PlayerPosX.Current;
            var y = _data.PlayerPosY.Current;
            var z = _data.PlayerPosZ.Current;

            if (logCoords && _movieTimer.Enabled || _cutsceneTimer.Enabled || _loadTimer.Enabled)
            {
                Debug.WriteLine($"x={x} y={y} z={z}");
            }

            if (_data.CurrentBikMovie.Changed && _data.CurrentBikMovie.Old != String.Empty)
            {
                Debug.WriteLine($"Movie Changed - {_data.CurrentBikMovie.Old} -> {_data.CurrentBikMovie.Current} x={x}");

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
                Debug.WriteLine($"Level Changed - {_data.CurrentLevel.Old} -> {_data.CurrentLevel.Current} '{currentLevelStr}' x={x}");

                if (currentLevelStr == "L_DLC07_BaseIntro_P" || currentLevelStr == "DLC06_Tower_P")
                    OnFirstLevelLoading?.Invoke(this, EventArgs.Empty);

                _oncePerLevelFlag = true;
            }

            if (_data.IsLoading.Changed)
            {
                if (_data.IsLoading.Current)
                {
                    Debug.WriteLine($"Load Start - {combinedStr} x={x}");

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
                    Debug.WriteLine($"Load End - {combinedStr} x={x}");

                    if (_loadingStarted)
                    {
                        _loadingStarted = false;
                        OnLoadFinished?.Invoke(this, level, _previousLevel, movie, x, y, z);

                        if (level != _previousLevel)
                        {
                            _cutsceneCount = 0;
                        }
                    }

                    if (((currentMovie == "LoadingEmpressTower" || currentMovie == "INTRO_LOC") && currentLevelStr == "l_tower_p")
                        || (currentMovie == "Loading" || currentMovie == "LoadingDLC06Tower") && currentLevelStr == "DLC06_Tower_P") // KoD
                    {
                        OnPlayerGainedControl?.Invoke(this, EventArgs.Empty);
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

            if (_data.PlayerPosX.Changed && _data.PlayerPosX.Old == 0.0f && _loadingStarted && x < 9826.5f && x > 9826.0f)
            {
                if (currentLevelStr == "l_tower_p")
                {
                    _cutsceneCount = 0;
                    OnFirstLevelLoading?.Invoke(this, EventArgs.Empty);
                }
            }

            if (_data.CutsceneActive.Changed)
            {
                Debug.WriteLine($"In-Game Cutscene {(_data.CutsceneActive.Current ? "Start" : "End")} - {combinedStr} x={x}");

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
                    _cutsceneCount++;
                    _cutsceneTimer.Interval = 5000;
                    _cutsceneTimer.Start();
                }
            }

            if (_data.MissionStatsScreenFlags.Changed && _data.MissionStatsScreenActive.Current)
            {
                Debug.WriteLine($"Mission End x={x}");
                OnAreaCompleted?.Invoke(this, AreaCompletionType.MissionEnd);
            }

            if ((_movieTimer.Enabled || _cutsceneTimer.Enabled || _loadTimer.Enabled) && (_data.PlayerPosX.Changed || _data.PlayerPosY.Changed || _data.PlayerPosZ.Changed))
            {
                if (_movieTimer.Enabled)
                {
                    OnPostMoviePlayerPositionChanged?.Invoke(this, movie, x, y, z);
                }
                if (_cutsceneTimer.Enabled)
                {
                    OnPostCutscenePlayerPositionChanged?.Invoke(this, level, _cutsceneCount, x, y, z);
                }
                if (_loadTimer.Enabled)
                {
                    OnPostLoadPlayerPositionChanged?.Invoke(this, level, _previousLevel, x, y, z);
                }
            }
        }

        bool TryGetGameProcess()
        {
            Process game = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.ToLower() == "dishonored"
                && !p.HasExited && !_ignorePIDs.Contains(p.Id));
            if (game == null)
                return false;

            ProcessModuleWow64Safe binkw32 = game.ModulesWow64Safe().FirstOrDefault(p => p.ModuleName.ToLower() == "binkw32.dll");
            if (binkw32 == null)
                return false;

            if (binkw32.ModuleMemorySize != (int)ExpectedDllSizes.BinkW32Dll)
            {
                _ignorePIDs.Add(game.Id);
                MessageBox.Show("binkw32.dll was not the expected version.", "LiveSplit.Dishonored",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            GameVersion version;
            if (game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.DishonoredExe12)
            {
                version = GameVersion.v12;
            }
            else if (game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.DishonoredExe14Reloaded || game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.DishonoredExe14Steam)
            {
                version = GameVersion.v14;
            }
            else
            {
                _ignorePIDs.Add(game.Id);
                MessageBox.Show("Unexpected game version. Dishonored 1.2 or 1.4 is required.", "LiveSplit.Dishonored",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);    
                return false;
            }

            Debug.WriteLine("game version " + version);
            _data = new GameData(version);
            _process = game;

            var ptr = IntPtr.Zero;
            var target = new SigScanTarget("8B 10 8B C8 8B 82 D0 03 00 00 53 FF D0");
            foreach (var page in game.MemoryPages(true))
            {
                var scanner = new SignatureScanner(game, page.BaseAddress, (int)page.RegionSize);
                ptr = scanner.Scan(target);
                if (ptr != IntPtr.Zero)
                {
                    break;
                }
            }
            if (ptr == IntPtr.Zero)
            {
                Debug.WriteLine("Unable to find world speed pointer");
                return true;
            }

            _worldSpeedPtr = ptr;
            var returnHerePtr = _worldSpeedPtr + _overwriteBytes;
            Debug.WriteLine($"worldSpeedPtr={_worldSpeedPtr.ToString("X")}");
            Debug.WriteLine($"returnHerePtr={returnHerePtr.ToString("X")}");

            _setWorldSpeedPtr = game.AllocateMemory(sizeof(float));
            game.WriteBytes(_setWorldSpeedPtr, BitConverter.GetBytes(1f));
            var setWorldSpeedPtrBytes = BitConverter.GetBytes((uint)_setWorldSpeedPtr);

            var worldSpeedDetourBytes = new List<byte>
            {
                0x8D, 0x88, 0xF0, 0x04, 0x00, 0x00, // lea ecx,[eax+000004F0]
                0x8B, 0x15,                         // mov edx,[setWorldSpeed]
            };
            worldSpeedDetourBytes.AddRange(setWorldSpeedPtrBytes);
            worldSpeedDetourBytes.AddRange(new byte[] {
                0x89, 0x51, 0xF0,                   // mov[ecx - 10],edx
                0x8B, 0x10,                         // mov edx,[eax]
                0x8B, 0xC8,                         // mov ecx,eax
                0x8B, 0x82, 0xD0, 0x03, 0x00, 0x00, // mov eax,[edx+000003D0]
            });
            var jumpOffset = worldSpeedDetourBytes.Count;
            worldSpeedDetourBytes.AddRange(new byte[] {
                255, 255, 255, 255, 255,            // jmp [returnHere] (placeholder)
            });

            _injectedFuncPtr = game.AllocateMemory(worldSpeedDetourBytes.Count);
            Debug.WriteLine($"injectedFuncPtr={_injectedFuncPtr.ToString("X")}");

            game.Suspend();
            try
            {
                _copyWorldSpeedPtr = game.WriteDetour(_worldSpeedPtr, _overwriteBytes, _injectedFuncPtr);
                Debug.WriteLine($"copyWorldSpeedPtr={_copyWorldSpeedPtr.ToString("X")}");
                game.WriteBytes(_injectedFuncPtr, worldSpeedDetourBytes.ToArray());
                game.WriteJumpInstruction(_injectedFuncPtr + jumpOffset, returnHerePtr);
                Debug.WriteLine("injection successful");
            }
            catch
            {
                FreeMemory();
                throw;
            }
            finally
            {
                game.Resume();
            }

            return true;
        }

        void FreeMemory()
        {
            if (_process == null || _process.HasExited)
            {
                return;
            }

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
            {
                return;
            }

            _process.Suspend();
            try
            {
                if (_copyWorldSpeedPtr != IntPtr.Zero)
                {
                    var bytes = _process.ReadBytes(_copyWorldSpeedPtr, _overwriteBytes);
                    _process.WriteBytes(_worldSpeedPtr, bytes);
                }
            }
            catch
            {
                throw;
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
            var ptr = new DeepPointer(_data.StringTableBase, (id * 4), 0x10);
            return ptr.DerefString(_process, 32);
        }

        void movieTimer_OnTick(object sender, EventArgs e)
        {
            _movieTimer.Stop();
        }

        void cutsceneTimer_OnTick(object sender, EventArgs e)
        {
            _cutsceneTimer.Stop();
        }

        void loadTimer_OnTick(object sender, EventArgs e)
        {
            _loadTimer.Stop();
        }
    }

    enum GameVersion
    {
        v12,
        v14
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
}
