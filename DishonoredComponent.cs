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
    class Speedup
    {
        public int Duration { get; set; }
        public int Delay { get; set; } = 0;
        public float X { get; set; } = 0f;
        public float Y { get; set; } = 0f;
        public float Z { get; set; } = 0f;
        public float Tolerance { get; set; } = 1f;
        public Speedup Followup { get; set; }

        protected bool ApproxEqual(float value, float expected)
        {
            return expected == 0 || (value >= (expected - Tolerance) && value <= (expected + Tolerance));
        }

        public bool Matches(float x, float y, float z)
        {
            return ApproxEqual(x, X) && ApproxEqual(y, Y) && ApproxEqual(z, Z);
        }
    }

    class MovieSpeedup : Speedup
    {
        public Movie Movie { get; set; }

        public bool Matches(Movie movie, float x, float y, float z)
        {
            return movie == Movie && Matches(x, y, z);
        }
    }

    class CutsceneSpeedup : Speedup
    {
        public Level Level { get; set; }
        public int Count { get; set; }

        public bool Matches(Level level, int count, float x, float y, float z)
        {
            return level == Level && count == Count && Matches(x, y, z);
        }
    }

    class LoadSpeedup : Speedup
    {
        public Level Level { get; set; }
        public Level PreviousLevel { get; set; }

        public bool Matches(Level level, Level previousLevel, float x, float y, float z)
        {
            return level == Level && previousLevel == PreviousLevel && Matches(x, y, z);
        }
    }

    class DishonoredComponent : LogicComponent
    {
        public override string ComponentName => "Dishonored";

        public DishonoredSettings Settings { get; }

        private readonly TimerModel _timer;
        private readonly GameMemory _gameMemory;
        private readonly Timer _updateTimer;
        private readonly Timer _cutsceneTimer;

        private Speedup _pendingSpeedup;
        private Speedup _currentSpeedup;
        private long _elapsedTime = 0;
        private int _timeMultiplier = 1;
        private TimeSpan _lastTime;

        private readonly List<MovieSpeedup> _movieSpeedups;
        private readonly List<CutsceneSpeedup> _cutsceneSpeedups;
        private readonly List<LoadSpeedup> _loadSpeedups;

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
                new MovieSpeedup { Movie = Movie.Intro, Duration = 9200, X = -3908f, Z = -231f },
            };

            _cutsceneSpeedups = new List<CutsceneSpeedup>
            {
                new CutsceneSpeedup { Level = Level.Intro, Count = 2, Duration = 3700, X = 16242.7f, Y = 21620f, Z = 3354.9f, Tolerance = 0.1f },
                new CutsceneSpeedup { Level = Level.Intro, Count = 4, Duration = 6350, X = 15591.5f, Y = 21025.2f, Z = 3425.3f, Tolerance = 0.1f },
            };

            _loadSpeedups = new List<LoadSpeedup>
            {
                new LoadSpeedup { Level = Level.Prison, PreviousLevel = Level.Intro, Duration = 720, Delay = 500 },
                new LoadSpeedup { Level = Level.PubDay, PreviousLevel = Level.Sewers, Duration = 5640, Z = -599f },
                new LoadSpeedup { Level = Level.CampbellStreets, PreviousLevel = Level.PubDusk, Duration = 4250, X = 12809f },
                new LoadSpeedup { Level = Level.PubMorning, PreviousLevel = Level.CampbellBack, Duration = 2050, X = -2374f, Z = -601f },
                new LoadSpeedup { Level = Level.CatStreets, PreviousLevel = Level.PubDay, Duration = 4550, X = 4471f, Z = 1848f },
                new LoadSpeedup { Level = Level.PubDusk, PreviousLevel = Level.CatStreets, Duration = 5050, X = -11185f, Tolerance = 0.5f,
                    Followup = new Speedup { Duration = 1400, Delay = 8000,
                        Followup = new Speedup { Duration = 900, Delay = 1200 } } },
                new LoadSpeedup { Level = Level.Bridge1, PreviousLevel = Level.PubDusk, Duration = 4040, X = -12312f, Z = -583f },
                new LoadSpeedup { Level = Level.PubNight, PreviousLevel = Level.Bridge4, Duration = 4000, X = -11186f, Z = -584.3f },
                new LoadSpeedup { Level = Level.BoyleExterior, PreviousLevel = Level.PubDay, Duration = 2500, X = -9340f, Z = -1951.4f },
                new LoadSpeedup { Level = Level.PubMorning, PreviousLevel = Level.BoyleExterior, Duration = 3130, Y = -9403f, Tolerance = 4f },
                new LoadSpeedup { Level = Level.TowerReturnYard, PreviousLevel = Level.PubMorning, Duration = 5250, X = -8714f, Y = 33057f },
                new LoadSpeedup { Level = Level.PubDusk, PreviousLevel = Level.TowerReturnYard, Duration = 3340, Y = -10593f, Z = -584f },
                new LoadSpeedup { Level = Level.FloodedIntro, PreviousLevel = Level.PubDusk, Duration = 5000, X = -23249f, Tolerance = 0.5f,
                    Followup = new Speedup { Duration = 6850, Delay = 7850 } },
                new LoadSpeedup { Level = Level.FloodedStreets, PreviousLevel = Level.FloodedIntro, Duration = 1600, Delay = 1500 },
                new LoadSpeedup { Level = Level.FloodedStreets, PreviousLevel = Level.FloodedRefinery, Duration = 1600, Delay = 1500, X = -5397.104f },
                new LoadSpeedup { Level = Level.KingsparrowIsland, PreviousLevel = Level.Loyalists, Duration = 4550, Z = 1043f },
                new LoadSpeedup { Level = Level.KingsparrowLighthouse, PreviousLevel = Level.KingsparrowIsland, Duration = 900, Z = 1060f, Tolerance = 10f },
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
                    // when speeding up the game, also speed up IGT in the timer
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

            if (!Settings.CutsceneSpeedup || _cutsceneTimer.Enabled)
                return;

            var speedup = _loadSpeedups.Find(cs => cs.Matches(level, previousLevel, x, y, z));
            if (speedup != null)
            {
                Debug.WriteLine($"Found load speedup level={previousLevel}->{level} x={x} y={y} z={z}");
                DelaySpeedup(speedup);
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
            if (!Settings.CutsceneSpeedup || _cutsceneTimer.Enabled)
                return;

            var speedup = _movieSpeedups.Find(ps => ps.Matches(movie, x, y, z));
            if (speedup != null)
            {
                Debug.WriteLine($"Found movie position speedup movie={movie} x={x} y={y} z={z}");
                DelaySpeedup(speedup);
            }
        }

        void gameMemory_OnPostCutscenePlayerPositionChanged(object sender, Level level, int count, float x, float y, float z)
        {
            if (!Settings.CutsceneSpeedup || _cutsceneTimer.Enabled)
                return;

            var speedup = _cutsceneSpeedups.Find(ps => ps.Matches(level, count, x, y, z));
            if (speedup != null)
            {
                Debug.WriteLine($"Found cutscene position speedup level={level} x={x} y={y} z={z}");
                DelaySpeedup(speedup);
            }
        }

        void gameMemory_OnPostLoadPlayerPositionChanged(object sender, Level level, Level previousLevel, float x, float y, float z)
        {
            if (!Settings.CutsceneSpeedup || _cutsceneTimer.Enabled)
                return;

            var speedup = _loadSpeedups.Find(ps => ps.Matches(level, previousLevel, x, y, z));
            if (speedup != null)
            {
                Debug.WriteLine($"Found load position speedup level={previousLevel}->{level} x={x} y={y} z={z}");
                DelaySpeedup(speedup);
            }
        }

        void DelaySpeedup(Speedup speedup)
        {
            if (!Settings.CutsceneSpeedup || _cutsceneTimer.Enabled)
                return;

            if (speedup.Delay <= 0)
            {
                TriggerSpeedup(speedup);
                return;
            }

            Debug.WriteLine($"Delaying speedup for {speedup.Delay}ms");
            _pendingSpeedup = speedup;
            _cutsceneTimer.Interval = speedup.Delay;
            _cutsceneTimer.Start();
        }

        void TriggerSpeedup(Speedup speedup)
        {
            _pendingSpeedup = null;

            if (!Settings.CutsceneSpeedup || _cutsceneTimer.Enabled || speedup.Duration <= 0)
                return;

            Debug.WriteLine($"Triggering speedup for {speedup.Duration}ms");
            _timeMultiplier = 10;
            _gameMemory.SetWorldSpeed(_timeMultiplier);
            _cutsceneTimer.Interval = speedup.Duration;
            _cutsceneTimer.Start();
            _currentSpeedup = speedup;
        }

        void EndSpeedup(bool stopAll = false)
        {
            Debug.WriteLine("Ending active speedup, if any");
            _gameMemory.SetWorldSpeed(1f);
            _timeMultiplier = 1;

            if (!stopAll && _currentSpeedup != null && _currentSpeedup.Followup != null)
                DelaySpeedup(_currentSpeedup.Followup);

            _currentSpeedup = null;
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
