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
    abstract class PositionSpeedup
    {
        public Func<string, float> SettingsGetter { get; set; }
        public string Setting { get; set; }
        public Level Level { get; set; }
        public float Tolerance = 0.5f;

        protected float _setX { get => SettingsGetter($"X{Setting}"); }
        protected float _setY { get => SettingsGetter($"Y{Setting}"); }
        protected float _setZ { get => SettingsGetter($"Z{Setting}"); }
        protected bool _coordsSet { get => _setX != 0 || _setY != 0 || _setZ != 0; }

        protected bool ApproxEqual(float value, float expected)
        {
            return expected == 0 || (value >= (expected - Tolerance) && value <= (expected + Tolerance));
        }
    }

    class MovieSpeedup : PositionSpeedup
    {
        public Movie Movie { get; set; }

        public bool Matches(Movie movie, float x, float y, float z)
        {
            if (!_coordsSet)
            {
                return false;
            }

            return movie == Movie && ApproxEqual(x, _setX) && ApproxEqual(y, _setY) && ApproxEqual(z, _setZ);
        }
    }

    class CutsceneSpeedup : PositionSpeedup
    {
        public int Count { get; set; }

        public bool Matches(Level level, int count, float x, float y, float z)
        {
            if (!_coordsSet)
            {
                return false;
            }

            return level == Level && count == Count && ApproxEqual(x, _setX) && ApproxEqual(y, _setY) && ApproxEqual(z, _setZ);
        }
    }

    class LoadDelaySpeedup : PositionSpeedup
    {
        public Level PreviousLevel { get; set; }
        public float PlayerPosX { get; set; }
        public float PlayerPosY { get; set; }
        public float PlayerPosZ { get; set; }

        public bool Matches(Level level, Level previousLevel, float x, float y, float z)
        {
            if (_coordsSet)
            {
                return false;
            }

            return level == Level && previousLevel == PreviousLevel && ApproxEqual(x, PlayerPosX) && ApproxEqual(y, PlayerPosY) && ApproxEqual(z, PlayerPosZ);
        }
    }

    class LoadPositionSpeedup : PositionSpeedup
    {
        public Level PreviousLevel { get; set; }

        public bool Matches(Level level, Level previousLevel, float x, float y, float z)
        {
            if (!_coordsSet)
            {
                return false;
            }

            return level == Level && previousLevel == PreviousLevel && ApproxEqual(x, _setX) && ApproxEqual(y, _setY) && ApproxEqual(z, _setZ);
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

        private List<MovieSpeedup> _movieSpeedups;
        private List<CutsceneSpeedup> _cutsceneSpeedups;
        private List<LoadDelaySpeedup> _loadDelaySpeedups;
        private List<LoadPositionSpeedup> _loadPositionSpeedups;

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

            _movieSpeedups = new List<MovieSpeedup>
            {
                new MovieSpeedup { SettingsGetter = GetSettingsFloat, Setting = "Intro", Movie = Movie.Intro },
            };

            _cutsceneSpeedups = new List<CutsceneSpeedup>
            {
                new CutsceneSpeedup { SettingsGetter = GetSettingsFloat, Setting = "BlindIntro", Level = Level.Intro, Count = 2, Tolerance = 0.1f },
                new CutsceneSpeedup { SettingsGetter = GetSettingsFloat, Setting = "IntroEnd", Level = Level.Intro, Count = 4, Tolerance = 0.1f },
            };

            _loadDelaySpeedups = new List<LoadDelaySpeedup>
            {
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "Prison", Level = Level.Prison, PreviousLevel = Level.Intro },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "PostSewers", Level = Level.PubDay, PreviousLevel = Level.Sewers },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "Campbell", Level = Level.CampbellStreets, PreviousLevel = Level.PubDusk },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "PostCampbell", Level = Level.PubMorning, PreviousLevel = Level.CampbellBack },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "Cat", Level = Level.CatStreets, PreviousLevel = Level.PubDay },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "PostCat", Level = Level.PubDusk, PreviousLevel = Level.CatStreets },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "Bridge", Level = Level.Bridge1, PreviousLevel = Level.PubDusk },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "PostBridge", Level = Level.PubNight, PreviousLevel = Level.Bridge4 },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "Boyle", Level = Level.BoyleExterior, PreviousLevel = Level.PubDay },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "PostBoyle", Level = Level.PubMorning, PreviousLevel = Level.BoyleExterior },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "Tower", Level = Level.TowerReturnYard, PreviousLevel = Level.PubMorning },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "PostTower", Level = Level.PubDusk, PreviousLevel = Level.TowerReturnYard },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "Flooded", Level = Level.FloodedIntro, PreviousLevel = Level.PubDusk },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "FloodedCell", Level = Level.FloodedStreets, PreviousLevel = Level.FloodedIntro },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "FloodedCell", Level = Level.FloodedStreets, PreviousLevel = Level.FloodedRefinery, PlayerPosX = -5397.104f },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "Kingsparrow", Level = Level.KingsparrowIsland, PreviousLevel = Level.Loyalists },
                new LoadDelaySpeedup { SettingsGetter = GetSettingsFloat, Setting = "Lighthouse", Level = Level.KingsparrowLighthouse, PreviousLevel = Level.KingsparrowIsland },
            };

            _loadPositionSpeedups = new List<LoadPositionSpeedup>
            {
                new LoadPositionSpeedup { SettingsGetter = GetSettingsFloat, Setting = "PostSewers", Level = Level.PubDay, PreviousLevel = Level.Sewers },
                new LoadPositionSpeedup { SettingsGetter = GetSettingsFloat, Setting = "Campbell", Level = Level.CampbellStreets, PreviousLevel = Level.PubDusk },
                new LoadPositionSpeedup { SettingsGetter = GetSettingsFloat, Setting = "PostCampbell", Level = Level.PubMorning, PreviousLevel = Level.CampbellBack },
                new LoadPositionSpeedup { SettingsGetter = GetSettingsFloat, Setting = "Cat", Level = Level.CatStreets, PreviousLevel = Level.PubDay },
                new LoadPositionSpeedup { SettingsGetter = GetSettingsFloat, Setting = "PostCat", Level = Level.PubDusk, PreviousLevel = Level.CatStreets },
                new LoadPositionSpeedup { SettingsGetter = GetSettingsFloat, Setting = "Bridge", Level = Level.Bridge1, PreviousLevel = Level.PubDusk },
                new LoadPositionSpeedup { SettingsGetter = GetSettingsFloat, Setting = "PostBridge", Level = Level.PubNight, PreviousLevel = Level.Bridge4 },
                new LoadPositionSpeedup { SettingsGetter = GetSettingsFloat, Setting = "Boyle", Level = Level.BoyleExterior, PreviousLevel = Level.PubDay },
                new LoadPositionSpeedup { SettingsGetter = GetSettingsFloat, Setting = "PostBoyle", Level = Level.PubMorning, PreviousLevel = Level.BoyleExterior, Tolerance = 4f },
                new LoadPositionSpeedup { SettingsGetter = GetSettingsFloat, Setting = "Tower", Level = Level.TowerReturnYard, PreviousLevel = Level.PubMorning },
                new LoadPositionSpeedup { SettingsGetter = GetSettingsFloat, Setting = "PostTower", Level = Level.PubDusk, PreviousLevel = Level.TowerReturnYard },
                new LoadPositionSpeedup { SettingsGetter = GetSettingsFloat, Setting = "Flooded", Level = Level.FloodedIntro, PreviousLevel = Level.PubDusk },
                new LoadPositionSpeedup { SettingsGetter = GetSettingsFloat, Setting = "Kingsparrow", Level = Level.KingsparrowIsland, PreviousLevel = Level.Loyalists },
            };

            _cutsceneTimer = new Timer() { Enabled = false };
            _cutsceneTimer.Tick += cutsceneTimer_Tick;

            _gameMemory = new GameMemory();
            _gameMemory.OnFirstLevelLoading += gameMemory_OnFirstLevelLoading;
            _gameMemory.OnPlayerGainedControl += gameMemory_OnPlayerGainedControl;
            _gameMemory.OnLoadStarted += gameMemory_OnLoadStarted;
            _gameMemory.OnLoadFinished += gameMemory_OnLoadFinished;
            _gameMemory.OnPlayerLostControl += gameMemory_OnPlayerLostControl;
            _gameMemory.OnAreaCompleted += gameMemory_OnAreaCompleted;
            _gameMemory.OnPostMoviePlayerPositionChanged += gameMemory_OnPostMoviePlayerPositionChanged;
            _gameMemory.OnPostCutscenePlayerPositionChanged += gameMemory_OnPostCutscenePlayerPositionChanged;
            _gameMemory.OnPostLoadPlayerPositionChanged += gameMemory_OnPostLoadPlayerPositionChanged;
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
                _gameMemory.Update(Settings.LogCoords);
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

        void gameMemory_OnLoadFinished(object sender, Level level, Level previousLevel, Movie movie, float x, float y, float z)
        {
            _timer.CurrentState.IsGameTimePaused = false;

            var speedup = _loadDelaySpeedups.Find(cs => cs.Matches(level, previousLevel, x, y, z));
            if (speedup != null)
            {
                Debug.WriteLine($"Found load delay speedup {speedup.Setting} level={previousLevel}->{level}");
                DelaySpeedup(speedup.Setting);
            }
        }

        void gameMemory_OnPlayerLostControl(object sender, EventArgs e)
        {
            if (Settings.AutoStartEnd)
                _timer.Split();
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

        void gameMemory_OnPostMoviePlayerPositionChanged(object sender, Movie movie, float x, float y, float z)
        {
            if (_cutsceneTimer.Enabled)
            {
                return;
            }

            var speedup = _movieSpeedups.Find(ps => ps.Matches(movie, x, y, z));
            if (speedup != null)
            {
                Debug.WriteLine($"Found movie position speedup {speedup.Setting} movie={movie} x={x} y={y} z={z}");
                TriggerSpeedup(speedup.Setting);
            }
        }

        void gameMemory_OnPostCutscenePlayerPositionChanged(object sender, Level level, int count, float x, float y, float z)
        {
            if (_cutsceneTimer.Enabled)
            {
                return;
            }

            var speedup = _cutsceneSpeedups.Find(ps => ps.Matches(level, count, x, y, z));
            if (speedup != null)
            {
                Debug.WriteLine($"Found cutscene position speedup {speedup.Setting} level={level} x={x} y={y} z={z}");
                TriggerSpeedup(speedup.Setting);
            }
        }

        void gameMemory_OnPostLoadPlayerPositionChanged(object sender, Level level, Level previousLevel, float x, float y, float z)
        {
            if (_cutsceneTimer.Enabled)
            {
                return;
            }

            var speedup = _loadPositionSpeedups.Find(ps => ps.Matches(level, previousLevel, x, y, z));
            if (speedup != null)
            {
                Debug.WriteLine($"Found load position speedup {speedup.Setting} level={previousLevel}->{level} x={x} y={y} z={z}");
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
            var property = typeof(DishonoredSettings).GetProperty(setting);
            if (property == null)
            {
                return null;
            }
            return property.GetValue(Settings);
        }

        bool GetSettingsBool(string setting)
        {
            return (bool)GetSettingsValue(setting);
        }

        int GetSettingsInt(string setting)
        {
            var value = GetSettingsValue(setting);
            if (value == null)
            {
                return 0;
            }
            return (int)value;
        }

        float GetSettingsFloat(string setting)
        {
            var value = GetSettingsValue(setting);
            if (value == null)
            {
                return 0f;
            }
            return (float)value;
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
