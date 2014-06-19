using System;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.Dishonored
{
    public partial class DishonoredSettings : UserControl
    {
        public bool DrawWithoutLoads { get; set; }
        public bool AutoStartEnd { get; set; }
        public bool AutoSplitIntroEnd { get; set; }
        public bool AutoSplitMissionEnd { get; set; }
        public bool AutoSplitPrisonEscape { get; set; }
        public bool AutoSplitOutsidersDream { get; set; }
        public bool AutoSplitWeepers { get; set; }

        public DishonoredSettings()
        {
            InitializeComponent();

            this.chkDisplayWithoutLoads.DataBindings.Add("Checked", this, "DrawWithoutLoads", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoStartEnd.DataBindings.Add("Checked", this, "AutoStartEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoSplitIntroEnd.DataBindings.Add("Checked", this, "AutoSplitIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoSplitMissionEnd.DataBindings.Add("Checked", this, "AutoSplitMissionEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoSplitPrisonEscape.DataBindings.Add("Checked", this, "AutoSplitPrisonEscape", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoSplitOutsidersDream.DataBindings.Add("Checked", this, "AutoSplitOutsidersDream", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoSplitWeepers.DataBindings.Add("Checked", this, "AutoSplitWeepers", false, DataSourceUpdateMode.OnPropertyChanged);

            // defaults
            this.DrawWithoutLoads = true;
            this.AutoStartEnd = true;
        }

        public XmlNode GetSettings(XmlDocument doc)
        {
            XmlElement settingsNode = doc.CreateElement("Settings");

            settingsNode.AppendChild(ToElement(doc, "Version", Assembly.GetExecutingAssembly().GetName().Version.ToString(3)));

            settingsNode.AppendChild(ToElement(doc, "DrawWithoutLoads", this.DrawWithoutLoads));
            settingsNode.AppendChild(ToElement(doc, "AutoStartEnd", this.AutoStartEnd));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitIntroEnd", this.AutoSplitIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitMissionEnd", this.AutoSplitMissionEnd));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitPrisonEscape", this.AutoSplitPrisonEscape));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitOutsidersDream", this.AutoSplitOutsidersDream));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitWeepers", this.AutoSplitWeepers));

            return settingsNode;
        }

        public void SetSettings(XmlNode settings)
        {
            this.DrawWithoutLoads = ParseBool(settings, "DrawWithoutLoads", true);
            this.AutoStartEnd = ParseBool(settings, "AutoStartEnd", true);
            this.AutoSplitIntroEnd = ParseBool(settings, "AutoSplitIntroEnd");
            this.AutoSplitMissionEnd = ParseBool(settings, "AutoSplitMissionEnd");
            this.AutoSplitPrisonEscape = ParseBool(settings, "AutoSplitPrisonEscape");
            this.AutoSplitOutsidersDream = ParseBool(settings, "AutoSplitOutsidersDream");
            this.AutoSplitWeepers = ParseBool(settings, "AutoSplitWeepers");
        }

        static bool ParseBool(XmlNode settings, string setting, bool default_ = false)
        {
            bool val;
            return settings[setting] != null ?
                (Boolean.TryParse(settings[setting].InnerText, out val) ? val : default_)
                : default_;
        }

        static XmlElement ToElement<T>(XmlDocument document, string name, T value)
        {
            XmlElement str = document.CreateElement(name);
            str.InnerText = value.ToString();
            return str;
        }
    }
}
