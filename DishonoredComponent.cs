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
        public float? PlayerPosX { get; set; }
        public string Setting { get; set; }

        private const float _tolerance = 3f;

        public bool ApproxEqual(float val)
        {
            return val > (PlayerPosX - _tolerance) && val < (PlayerPosX + _tolerance);
        }

        public bool Matches(Level previousLevel, Level nextLevel, float playerPosX)
        {
            return previousLevel == PreviousLevel && nextLevel == NextLevel && (PlayerPosX == null || ApproxEqual(playerPosX));
        }
    }

    class MovieSpeedup
    {
        public Movie Movie { get; set; }
        public string Setting { get; set; }

        public bool Matches(Movie movie)
        {
            return movie == Movie;
        }
    }

    class PositionSpeedup
    {
        public Level Level { get; set; }
        public string Setting { get; set; }
        public float Tolerance = 0.5f;

        private bool ApproxEqual(float value, float expected)
        {
            return expected == 0 || (value > (expected - Tolerance) && value < (expected + Tolerance));
        }

        public bool Matches(Level level, float x, float y, float z, Func<string, float> settingsGetter)
        {
            if (level != Level)
            {
                return false;
            }

            return ApproxEqual(x, settingsGetter($"X{Setting}")) && ApproxEqual(y, settingsGetter($"Y{Setting}")) && ApproxEqual(z, settingsGetter($"Z{Setting}"));
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
        private long _elapsedTime = 0;
        private int _timeMultiplier = 1;
        private TimeSpan _lastTime;
        private string _currentSpeedup = null;

        private List<CutsceneSpeedup> _cutsceneSpeedups = new List<CutsceneSpeedup>
        {
            new CutsceneSpeedup { Level = Level.Intro, PlayerPosX = 15476f, Setting = "IntroEnd" },
            new CutsceneSpeedup { Level = Level.Intro, PlayerPosX = 15482f, Setting = "IntroEnd" },
        };

        private List<LoadSpeedup> _loadSpeedups = new List<LoadSpeedup>
        {
            new LoadSpeedup { PreviousLevel = Level.Intro, NextLevel = Level.Prison, Setting = "Prison" },
            new LoadSpeedup { PreviousLevel = Level.Sewers, NextLevel = Level.PubDay, Setting = "PostSewers" },
            //new LoadSpeedup { PreviousLevel = Level.PubDusk, NextLevel = Level.CampbellStreets, Setting = "Campbell" },
            new LoadSpeedup { PreviousLevel = Level.CampbellBack, NextLevel = Level.PubMorning, Setting = "PostCampbell" },
            new LoadSpeedup { PreviousLevel = Level.PubDay, NextLevel = Level.CatStreets, Setting = "Cat" },
            //new LoadSpeedup { PreviousLevel = Level.CatStreets, NextLevel = Level.PubDusk, Setting = "PostCat" },
            new LoadSpeedup { PreviousLevel = Level.PubDusk, NextLevel = Level.Bridge1, Setting = "Bridge" },
            new LoadSpeedup { PreviousLevel = Level.Bridge4, NextLevel = Level.PubNight, Setting = "PostBridge" },
            new LoadSpeedup { PreviousLevel = Level.PubDay, NextLevel = Level.BoyleExterior, Setting = "Boyle" },
            new LoadSpeedup { PreviousLevel = Level.BoyleExterior, NextLevel = Level.PubMorning, Setting = "PostBoyle" },
            new LoadSpeedup { PreviousLevel = Level.PubMorning, NextLevel = Level.TowerReturnYard, Setting = "Tower" },
            new LoadSpeedup { PreviousLevel = Level.TowerReturnYard, NextLevel = Level.PubDusk, Setting = "PostTower" },
            //new LoadSpeedup { PreviousLevel = Level.PubDusk, NextLevel = Level.FloodedIntro, Setting = "Flooded" },
            new LoadSpeedup { PreviousLevel = Level.FloodedIntro, NextLevel = Level.FloodedStreets, Setting = "FloodedCell" },
            new LoadSpeedup { PreviousLevel = Level.FloodedRefinery, NextLevel = Level.FloodedStreets, PlayerPosX = -5397.104f, Setting = "FloodedCell" },
            new LoadSpeedup { PreviousLevel = Level.Loyalists, NextLevel = Level.KingsparrowIsland, Setting = "Kingsparrow" },
            new LoadSpeedup { PreviousLevel = Level.KingsparrowIsland, NextLevel = Level.KingsparrowLighthouse, Setting = "Lighthouse" },
        };

        private List<MovieSpeedup> _movieSpeedups = new List<MovieSpeedup>
        {
            new MovieSpeedup { Movie = Movie.Intro, Setting = "Intro" },
        };

        private List<PositionSpeedup> _positionSpeedups = new List<PositionSpeedup>
        {
            new PositionSpeedup { Level = Level.CampbellStreets, Setting = "Campbell" },
            new PositionSpeedup { Level = Level.PubDusk, Setting = "PostCat" },
            new PositionSpeedup { Level = Level.FloodedIntro, Setting = "Flooded" },
        };

        private Dictionary<string, string> _speedupFollowups = new Dictionary<string, string>
        {
            ["PostCat"] = "PostCat2",
            ["PostCat2"] = "PostCat3",
            ["Flooded"] = "Flooded2",
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
            _gameMemory.OnMovieEnded += gameMemory_OnMovieEnded;
            _gameMemory.OnPlayerPositionChanged += gameMemory_OnPlayerPositionChanged;
        }

        public override void Dispose()
        {
            _timer.CurrentState.OnStart -= timer_OnStart;
            _updateTimer?.Dispose();
            _cutsceneTimer?.Dispose();
            _gameMemory?.Dispose();
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

            var now = _timer.CurrentState.CurrentTime.RealTime;
            if (now != null)
            {
                if (!_timer.CurrentState.IsGameTimePaused && Settings.CutsceneSpeedup)
                {
                    _elapsedTime += ((TimeSpan)now - _lastTime).Ticks * _timeMultiplier;
                    _timer.CurrentState.SetGameTime(new TimeSpan(_elapsedTime));
                }
                _lastTime = (TimeSpan)now;
            }
        }

        void cutsceneTimer_Tick(object sender, EventArgs eventArgs)
        {
            _cutsceneTimer.Stop();

            if (_pendingSpeedup != null)
            {
                TriggerSpeedup(_pendingSpeedup);
            }
            else
            {
                EndSpeedup();
            }
        }

        void timer_OnStart(object sender, EventArgs e)
        {
            _timer.InitializeGameTime();
            _elapsedTime = 0;
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

            // must be a manual load, just let the player deal with it
            if (_cutsceneTimer.Enabled)
            {
                EndSpeedup(true);
                _cutsceneTimer.Stop();
                _pendingSpeedup = null;
            }
        }

        void gameMemory_OnLoadFinished(object sender, Level previousLevel, Level currentLevel, Movie movie, float playerPosX)
        {
            _timer.CurrentState.IsGameTimePaused = false;

            var speedup = _loadSpeedups.Find(cs => cs.Matches(previousLevel, currentLevel, playerPosX));
            if (speedup != null)
            {
                Debug.WriteLine($"Found load speedup for level {speedup.PreviousLevel} -> {speedup.NextLevel}, setting {speedup.Setting}");
                DelaySpeedup(speedup.Setting);
            }
        }

        void gameMemory_OnPlayerLostControl(object sender, EventArgs e)
        {
            if (Settings.AutoStartEnd)
                _timer.Split();
        }

        void gameMemory_OnCutsceneStarted(object sender, Level level, float playerPosX, bool isLoading)
        {
            var speedup = _cutsceneSpeedups.Find(cs => cs.Matches(level, playerPosX));
            if (speedup != null)
            {
                Debug.WriteLine($"Found cutscene speedup for level {speedup.Level}, pos {speedup.PlayerPosX}, setting {speedup.Setting}");
                DelaySpeedup(speedup.Setting);
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

        void gameMemory_OnMovieEnded(object sender, Movie movie)
        {
            var speedup = _movieSpeedups.Find(ms => ms.Matches(movie));
            if (speedup != null)
            {
                DelaySpeedup(speedup.Setting);
            }
        }

        void gameMemory_OnPlayerPositionChanged(object sender, Level level, float x, float y, float z)
        {
            if (_cutsceneTimer.Enabled)
            {
                return;
            }

            var speedup = _positionSpeedups.Find(ps => ps.Matches(level, x, y, z, GetSettingsFloat));
            if (speedup != null)
            {
                Debug.WriteLine($"Found position speedup for {speedup.Setting} level={level} x={x} y={y} z={z}");
                TriggerSpeedup(speedup.Setting);
            }
        }

        void DelaySpeedup(string setting)
        {
            var enabled = GetSettingsBool($"Enabled{setting}");
            if (!enabled)
            {
                return;
            }

            var delay = GetSettingsInt($"Delay{setting}");
            if (delay <= 0)
            {
                TriggerSpeedup(setting);
                return;
            }

            Debug.WriteLine($"Delaying speedup {setting} for {delay}ms");
            _pendingSpeedup = setting;
            _cutsceneTimer.Interval = delay;
            _cutsceneTimer.Start();
        }

        void TriggerSpeedup(string setting)
        {
            _pendingSpeedup = null;
            var enabled = GetSettingsBool($"Enabled{setting}");
            var duration = GetSettingsInt($"Speedup{setting}");

            if (!Settings.CutsceneSpeedup || _cutsceneTimer.Enabled || !enabled || duration <= 0)
                return;

            Debug.WriteLine($"Triggering speedup {setting}");
            _gameMemory.SetWorldSpeed(10f);
            _timeMultiplier = 10;
            _cutsceneTimer.Interval = duration;
            _cutsceneTimer.Start();
            _currentSpeedup = setting;
        }

        void EndSpeedup(bool stopAll = false)
        {
            Debug.WriteLine("Ending active speedup, if any");
            _gameMemory.SetWorldSpeed(1f);
            _timeMultiplier = 1;

            if (!stopAll && _currentSpeedup != null && _speedupFollowups.ContainsKey(_currentSpeedup))
            {
                DelaySpeedup(_speedupFollowups[_currentSpeedup]);
            }

            _currentSpeedup = null;
        }

        object GetSettingsValue(string setting)
        {
            return typeof(DishonoredSettings).GetProperty(setting).GetValue(Settings, null);
        }

        bool GetSettingsBool(string setting)
        {
            return (bool)GetSettingsValue(setting);
        }

        int GetSettingsInt(string setting)
        {
            return (int)GetSettingsValue(setting);
        }

        float GetSettingsFloat(string setting)
        {
            return (float)GetSettingsValue(setting);
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
