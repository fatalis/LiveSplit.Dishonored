using System.Reflection;
using LiveSplit.Dishonored;
using LiveSplit.UI.Components;
using System;
using LiveSplit.Model;

[assembly: ComponentFactory(typeof(DishonoredFactory))]

namespace LiveSplit.Dishonored
{
    public class DishonoredFactory : IComponentFactory
    {
        public string ComponentName => "Dishonored";
        public string Description => "Automates splitting and load removal for Dishonored.";
        public ComponentCategory Category => ComponentCategory.Control;

        public IComponent Create(LiveSplitState state)
        {
            return new DishonoredComponent(state);
        }

        public string UpdateName => this.ComponentName;
        public string UpdateURL => "https://raw.githubusercontent.com/fatalis/LiveSplit.Dishonored/master/";
        public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        public string XMLURL => this.UpdateURL + "Release/update.LiveSplit.Dishonored.xml";
    }
}
