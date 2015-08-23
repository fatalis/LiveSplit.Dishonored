using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.UI;
using System;
using System.Xml;
using System.Windows.Forms;

namespace LiveSplit.Dishonored
{
    class DishonoredComponent : LogicComponent
    {
        public override string ComponentName => "Dishonored";

        public DishonoredSettings Settings { get; set; }

        private TimerModel _timer;
        private GameMemory _gameMemory;

        public DishonoredComponent(LiveSplitState state)
        {
            this.Settings = new DishonoredSettings();

            _timer = new TimerModel { CurrentState = state };
            _timer.CurrentState.OnStart += timer_OnStart;

            _gameMemory = new GameMemory();
            _gameMemory.OnFirstLevelLoading += gameMemory_OnFirstLevelLoading;
            _gameMemory.OnPlayerGainedControl += gameMemory_OnPlayerGainedControl;
            _gameMemory.OnLoadStarted += gameMemory_OnLoadStarted;
            _gameMemory.OnLoadFinished += gameMemory_OnLoadFinished;
            _gameMemory.OnPlayerLostControl += gameMemory_OnPlayerLostControl;
            _gameMemory.OnAreaCompleted += gameMemory_OnAreaCompleted;
            _gameMemory.StartMonitoring();
        }

        public override void Dispose()
        {
            _timer.CurrentState.OnStart -= timer_OnStart;
            _gameMemory?.Stop();
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
            if (this.Settings.AutoStartEnd)
                _timer.Reset();
        }

        void gameMemory_OnPlayerGainedControl(object sender, EventArgs e)
        {
            if (this.Settings.AutoStartEnd)
                _timer.Start();
        }

        void gameMemory_OnLoadStarted(object sender, EventArgs e)
        {
            _timer.CurrentState.IsGameTimePaused = true;
        }

        void gameMemory_OnLoadFinished(object sender, EventArgs e)
        {
            _timer.CurrentState.IsGameTimePaused = false;
        }

        void gameMemory_OnPlayerLostControl(object sender, EventArgs e)
        {
            if (this.Settings.AutoStartEnd)
                _timer.Split();
        }

        void gameMemory_OnAreaCompleted(object sender, GameMemory.AreaCompletionType type)
        {
            if ((type == GameMemory.AreaCompletionType.IntroEnd && this.Settings.AutoSplitIntroEnd)
                || (type == GameMemory.AreaCompletionType.MissionEnd && this.Settings.AutoSplitMissionEnd)
                || (type == GameMemory.AreaCompletionType.PrisonEscape && this.Settings.AutoSplitPrisonEscape)
                || (type == GameMemory.AreaCompletionType.OutsidersDream && this.Settings.AutoSplitOutsidersDream)
                || (type == GameMemory.AreaCompletionType.Weepers && this.Settings.AutoSplitWeepers))
            {
                _timer.Split();
            }
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return this.Settings.GetSettings(document);
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return this.Settings;
        }

        public override void SetSettings(XmlNode settings)
        {
            this.Settings.SetSettings(settings);
        }
    }
}
