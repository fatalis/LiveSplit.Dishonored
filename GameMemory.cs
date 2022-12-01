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
                CurrentLevel = new MemoryWatcher<int>(new DeepPointer(0xFB7838, 0x2c0, 0x314, 0, 0x38));
                CurrentBikMovie = new StringWatcher(new DeepPointer(0xFC6AD4, 0x48, 0), 64);
                CutsceneActive = new MemoryWatcher<bool>(new DeepPointer(0xFB51CC, 0x744));
                MissionStatsScreenFlags = new MemoryWatcher<int>(new DeepPointer(0xFDEB08, 0x24, 0x41C, 0x2E0, 0xC4));
                StringTableBase = 0xFA3624;
            }
            else if (version == GameVersion.v14)
            {
                PlayerPosX = new MemoryWatcher<float>(new DeepPointer(0x1052DE8, 0xC4));
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

    class GameMemory
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
        public delegate void LoadFinishedHandler(object sender, Level previousLevel, Level currentLevel, Movie movie, float playerPosX);
        public event LoadFinishedHandler OnLoadFinished;
        public event EventHandler OnPlayerLostControl;
        public delegate void AreaCompletedEventHandler(object sender, AreaCompletionType type);
        public event AreaCompletedEventHandler OnAreaCompleted;
        public delegate void CutsceneStartedHandler(object sender, Level level, float playerPosX, bool isLoading);
        public event CutsceneStartedHandler OnCutsceneStarted;
        public delegate void MovieEndedHandler(object sender, Movie movie);
        public event MovieEndedHandler OnMovieEnded;

        private List<int> _ignorePIDs;

        private GameData _data;
        private Process _process;
        private bool _loadingStarted;
        private bool _oncePerLevelFlag;
        private Level _previousLevel = Level.None;

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
        }

        public void Update()
        {
            if (_process == null || _process.HasExited)
            {
                if (!TryGetGameProcess())
                    return;
            }

            TimedTraceListener.Instance.UpdateCount++;

            _data.UpdateAll(_process);

            if (_data.CurrentBikMovie.Changed && _data.CurrentBikMovie.Old != String.Empty)
            {
                Debug.WriteLine($"Movie Changed - {_data.CurrentBikMovie.Old} -> {_data.CurrentBikMovie.Current} x={_data.PlayerPosX.Current}");

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
                string currentLevelStr = GetEngineStringByID(_data.CurrentLevel.Current);
                Debug.WriteLine($"Level Changed - {_data.CurrentLevel.Old} -> {_data.CurrentLevel.Current} '{currentLevelStr}' x={_data.PlayerPosX.Current}");

                if (currentLevelStr == "L_DLC07_BaseIntro_P" || currentLevelStr == "DLC06_Tower_P")
                    OnFirstLevelLoading?.Invoke(this, EventArgs.Empty);

                _oncePerLevelFlag = true;
            }

            if (_data.IsLoading.Changed)
            {
                string currentMovie = _data.CurrentBikMovie.Current;
                string currentLevelStr = GetEngineStringByID(_data.CurrentLevel.Current);
                string combinedStr = currentMovie + "|" + currentLevelStr;
                Level level = _levels.Where(l => currentLevelStr.ToLower().StartsWith(l.Key.ToLower())).Select(l => l.Value).FirstOrDefault();

                if (_data.IsLoading.Current)
                {
                    Debug.WriteLine($"Load Start - {combinedStr} x={_data.PlayerPosX.Current}");

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
                }
                else
                {
                    Debug.WriteLine($"Load End - {combinedStr} x={_data.PlayerPosX.Current}");

                    if (_loadingStarted)
                    {
                        _loadingStarted = false;
                        var movie = _movies.Where(m => currentMovie == m.Key).Select(m => m.Value).FirstOrDefault();
                        OnLoadFinished?.Invoke(this, _previousLevel, level, movie, _data.PlayerPosX.Current);
                    }

                    if (((currentMovie == "LoadingEmpressTower" || currentMovie == "INTRO_LOC") && currentLevelStr == "l_tower_p")
                        || (currentMovie == "Loading" || currentMovie == "LoadingDLC06Tower") && currentLevelStr == "DLC06_Tower_P") // KoD
                    {
                        OnPlayerGainedControl?.Invoke(this, EventArgs.Empty);
                    }

                    if (!currentMovie.StartsWith("Loading"))
                    {
                        var movie = _movies.Where(m => currentMovie == m.Key).Select(m => m.Value).FirstOrDefault();
                        if (movie != Movie.None)
                        {
                            OnMovieEnded?.Invoke(this, movie);
                        }
                    }
                }
            }

            if (_data.PlayerPosX.Changed && _data.PlayerPosX.Old == 0.0f && _loadingStarted &&
                _data.PlayerPosX.Current<9826.5f && _data.PlayerPosX.Current>9826.0f)
            {
                string currentLevelStr = GetEngineStringByID(_data.CurrentLevel.Current);

                if (currentLevelStr == "l_tower_p")
                    OnFirstLevelLoading?.Invoke(this, EventArgs.Empty);
            }

            if (_data.CutsceneActive.Changed)
            {
                string currentMovie = _data.CurrentBikMovie.Current;
                string currentLevelStr = GetEngineStringByID(_data.CurrentLevel.Current);
                string combinedStr = currentMovie + "|" + currentLevelStr;
                Debug.WriteLine($"In-Game Cutscene {(_data.CutsceneActive.Current ? "Start" : "End")} - {combinedStr} x={_data.PlayerPosX.Current}");

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
                    Level level = _levels.Where(l => currentLevelStr.ToLower().StartsWith(l.Key.ToLower())).Select(l => l.Value).FirstOrDefault();
                    OnCutsceneStarted?.Invoke(this, level, _data.PlayerPosX.Current, _data.IsLoading.Current);
                }
            }

            if (_data.MissionStatsScreenFlags.Changed && _data.MissionStatsScreenActive.Current)
            {
                Debug.WriteLine($"Mission End x={_data.PlayerPosX.Current}");
                OnAreaCompleted?.Invoke(this, AreaCompletionType.MissionEnd);
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

            return true;
        }

        string GetEngineStringByID(int id)
        {
            var ptr = new DeepPointer(_data.StringTableBase, (id*4), 0x10);
            return ptr.DerefString(_process, 32);
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
