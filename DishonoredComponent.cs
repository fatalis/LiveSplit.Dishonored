using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.UI;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Windows.Forms;
using System.Collections.Generic;
using static LiveSplit.Dishonored.GameMemory;

namespace LiveSplit.Dishonored
{
    class CutsceneSpeedup
    {
        public Level Level { get; set; }
        public float PlayerPosX { get; set; }
        public string Setting { get; set; }

        private const float _tolerance = 3f;

        public bool ApproxEqual(float val)
        {
            return val > (PlayerPosX - _tolerance) && val < (PlayerPosX + _tolerance);
        }

        public bool Matches(Level level, float playerPosX)
        {
            return level == Level && ApproxEqual(playerPosX);
        }
    }

    class LoadSpeedup
    {
        public Level PreviousLevel { get; set; }
        public Level NextLevel { get; set; }
        public string Setting { get; set; }

        public bool Matches(Level previousLevel, Level nextLevel)
        {
            return previousLevel == PreviousLevel && nextLevel == NextLevel;
        }
    }

    class DishonoredComponent : LogicComponent
    {
        public override string ComponentName => "Dishonored";

        public DishonoredSettings Settings { get; }

        private TimerModel _timer;
        private GameMemory _gameMemory;
        private Timer _updateTimer;
        private Timer _cutsceneTimer;
        private string _pendingSpeedup = null;

        private List<CutsceneSpeedup> _cutsceneSpeedups = new List<CutsceneSpeedup>
        {
            new CutsceneSpeedup { Level = Level.Intro, PlayerPosX = 15476f, Setting = "SpeedupIntroEnd" },
            //new CutsceneSpeedup { Level = Level.PubDay, PlayerPosX = -8746.955f, Setting = "SpeedupPostSewers" },
            //new CutsceneSpeedup { Level = Level.CampbellStreets, PlayerPosX = 12755.4f, Setting = "SpeedupCampbell" },
            //new CutsceneSpeedup { Level = Level.PubMorning, PlayerPosX = -2710f, Setting = "SpeedupPostCampbell" },
            //new CutsceneSpeedup { Level = Level.CatStreets, PlayerPosX = 4182.706f, Setting = "SpeedupCat" },
            //new CutsceneSpeedup { Level = Level.PubDusk, PlayerPosX = -11286.98f, Setting = "SpeedupPostCat" },
            //new CutsceneSpeedup { Level = Level.Bridge1, PlayerPosX = -10696.94f, Setting = "SpeedupBridge" },
            //new CutsceneSpeedup { Level = Level.PubNight, PlayerPosX = -9352.788f, Setting = "SpeedupPostBridge" },
            //new CutsceneSpeedup { Level = Level.BoyleExterior, PlayerPosX = -2622.572f, Setting = "SpeedupBoyle" },
            //new CutsceneSpeedup { Level = Level.BoyleExterior, PlayerPosX = -2731.568f, Setting = "SpeedupBoyle" },
            //new CutsceneSpeedup { Level = Level.PubMorning, PlayerPosX = -2473f, Setting = "SpeedupPostBoyle" },
            //new CutsceneSpeedup { Level = Level.TowerReturnYard, PlayerPosX = -9341.69f, Setting = "SpeedupTower" },
            //new CutsceneSpeedup { Level = Level.PubDusk, PlayerPosX = -8807.757f, Setting = "SpeedupPostTower" },
            //new CutsceneSpeedup { Level = Level.FloodedIntro, PlayerPosX = -21076.77f, Setting = "SpeedupFlooded" },
            //new CutsceneSpeedup { Level = Level.KingsparrowIsland, PlayerPosX = -999.74f, Setting = "SpeedupKingsparrow" },
            //new CutsceneSpeedup { Level = Level.KingsparrowIsland, PlayerPosX = -1027.088f, Setting = "SpeedupKingsparrow" },
            //new CutsceneSpeedup { Level = Level.KingsparrowIsland, PlayerPosX = -1817.145f, Setting = "SpeedupKingsparrow" },
        };

        private List<LoadSpeedup> _loadSpeedups = new List<LoadSpeedup>
        {
            new LoadSpeedup { PreviousLevel = Level.Sewers, NextLevel = Level.PubDay, Setting = "SpeedupPostSewers"},
            new LoadSpeedup { PreviousLevel = Level.PubDusk, NextLevel = Level.CampbellStreets, Setting = "SpeedupCampbell"},
            new LoadSpeedup { PreviousLevel = Level.CampbellBack, NextLevel = Level.PubMorning, Setting = "SpeedupPostCampbell"},
            new LoadSpeedup { PreviousLevel = Level.PubDay, NextLevel = Level.CatStreets, Setting = "SpeedupCat"},
            new LoadSpeedup { PreviousLevel = Level.CatStreets, NextLevel = Level.PubDusk, Setting = "SpeedupPostCat"},
            new LoadSpeedup { PreviousLevel = Level.PubDusk, NextLevel = Level.Bridge1, Setting = "SpeedupBridge"},
            new LoadSpeedup { PreviousLevel = Level.Bridge4, NextLevel = Level.PubNight, Setting = "SpeedupPostBridge"},
            new LoadSpeedup { PreviousLevel = Level.PubDay, NextLevel = Level.BoyleExterior, Setting = "SpeedupBoyle"},
            new LoadSpeedup { PreviousLevel = Level.BoyleExterior, NextLevel = Level.PubMorning, Setting = "SpeedupPostBoyle"},
            new LoadSpeedup { PreviousLevel = Level.PubMorning, NextLevel = Level.TowerReturnYard, Setting = "SpeedupTower"},
            new LoadSpeedup { PreviousLevel = Level.TowerReturnYard, NextLevel = Level.PubDusk, Setting = "SpeedupPostTower"},
            new LoadSpeedup { PreviousLevel = Level.PubDusk, NextLevel = Level.FloodedIntro, Setting = "SpeedupFlooded"},
            new LoadSpeedup { PreviousLevel = Level.Loyalists, NextLevel = Level.KingsparrowIsland, Setting = "SpeedupKingsparrow"},
        };

        public DishonoredComponent(LiveSplitState state)
        {
#if DEBUG
            Debug.Listeners.Clear();
            Debug.Listeners.Add(TimedTraceListener.Instance);
#endif

            Settings = new DishonoredSettings();

            _timer = new TimerModel { CurrentState = state };
            _timer.CurrentState.OnStart += timer_OnStart;

            _updateTimer = new Timer() { Interval = 15, Enabled = true };
            _updateTimer.Tick += updateTimer_Tick;

            _cutsceneTimer = new Timer() { Enabled = true };
            _cutsceneTimer.Tick += cutsceneTimer_Tick;

            _gameMemory = new GameMemory();
            _gameMemory.OnFirstLevelLoading += gameMemory_OnFirstLevelLoading;
            _gameMemory.OnPlayerGainedControl += gameMemory_OnPlayerGainedControl;
            _gameMemory.OnLoadStarted += gameMemory_OnLoadStarted;
            _gameMemory.OnLoadFinished += gameMemory_OnLoadFinished;
            _gameMemory.OnPlayerLostControl += gameMemory_OnPlayerLostControl;
            _gameMemory.OnAreaCompleted += gameMemory_OnAreaCompleted;
            _gameMemory.OnCutsceneStarted += gameMemory_OnCutsceneStarted;
        }

        public override void Dispose()
        {
            _timer.CurrentState.OnStart -= timer_OnStart;
            _updateTimer?.Dispose();
            _cutsceneTimer?.Dispose();
        }

        void updateTimer_Tick(object sender, EventArgs eventArgs)
        {
            try
            {
                _gameMemory.Update();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        void cutsceneTimer_Tick(object sender, EventArgs eventArgs)
        {
            SendKeys.Send("{F6}");
            _cutsceneTimer.Stop();
        }

        void timer_OnStart(object sender, EventArgs e)
        {
            _timer.InitializeGameTime();
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {

        }

        void gameMemory_OnFirstLevelLoading(object sender, EventArgs e)
        {
            if (Settings.AutoStartEnd)
                _timer.Reset();
        }

        void gameMemory_OnPlayerGainedControl(object sender, EventArgs e)
        {
            if (Settings.AutoStartEnd)
                _timer.Start();
        }

        void gameMemory_OnLoadStarted(object sender, EventArgs e)
        {
            _timer.CurrentState.IsGameTimePaused = true;
        }

        void gameMemory_OnLoadFinished(object sender, Level previousLevel, Level currentLevel, float playerPosX)
        {
            _timer.CurrentState.IsGameTimePaused = false;

            if (_pendingSpeedup != null)
            {
                TriggerSpeedup(_pendingSpeedup);
                _pendingSpeedup = null;
            }

            LoadSpeedup speedup = _loadSpeedups.Find(cs => cs.Matches(previousLevel, currentLevel));
            if (speedup != null)
            {
                Debug.WriteLine($"Found load speedup for level {speedup.PreviousLevel} -> {speedup.NextLevel}, setting {speedup.Setting}");
                TriggerSpeedup(speedup.Setting);
            }
        }

        void gameMemory_OnPlayerLostControl(object sender, EventArgs e)
        {
            if (Settings.AutoStartEnd)
                _timer.Split();
        }

        void gameMemory_OnCutsceneStarted(object sender, Level level, float playerPosX, bool isLoading)
        {
            CutsceneSpeedup speedup = _cutsceneSpeedups.Find(cs => cs.Level == level && cs.ApproxEqual(playerPosX));
            if (speedup != null)
            {
                Debug.WriteLine($"Found cutscene speedup for level {speedup.Level}, pos {speedup.PlayerPosX}, setting {speedup.Setting}");
                if (isLoading)
                {
                    _pendingSpeedup = speedup.Setting;
                } else
                {
                    TriggerSpeedup(speedup.Setting);
                }
            }
        }

        void gameMemory_OnAreaCompleted(object sender, AreaCompletionType type)
        {
            if ((type == AreaCompletionType.IntroEnd && Settings.AutoSplitIntroEnd)
                || (type == AreaCompletionType.MissionEnd && Settings.AutoSplitMissionEnd)
                || (type == AreaCompletionType.PrisonEscape && Settings.AutoSplitPrisonEscape)
                || (type == AreaCompletionType.OutsidersDream && Settings.AutoSplitOutsidersDream)
                || (type == AreaCompletionType.Weepers && Settings.AutoSplitWeepers))
            {
                _timer.Split();
            }
        }

        void TriggerSpeedup(string setting)
        {
            if (!Settings.CutsceneSpeedup || _cutsceneTimer.Enabled)
                return;
            
            SendKeys.Send("{F7}");
            _cutsceneTimer.Interval = GetDuration(setting);
            _cutsceneTimer.Start();
        }

        int GetDuration(string setting)
        {
            var thing = typeof(DishonoredSettings).GetProperty(setting).GetValue(Settings, null);
            Debug.WriteLine($"setting {setting} gave value {thing}");
            return (int)thing;
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return Settings;
        }

        public override void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
        }
    }

    public class TimedTraceListener : DefaultTraceListener
    {
        private static TimedTraceListener _instance;
        public static TimedTraceListener Instance => _instance ?? (_instance = new TimedTraceListener());

        private TimedTraceListener() { }

        public int UpdateCount
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public override void WriteLine(string message)
        {
            base.WriteLine("Dishonored: " + UpdateCount + " " + message);
        }
    }
}
