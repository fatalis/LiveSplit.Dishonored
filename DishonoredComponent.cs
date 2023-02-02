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
using System.Numerics;

namespace LiveSplit.Dishonored
{
    class Speedup
    {
        public int Duration { get; set; }
        public int Delay { get; set; } = 0;
        public float X { get; set; } = float.NaN;
        public float Y { get; set; } = float.NaN;
        public float Z { get; set; } = float.NaN;
        public float Tolerance { get; set; } = 1.0f;
        public Speedup Followup { get; set; }

        protected bool ApproxEqual(float value, float expected)
        {
            return float.IsNaN(expected) || (Math.Abs(value - expected) <= Tolerance);
        }

        public bool Matches(Vector3 pos)
        {
            return ApproxEqual(pos.X, X) && ApproxEqual(pos.Y, Y) && ApproxEqual(pos.Z, Z);
        }
    }

    class MovieSpeedup : Speedup
    {
        public Movie Movie { get; set; }

        public bool Matches(Movie movie, Vector3 pos)
        {
            return movie == Movie && Matches(pos);
        }
    }

    class CutsceneSpeedup : Speedup
    {
        public Level Level { get; set; }
        public int Count { get; set; } = 0;

        private int _count = 0;

        public bool Matches(Level level, Vector3 pos)
        {
            if (level == Level && Matches(pos))
            {
                if (Count == 0)
                {
                    return true;
                }
                else if (_count >= 0)
                {
                    if (++_count == Count)
                    {
                        _count = -1;
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine($"Incrementing cutscene position count level={level} pos={pos} counts=({_count} < {Count})");
                    }
                }
            }

            return false;
        }

        public void OnReset()
        {
            _count = 0;
        }

        public void OnLoad()
        {
            // If a level is loaded and the `currentCount` has been incremented,
            // we could be loading a save before or after the it was increment.
            // There is no way to tell so we just disable the speedup.
            if (_count > 0) {
                _count = -1;
            }
        }
    }

    class LoadSpeedup : Speedup
    {
        public Level Level { get; set; }
        public Level PreviousLevel { get; set; }

        public bool Matches(Level level, Level previousLevel, Vector3 pos)
        {
            return level == Level && previousLevel == PreviousLevel && Matches(pos);
        }
    }

    class DishonoredComponent : LogicComponent
    {
        public override string ComponentName => "Dishonored";

        public DishonoredSettings Settings { get; }

        private readonly TimerModel _timer;
        private readonly GameMemory _gameMemory;
        private readonly Timer _updateTimer;
        private readonly Timer _speedupTimer;

        private Speedup _pendingSpeedup;
        private Speedup _currentSpeedup;
        private long _elapsedTime = 0;
        private int _timeMultiplier = 1;
        private TimeSpan _lastTime;

        private readonly List<MovieSpeedup> _movieSpeedups;
        private readonly List<CutsceneSpeedup> _cutsceneSpeedups;
        private readonly List<LoadSpeedup> _loadPositionSpeedups;
        private readonly List<LoadSpeedup> _loadDelaySpeedups;

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

            // these are triggered after a (non-loading) movie finishes playing
            _movieSpeedups = new List<MovieSpeedup>
            {
                new MovieSpeedup { Movie = Movie.Intro, Duration = 9200, X = -3908f, Z = -231f },
            };

            // these are triggered after an in-game cutscene starts
            _cutsceneSpeedups = new List<CutsceneSpeedup>
            {
                new CutsceneSpeedup { Level = Level.Intro, Duration = 1600, X = 13276.1f, Y = 23073.2f, Z = 2636.3f, Tolerance = 0.1f },
                new CutsceneSpeedup { Level = Level.Intro, Duration = 3700, X = 16242.7f, Y = 21620.0f, Z = 3354.9f, Tolerance = 0.1f },
                new CutsceneSpeedup { Level = Level.Intro, Count = 2, Duration = 6350, X = 15591.5f, Y = 21025.2f, Z = 3425.3f, Tolerance = 0.1f },
            };

            // these are triggered after a delay after a load finishes
            _loadDelaySpeedups = new List<LoadSpeedup>
            {
                new LoadSpeedup { Level = Level.Prison, PreviousLevel = Level.Intro, Duration = 720, Delay = 500 },
                new LoadSpeedup { Level = Level.FloodedStreets, PreviousLevel = Level.FloodedIntro, Duration = 1600, Delay = 1500 },
                new LoadSpeedup { Level = Level.FloodedStreets, PreviousLevel = Level.FloodedRefinery, Duration = 1600, Delay = 1500, X = -5397.104f },
            };

            // these are triggered after hitting coordinates after a load finishes
            _loadPositionSpeedups = new List<LoadSpeedup>
            {
                new LoadSpeedup { Level = Level.PubDay, PreviousLevel = Level.Sewers, Duration = 5640, Y = -7993.3f, Z = -598.1f, Tolerance = 3.0f },
                new LoadSpeedup { Level = Level.CampbellStreets, PreviousLevel = Level.PubDusk, Duration = 4250, X = 12809f },
                new LoadSpeedup { Level = Level.PubMorning, PreviousLevel = Level.CampbellBack, Duration = 2050, X = -2369.2f, Z = -600.6f },
                new LoadSpeedup { Level = Level.CatStreets, PreviousLevel = Level.PubDay, Duration = 4550, X = 4460.5f, Z = 1849.5f },
                new LoadSpeedup { Level = Level.PubDusk, PreviousLevel = Level.CatStreets, Duration = 5050, X = -11185f, Tolerance = 0.5f,
                    Followup = new Speedup { Duration = 1400, Delay = 8000,
                        Followup = new Speedup { Duration = 900, Delay = 12100 } } },
                new LoadSpeedup { Level = Level.Bridge1, PreviousLevel = Level.PubDusk, Duration = 4040, X = -12312f, Z = -583f },
                new LoadSpeedup { Level = Level.PubNight, PreviousLevel = Level.Bridge4, Duration = 4000, X = -11180.5f, Z = -583.3f },
                new LoadSpeedup { Level = Level.BoyleExterior, PreviousLevel = Level.PubDay, Duration = 2500, X = -9340f, Z = -1951.4f },
                new LoadSpeedup { Level = Level.PubMorning, PreviousLevel = Level.BoyleExterior, Duration = 3130, Y = -9403f, Tolerance = 4f },
                new LoadSpeedup { Level = Level.TowerReturnYard, PreviousLevel = Level.PubMorning, Duration = 5250, X = -8714f, Y = 33057f },
                new LoadSpeedup { Level = Level.PubDusk, PreviousLevel = Level.TowerReturnYard, Duration = 3340, Y = -10615f, Z = -583.1f, Tolerance = 2f },
                new LoadSpeedup { Level = Level.FloodedIntro, PreviousLevel = Level.PubDusk, Duration = 5000, X = -23249f, Tolerance = 0.5f,
                    Followup = new Speedup { Duration = 6850, Delay = 7850 } },
                new LoadSpeedup { Level = Level.KingsparrowIsland, PreviousLevel = Level.Loyalists, Duration = 4550, Y = 18200f, Z = 1040.8f, Tolerance = 3.0f },
                new LoadSpeedup { Level = Level.KingsparrowLighthouse, PreviousLevel = Level.KingsparrowIsland, Duration = 900, Z = 1060f, Tolerance = 10f },
            };

            _speedupTimer = new Timer() { Enabled = false };
            _speedupTimer.Tick += speedupTimer_Tick;

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
            _speedupTimer?.Dispose();
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
                if (!_timer.CurrentState.IsGameTimePaused && Settings.EnableSpeedups)
                {
                    // when speeding up the game, also speed up IGT in the timer
                    _elapsedTime += ((TimeSpan)now - _lastTime).Ticks * _timeMultiplier;
                    _timer.CurrentState.SetGameTime(new TimeSpan(_elapsedTime));
                }
                _lastTime = (TimeSpan)now;
            }
        }

        void speedupTimer_Tick(object sender, EventArgs eventArgs)
        {
            _speedupTimer.Stop();

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
            foreach (var speedup in _cutsceneSpeedups)
            {
                speedup.OnReset();
            }
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

            foreach (var speedup in _cutsceneSpeedups)
            {
                speedup.OnLoad();
            }

            // must be a manual load, just let the player deal with it
            if (_speedupTimer.Enabled)
            {
                EndSpeedup(true);
                _speedupTimer.Stop();
                _pendingSpeedup = null;
            }
        }

        void gameMemory_OnLoadFinished(object sender, Level level, Level previousLevel, Movie movie, Vector3 pos)
        {
            _timer.CurrentState.IsGameTimePaused = false;

            if (!Settings.EnableSpeedups || !Settings.SpeedupLoadDelays || _speedupTimer.Enabled)
                return;

            var speedup = _loadDelaySpeedups.Find(cs => cs.Matches(level, previousLevel, pos));
            if (speedup != null)
            {
                Debug.WriteLine($"Found load delay speedup level={previousLevel}->{level} pos={pos}");
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
                || (type == AreaCompletionType.Weepers && Settings.AutoSplitWeepers)
                || (type == AreaCompletionType.DLC06IntroEnd && Settings.AutoSplitDLC06IntroEnd))
            {
                _timer.Split();
            }
        }

        void gameMemory_OnPostMoviePlayerPositionChanged(object sender, Movie movie, Vector3 pos)
        {
            if (!Settings.EnableSpeedups || !Settings.SpeedupMovies || _speedupTimer.Enabled)
                return;

            var speedup = _movieSpeedups.Find(ps => ps.Matches(movie, pos));
            if (speedup != null)
            {
                Debug.WriteLine($"Found movie position speedup movie={movie} pos={pos}");
                DelaySpeedup(speedup);
            }
        }

        void gameMemory_OnPostCutscenePlayerPositionChanged(object sender, Level level, Vector3 pos)
        {
            if (!Settings.EnableSpeedups || !Settings.SpeedupInGameCutscenes || _speedupTimer.Enabled)
                return;

            var speedup = _cutsceneSpeedups.Find(ps => ps.Matches(level, pos));
            if (speedup != null)
            {
                Debug.WriteLine($"Found cutscene position speedup level={level} pos={pos}");
                DelaySpeedup(speedup);
            }
        }

        void gameMemory_OnPostLoadPlayerPositionChanged(object sender, Level level, Level previousLevel, Vector3 pos)
        {
            if (!Settings.EnableSpeedups || !Settings.SpeedupLoadPositions || _speedupTimer.Enabled)
                return;

            var speedup = _loadPositionSpeedups.Find(ps => ps.Matches(level, previousLevel, pos));
            if (speedup != null)
            {
                Debug.WriteLine($"Found load position speedup level={previousLevel}->{level} pos={pos}");
                DelaySpeedup(speedup);
            }
        }

        void DelaySpeedup(Speedup speedup)
        {
            if (!Settings.EnableSpeedups || _speedupTimer.Enabled)
                return;

            if (speedup.Delay <= 0)
            {
                TriggerSpeedup(speedup);
                return;
            }

            Debug.WriteLine($"Delaying speedup for {speedup.Delay}ms");
            _pendingSpeedup = speedup;
            _speedupTimer.Interval = speedup.Delay;
            _speedupTimer.Start();
        }

        void TriggerSpeedup(Speedup speedup)
        {
            _pendingSpeedup = null;

            if (!Settings.EnableSpeedups || _speedupTimer.Enabled || speedup.Duration <= 0)
                return;

            Debug.WriteLine($"Triggering speedup for {speedup.Duration}ms");
            _timeMultiplier = 10;
            _gameMemory.SetWorldSpeed(_timeMultiplier);
            _speedupTimer.Interval = speedup.Duration;
            _speedupTimer.Start();
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
