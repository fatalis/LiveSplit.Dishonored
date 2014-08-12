using System.Reflection;
using LiveSplit.UI.Components;
using System;
using LiveSplit.Model;

namespace LiveSplit.Dishonored
{
    public class DishonoredFactory : IComponentFactory
    {
        public string ComponentName
        {
            get { return "Dishonored"; }
        }

        public string Description
        {
            get { return "Automates splitting and load removal for Dishonored."; }
        }

        public ComponentCategory Category
        {
            get {  return ComponentCategory.Control; }
        }

        public IComponent Create(LiveSplitState state)
        {
            return new DishonoredComponent(state);
        }

        public string UpdateName
        {
            get { return this.ComponentName; }
        }

        public string UpdateURL
        {
            get { return "http://fatalis.hive.ai/livesplit/update/"; }
        }

        public Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public string XMLURL
        {
            get { return this.UpdateURL + "Components/update.LiveSplit.Dishonored.xml"; }
        }
    }
}
