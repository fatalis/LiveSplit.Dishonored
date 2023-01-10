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
            (this.MissionStatsScreenFlags.Old & 1) != 0,
            (this.MissionStatsScreenFlags.Current & 1) != 0);

        public int StringTableBase { get; }

        public GameData(GameVersion version)
        {
            if (version == GameVersion.v12)
            {
                this.PlayerPosX = new MemoryWatcher<float>(new DeepPointer(0xFCCBDC, 0xC4));
                this.CurrentLevel = new MemoryWatcher<int>(new DeepPointer(0xFB7838, 0x2c0, 0x314, 0, 0x38));
                this.CurrentBikMovie = new StringWatcher(new DeepPointer(0xFC6AD4, 0x48, 0), 64);
                this.CutsceneActive = new MemoryWatcher<bool>(new DeepPointer(0xFB51CC, 0x744));
                this.MissionStatsScreenFlags = new MemoryWatcher<int>(new DeepPointer(0xFDEB08, 0x24, 0x41C, 0x2E0, 0xC4));
                this.StringTableBase = 0xFA3624;
                this.IsLoading = new MemoryWatcher<bool>(new DeepPointer("binkw32.dll", 0x312F4));
            }
            else if (version == GameVersion.v14)
            {
                this.PlayerPosX = new MemoryWatcher<float>(new DeepPointer(0x1052DE8, 0xC4));
                this.CurrentLevel = new MemoryWatcher<int>(new DeepPointer(0x103D878, 0x2c0, 0x314, 0, 0x38));
                this.CurrentBikMovie = new StringWatcher(new DeepPointer(0x104CB18, 0x48, 0), 64);
                this.CutsceneActive = new MemoryWatcher<bool>(new DeepPointer(0x103B20C, 0x744));
                this.MissionStatsScreenFlags = new MemoryWatcher<int>(new DeepPointer(0x1065184, 0x24, 0x41C, 0x2F4, 0xC4));
                this.StringTableBase = 0x1029664;
                this.IsLoading = new MemoryWatcher<bool>(new DeepPointer("binkw32.dll", 0x312F4));
            }
            else if (version == GameVersion.EGS)
            {
                this.PlayerPosX = new MemoryWatcher<float>(new DeepPointer(0x1815310, 0xb0));
                this.CurrentLevel = new MemoryWatcher<int>(new DeepPointer(0x1815310, 0x1a0, 0x5b8));
                this.CurrentBikMovie = new StringWatcher(new DeepPointer(0x1810348, 0x48, 0), 64);
                this.CutsceneActive = new MemoryWatcher<bool>(new DeepPointer(0x1802D88, 0x9ec));
                this.MissionStatsScreenFlags = new MemoryWatcher<int>(new DeepPointer(0x18292F8, 0x3c, 0x550, 0x420, 0x110));
                this.StringTableBase = 0x17F4270;
                this.IsLoading = new MemoryWatcher<bool>(new DeepPointer("binkw64.dll", 0x31494));
            }

            this.CurrentLevel.FailAction = MemoryWatcher.ReadFailAction.SetZeroOrNull;

            this.AddRange(this.GetType().GetProperties()
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
            Weepers,
            DLC06IntroEnd,
        }

        public event EventHandler OnFirstLevelLoading;
        public event EventHandler OnPlayerGainedControl;
        public event EventHandler OnLoadStarted;
        public event EventHandler OnLoadFinished;
        public event EventHandler OnPlayerLostControl;
        public delegate void AreaCompletedEventHandler(object sender, AreaCompletionType type);
        public event AreaCompletedEventHandler OnAreaCompleted;

        private List<int> _ignorePIDs;

        private GameData _data;
        private Process _process;
        private bool _loadingStarted;
        private bool _oncePerLevelFlag;

        private enum ExpectedDllSizes
        {
            DishonoredExe12 = 18219008,
            DishonoredExe14Reloaded = 18862080,
            DishonoredExe14Steam = 19427328,
            DishonoredExeEGS = 27553792,
            BinkW32Dll = 241664,
            BinkW64Dll = 364544,
        }

        private Dictionary<string, AreaCompletionType> _areaCompletions = new Dictionary<string, AreaCompletionType>
        {
            ["LoadingSewers|L_Prison_"]            = AreaCompletionType.PrisonEscape,
            ["LoadingStreets|L_Pub_Dusk_"]         = AreaCompletionType.OutsidersDream,
            ["LoadingStreets|L_Pub_Day_"]          = AreaCompletionType.Weepers,
            ["LoadingDLC06Slaughter|DLC06_Tower_"] = AreaCompletionType.DLC06IntroEnd,
        };

        public GameMemory()
        {
            _ignorePIDs = new List<int>();
        }

        public void Update()
        {
            if (_process == null || _process.HasExited)
            {
                if (!this.TryGetGameProcess())
                    return;
            }

            TimedTraceListener.Instance.UpdateCount++;

            _data.UpdateAll(_process);

            if (_data.CurrentBikMovie.Changed && _data.CurrentBikMovie.Old != String.Empty)
            {
                Debug.WriteLine($"Movie Changed - {_data.CurrentBikMovie.Old} -> {_data.CurrentBikMovie.Current}");

                // special case for Intro End split because two movies play back-to-back
                // which can cause isLoading to not detect changes
                if (_data.CurrentBikMovie.Current == "LoadingPrison" && _data.CurrentBikMovie.Old == "Dishonored")
                {
                    _loadingStarted = true;

                    this.OnLoadStarted?.Invoke(this, EventArgs.Empty);
                    this.OnAreaCompleted?.Invoke(this, AreaCompletionType.IntroEnd);
                }
            }

            if (_data.CurrentLevel.Changed)
            {
                string currentLevelStr = this.GetEngineStringByID(_data.CurrentLevel.Current);
                Debug.WriteLine($"Level Changed - {_data.CurrentLevel.Old} -> {_data.CurrentLevel.Current} '{currentLevelStr}'");

                if (currentLevelStr == "DLC06_Tower_P" || currentLevelStr == "L_DLC07_BaseIntro_P")
                    this.OnFirstLevelLoading?.Invoke(this, EventArgs.Empty);

                _oncePerLevelFlag = true;
            }

            if (_data.IsLoading.Changed)
            {
                string currentMovie = _data.CurrentBikMovie.Current;
                string currentLevelStr = this.GetEngineStringByID(_data.CurrentLevel.Current);

                if (_data.IsLoading.Current)
                {
                    Debug.WriteLine($"Load Start - {currentMovie + "|" + currentLevelStr}");

                    // ignore the intro sequence and the dishonored logo screen
                    if (currentMovie != "INTRO_LOC" && currentMovie != "Dishonored")
                    {
                        // ignore intro end if it happens, see special case above
                        if (!(currentMovie == "LoadingPrison" && currentLevelStr.ToLower().StartsWith("L_Tower_")))
                        {
                            _loadingStarted = true;
                            this.OnLoadStarted?.Invoke(this, EventArgs.Empty);
                        }
                    }

                    AreaCompletionType completionType = _areaCompletions.Where(c => (currentMovie.ToLower() + "|" + currentLevelStr.ToLower()).StartsWith(c.Key.ToLower())).Select(c => c.Value).FirstOrDefault();
                    if (completionType != AreaCompletionType.None)
                        this.OnAreaCompleted?.Invoke(this, completionType);
                }
                else
                {
                    Debug.WriteLine($"Load End - {currentMovie + "|" + currentLevelStr}");

                    if (_loadingStarted)
                    {
                        _loadingStarted = false;
                        this.OnLoadFinished?.Invoke(this, EventArgs.Empty);
                    }

                    if (((currentMovie == "LoadingEmpressTower" || currentMovie == "INTRO_LOC") && currentLevelStr == "l_tower_p")
                        || (currentMovie == "Loading" || currentMovie == "LoadingDLC06Tower") && currentLevelStr == "DLC06_Tower_P") // KoD
                    {
                        this.OnPlayerGainedControl?.Invoke(this, EventArgs.Empty);
                    }

                    if ((currentMovie == "Loading" || currentMovie == "LoadingDLC07Intro") &&
                        currentLevelStr == "L_DLC07_BaseIntro_P" && _data.PlayerPosX.Current == -1831.55188f)
                    {
                        this.OnPlayerGainedControl?.Invoke(this, EventArgs.Empty);
                        _oncePerLevelFlag = false;
                    }
                }
            }

            if (_data.PlayerPosX.Changed && _loadingStarted &&
                _data.PlayerPosX.Old == 0.0f && Math.Abs(_data.PlayerPosX.Current-9826.25f) < 0.25f)
            {
                string currentLevelStr = this.GetEngineStringByID(_data.CurrentLevel.Current);

                if(currentLevelStr == "l_tower_p")
                    this.OnFirstLevelLoading?.Invoke(this, EventArgs.Empty);
            }

            if (_data.CutsceneActive.Changed)
            {
                string currentLevelStr = this.GetEngineStringByID(_data.CurrentLevel.Current);
                Debug.WriteLine($"In-Game Cutscene {(_data.CutsceneActive.Current ? "Start" : "End")}");

                if (_data.CutsceneActive.Current && currentLevelStr.StartsWith("L_LightH_"))
                {
                    this.OnPlayerLostControl?.Invoke(this, EventArgs.Empty);
                }
                else if (!_data.CutsceneActive.Current && currentLevelStr == "L_DLC07_BaseIntro_P" && _oncePerLevelFlag)
                {
                    _oncePerLevelFlag = false;
                    this.OnPlayerGainedControl?.Invoke(this, EventArgs.Empty);
                }
            }

            if (_data.MissionStatsScreenFlags.Changed && _data.MissionStatsScreenActive.Current)
            {
                Debug.WriteLine("Mission End");
                this.OnAreaCompleted?.Invoke(this, AreaCompletionType.MissionEnd);
            }
        }

        bool TryGetGameProcess()
        {
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
            if (game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.DishonoredExe12)
            {
                version = GameVersion.v12;
            }
            else if (game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.DishonoredExe14Reloaded || game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.DishonoredExe14Steam)
            {
                version = GameVersion.v14;
            }
            else if (game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.DishonoredExeEGS)
            {
                version = GameVersion.EGS;
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
    }

    enum GameVersion
    {
        v12,
        v14,
        EGS,
    }

    class FakeMemoryWatcher<T>
    {
        public T Current { get; set; }
        public T Old { get; set; }

        public FakeMemoryWatcher(T old, T current)
        {
            this.Old = old;
            this.Current = current;
        }
    }
}
