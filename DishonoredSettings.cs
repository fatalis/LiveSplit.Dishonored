using System;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.Dishonored
{
    public partial class DishonoredSettings : UserControl
    {
        public bool AutoStartEnd { get; set; }
        public bool AutoSplitIntroEnd { get; set; }
        public bool AutoSplitMissionEnd { get; set; }
        public bool AutoSplitPrisonEscape { get; set; }
        public bool AutoSplitOutsidersDream { get; set; }
        public bool AutoSplitWeepers { get; set; }
        public bool CutsceneSpeedup { get; set; }
        public int SpeedupIntroEnd { get; set; }
        public int SpeedupPostSewers { get; set; }
        public int SpeedupCampbell { get; set; }
        public int SpeedupPostCampbell { get; set; }
        public int SpeedupCat { get; set; }
        public int SpeedupPostCat { get; set; }
        public int SpeedupBridge { get; set; }
        public int SpeedupPostBridge { get; set; }
        public int SpeedupBoyle { get; set; }
        public int SpeedupPostBoyle { get; set; }
        public int SpeedupTower { get; set; }
        public int SpeedupPostTower { get; set; }
        public int SpeedupFlooded { get; set; }
        public int SpeedupKingsparrow { get; set; }
        public int SpeedupFlooded2 { get; set; }
        public int SpeedupPrison { get; set; }
        public int SpeedupPostCat2 { get; set; }
        public int SpeedupPostCat3 { get; set; }
        public int DelayIntroEnd { get; set; }
        public int DelayPostSewers { get; set; }
        public int DelayCampbell { get; set; }
        public int DelayPostCampbell { get; set; }
        public int DelayCat { get; set; }
        public int DelayPostCat { get; set; }
        public int DelayBridge { get; set; }
        public int DelayPostBridge { get; set; }
        public int DelayBoyle { get; set; }
        public int DelayPostBoyle { get; set; }
        public int DelayTower { get; set; }
        public int DelayPostTower { get; set; }
        public int DelayFlooded { get; set; }
        public int DelayKingsparrow { get; set; }
        public int DelayFlooded2 { get; set; }
        public int DelayPrison { get; set; }
        public int DelayPostCat2 { get; set; }
        public int DelayPostCat3 { get; set; }

        public DishonoredSettings()
        {
            InitializeComponent();

            this.chkAutoStartEnd.DataBindings.Add("Checked", this, "AutoStartEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoSplitIntroEnd.DataBindings.Add("Checked", this, "AutoSplitIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoSplitMissionEnd.DataBindings.Add("Checked", this, "AutoSplitMissionEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoSplitPrisonEscape.DataBindings.Add("Checked", this, "AutoSplitPrisonEscape", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoSplitOutsidersDream.DataBindings.Add("Checked", this, "AutoSplitOutsidersDream", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoSplitWeepers.DataBindings.Add("Checked", this, "AutoSplitWeepers", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkCutsceneSpeedup.DataBindings.Add("Checked", this, "CutsceneSpeedup", false, DataSourceUpdateMode.OnPropertyChanged);

            this.txtSpeedupIntroEnd.DataBindings.Add("Text", this, "SpeedupIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostSewers.DataBindings.Add("Text", this, "SpeedupPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupCampbell.DataBindings.Add("Text", this, "SpeedupCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostCampbell.DataBindings.Add("Text", this, "SpeedupPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupCat.DataBindings.Add("Text", this, "SpeedupCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostCat.DataBindings.Add("Text", this, "SpeedupPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupBridge.DataBindings.Add("Text", this, "SpeedupBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostBridge.DataBindings.Add("Text", this, "SpeedupPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupBoyle.DataBindings.Add("Text", this, "SpeedupBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostBoyle.DataBindings.Add("Text", this, "SpeedupPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupTower.DataBindings.Add("Text", this, "SpeedupTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostTower.DataBindings.Add("Text", this, "SpeedupPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupFlooded.DataBindings.Add("Text", this, "SpeedupFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupKingsparrow.DataBindings.Add("Text", this, "SpeedupKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupFlooded2.DataBindings.Add("Text", this, "SpeedupFlooded2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPrison.DataBindings.Add("Text", this, "SpeedupPrison", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostCat2.DataBindings.Add("Text", this, "SpeedupPostCat2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostCat3.DataBindings.Add("Text", this, "SpeedupPostCat3", false, DataSourceUpdateMode.OnPropertyChanged);

            this.txtDelayIntroEnd.DataBindings.Add("Text", this, "DelayIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostSewers.DataBindings.Add("Text", this, "DelayPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayCampbell.DataBindings.Add("Text", this, "DelayCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostCampbell.DataBindings.Add("Text", this, "DelayPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayCat.DataBindings.Add("Text", this, "DelayCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostCat.DataBindings.Add("Text", this, "DelayPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayBridge.DataBindings.Add("Text", this, "DelayBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostBridge.DataBindings.Add("Text", this, "DelayPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayBoyle.DataBindings.Add("Text", this, "DelayBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostBoyle.DataBindings.Add("Text", this, "DelayPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayTower.DataBindings.Add("Text", this, "DelayTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostTower.DataBindings.Add("Text", this, "DelayPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayFlooded.DataBindings.Add("Text", this, "DelayFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayKingsparrow.DataBindings.Add("Text", this, "DelayKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayFlooded2.DataBindings.Add("Text", this, "DelayFlooded2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPrison.DataBindings.Add("Text", this, "DelayPrison", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostCat2.DataBindings.Add("Text", this, "DelayPostCat2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostCat3.DataBindings.Add("Text", this, "DelayPostCat3", false, DataSourceUpdateMode.OnPropertyChanged);

            // defaults
            this.AutoStartEnd = true;

            this.SpeedupIntroEnd = 5900;
            this.SpeedupPostSewers = 5800;
            this.SpeedupCampbell = 4900;
            this.SpeedupPostCampbell = 2100;
            this.SpeedupCat = 5000;
            this.SpeedupPostCat = 6000;
            this.SpeedupBridge = 4300;
            this.SpeedupPostBridge = 4000;
            this.SpeedupBoyle = 3600;
            this.SpeedupPostBoyle = 3300;
            this.SpeedupTower = 5300;
            this.SpeedupPostTower = 3300;
            this.SpeedupFlooded = 5000;
            this.SpeedupKingsparrow = 4500;
            this.SpeedupFlooded2 = 6500;
            this.SpeedupPrison = 600;
            this.SpeedupPostCat2 = 2000;
            this.SpeedupPostCat3 = 2000;

            this.DelayIntroEnd = 1500;
            this.DelayPostSewers = 1500;
            this.DelayCampbell = 1500;
            this.DelayPostCampbell = 1500;
            this.DelayCat = 1500;
            this.DelayPostCat = 1500;
            this.DelayBridge = 1500;
            this.DelayPostBridge = 1500;
            this.DelayBoyle = 1500;
            this.DelayPostBoyle = 1500;
            this.DelayTower = 1500;
            this.DelayPostTower = 1500;
            this.DelayFlooded = 1500;
            this.DelayKingsparrow = 1500;
            this.DelayFlooded2 = 8000;
            this.DelayPrison = 1500;
            this.DelayPostCat2 = 500;
            this.DelayPostCat3 = 500;
        }

        public XmlNode GetSettings(XmlDocument doc)
        {
            XmlElement settingsNode = doc.CreateElement("Settings");

            settingsNode.AppendChild(ToElement(doc, "Version", Assembly.GetExecutingAssembly().GetName().Version.ToString(3)));

            settingsNode.AppendChild(ToElement(doc, "AutoStartEnd", this.AutoStartEnd));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitIntroEnd", this.AutoSplitIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitMissionEnd", this.AutoSplitMissionEnd));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitPrisonEscape", this.AutoSplitPrisonEscape));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitOutsidersDream", this.AutoSplitOutsidersDream));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitWeepers", this.AutoSplitWeepers));
            settingsNode.AppendChild(ToElement(doc, "CutsceneSpeedup", this.CutsceneSpeedup));
            settingsNode.AppendChild(ToElement(doc, "SpeedupIntroEnd", this.SpeedupIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostSewers", this.SpeedupPostSewers));
            settingsNode.AppendChild(ToElement(doc, "SpeedupCampbell", this.SpeedupCampbell));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostCampbell", this.SpeedupPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "SpeedupCat", this.SpeedupCat));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostCat", this.SpeedupPostCat));
            settingsNode.AppendChild(ToElement(doc, "SpeedupBridge", this.SpeedupBridge));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostBridge", this.SpeedupPostBridge));
            settingsNode.AppendChild(ToElement(doc, "SpeedupBoyle", this.SpeedupBoyle));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostBoyle", this.SpeedupPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "SpeedupTower", this.SpeedupTower));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostTower", this.SpeedupPostTower));
            settingsNode.AppendChild(ToElement(doc, "SpeedupFlooded", this.SpeedupFlooded));
            settingsNode.AppendChild(ToElement(doc, "SpeedupKingsparrow", this.SpeedupKingsparrow));
            settingsNode.AppendChild(ToElement(doc, "SpeedupFlooded2", this.SpeedupFlooded2));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPrison", this.SpeedupPrison));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostCat2", this.SpeedupPostCat2));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostCat3", this.SpeedupPostCat3));
            settingsNode.AppendChild(ToElement(doc, "DelayIntroEnd", this.DelayIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "DelayPostSewers", this.DelayPostSewers));
            settingsNode.AppendChild(ToElement(doc, "DelayCampbell", this.DelayCampbell));
            settingsNode.AppendChild(ToElement(doc, "DelayPostCampbell", this.DelayPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "DelayCat", this.DelayCat));
            settingsNode.AppendChild(ToElement(doc, "DelayPostCat", this.DelayPostCat));
            settingsNode.AppendChild(ToElement(doc, "DelayBridge", this.DelayBridge));
            settingsNode.AppendChild(ToElement(doc, "DelayPostBridge", this.DelayPostBridge));
            settingsNode.AppendChild(ToElement(doc, "DelayBoyle", this.DelayBoyle));
            settingsNode.AppendChild(ToElement(doc, "DelayPostBoyle", this.DelayPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "DelayTower", this.DelayTower));
            settingsNode.AppendChild(ToElement(doc, "DelayPostTower", this.DelayPostTower));
            settingsNode.AppendChild(ToElement(doc, "DelayFlooded", this.DelayFlooded));
            settingsNode.AppendChild(ToElement(doc, "DelayKingsparrow", this.DelayKingsparrow));
            settingsNode.AppendChild(ToElement(doc, "DelayFlooded2", this.DelayFlooded2));
            settingsNode.AppendChild(ToElement(doc, "DelayPrison", this.DelayPrison));
            settingsNode.AppendChild(ToElement(doc, "DelayPostCat2", this.DelayPostCat2));
            settingsNode.AppendChild(ToElement(doc, "DelayPostCat3", this.DelayPostCat3));

            return settingsNode;
        }

        public void SetSettings(XmlNode settings)
        {
            this.AutoStartEnd = ParseBool(settings, "AutoStartEnd", true);
            this.AutoSplitIntroEnd = ParseBool(settings, "AutoSplitIntroEnd");
            this.AutoSplitMissionEnd = ParseBool(settings, "AutoSplitMissionEnd");
            this.AutoSplitPrisonEscape = ParseBool(settings, "AutoSplitPrisonEscape");
            this.AutoSplitOutsidersDream = ParseBool(settings, "AutoSplitOutsidersDream");
            this.AutoSplitWeepers = ParseBool(settings, "AutoSplitWeepers");
            this.CutsceneSpeedup = ParseBool(settings, "CutsceneSpeedup");
            this.SpeedupIntroEnd = ParseInt(settings, "SpeedupIntroEnd", 5900);
            this.SpeedupPostSewers = ParseInt(settings, "SpeedupPostSewers", 5800);
            this.SpeedupCampbell = ParseInt(settings, "SpeedupCampbell", 4900);
            this.SpeedupPostCampbell = ParseInt(settings, "SpeedupPostCampbell", 2100);
            this.SpeedupCat = ParseInt(settings, "SpeedupCat", 5000);
            this.SpeedupPostCat = ParseInt(settings, "SpeedupPostCat", 6000);
            this.SpeedupBridge = ParseInt(settings, "SpeedupBridge", 4300);
            this.SpeedupPostBridge = ParseInt(settings, "SpeedupPostBridge", 4000);
            this.SpeedupBoyle = ParseInt(settings, "SpeedupBoyle", 3600);
            this.SpeedupPostBoyle = ParseInt(settings, "SpeedupPostBoyle", 3300);
            this.SpeedupTower = ParseInt(settings, "SpeedupTower", 5300);
            this.SpeedupPostTower = ParseInt(settings, "SpeedupPostTower", 3300);
            this.SpeedupFlooded = ParseInt(settings, "SpeedupFlooded", 5000);
            this.SpeedupKingsparrow = ParseInt(settings, "SpeedupKingsparrow", 4500);
            this.SpeedupFlooded2 = ParseInt(settings, "SpeedupFlooded2", 6500);
            this.SpeedupPrison = ParseInt(settings, "SpeedupPrison", 600);
            this.SpeedupPostCat2 = ParseInt(settings, "SpeedupPostCat2", 2000);
            this.SpeedupPostCat3 = ParseInt(settings, "SpeedupPostCat3", 2000);
            this.DelayIntroEnd = ParseInt(settings, "DelayIntroEnd", 1500);
            this.DelayPostSewers = ParseInt(settings, "DelayPostSewers", 1500);
            this.DelayCampbell = ParseInt(settings, "DelayCampbell", 1500);
            this.DelayPostCampbell = ParseInt(settings, "DelayPostCampbell", 1500);
            this.DelayCat = ParseInt(settings, "DelayCat", 1500);
            this.DelayPostCat = ParseInt(settings, "DelayPostCat", 1500);
            this.DelayBridge = ParseInt(settings, "DelayBridge", 1500);
            this.DelayPostBridge = ParseInt(settings, "DelayPostBridge", 1500);
            this.DelayBoyle = ParseInt(settings, "DelayBoyle", 1500);
            this.DelayPostBoyle = ParseInt(settings, "DelayPostBoyle", 1500);
            this.DelayTower = ParseInt(settings, "DelayTower", 1500);
            this.DelayPostTower = ParseInt(settings, "DelayPostTower", 1500);
            this.DelayFlooded = ParseInt(settings, "DelayFlooded", 1500);
            this.DelayKingsparrow = ParseInt(settings, "DelayKingsparrow", 1500);
            this.DelayFlooded2 = ParseInt(settings, "DelayFlooded2", 8000);
            this.DelayPrison = ParseInt(settings, "DelayPrison", 1500);
            this.DelayPostCat2 = ParseInt(settings, "DelayPostCat2", 500);
            this.DelayPostCat3 = ParseInt(settings, "DelayPostCat3", 500);
        }

        static bool ParseBool(XmlNode settings, string setting, bool default_ = false)
        {
            bool val;
            return settings[setting] != null ?
                (bool.TryParse(settings[setting].InnerText, out val) ? val : default_)
                : default_;
        }

        static int ParseInt(XmlNode settings, string setting, int default_ = 0)
        {
            int val;
            return settings[setting] != null ?
                (int.TryParse(settings[setting].InnerText, out val) ? val : default_)
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
