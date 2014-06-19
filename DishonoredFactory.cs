using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using LiveSplit.UI.Components;
using System;
using LiveSplit.Model;

namespace LiveSplit.Dishonored
{
    public class DishonoredFactory : IComponentFactory
    {
        private DishonoredComponent _instance;

        public string ComponentName
        {
            get { return "Dishonored"; }
        }

        public IComponent Create(LiveSplitState state)
        {
            if (Environment.Is64BitProcess)
            {
                MessageBox.Show("LiveSplit is running as 64-bit. You need to run LiveSplit32BitPatcher.exe at least once.", "LiveSplit.Dishonored",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception("64-bit is unsupported");
            }

            // TODO: in LiveSplit 1.4, components will be IDisposable
            // this assumes the passed state is always the same one, until then
            return _instance ?? (_instance = new DishonoredComponent(state));

            // return new DishonoredComponent(state);
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
