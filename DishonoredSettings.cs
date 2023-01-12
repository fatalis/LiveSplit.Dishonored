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
        public bool AutoSplitDLC06IntroEnd { get; set; }
        public bool EnableSpeedups { get; set; }
        public bool SpeedupMovies { get; set; }
        public bool SpeedupInGameCutscenes { get; set; }
        public bool SpeedupLoadDelays { get; set; }
        public bool SpeedupLoadPositions { get; set; }

        public DishonoredSettings()
        {
            InitializeComponent();

            chkAutoStartEnd.DataBindings.Add("Checked", this, "AutoStartEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoSplitIntroEnd.DataBindings.Add("Checked", this, "AutoSplitIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoSplitMissionEnd.DataBindings.Add("Checked", this, "AutoSplitMissionEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoSplitPrisonEscape.DataBindings.Add("Checked", this, "AutoSplitPrisonEscape", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoSplitOutsidersDream.DataBindings.Add("Checked", this, "AutoSplitOutsidersDream", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoSplitWeepers.DataBindings.Add("Checked", this, "AutoSplitWeepers", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoSplitDLC06IntroEnd.DataBindings.Add("Checked", this, "AutoSplitDLC06IntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);

            chkEnableSpeedups.DataBindings.Add("Checked", this, "EnableSpeedups", false, DataSourceUpdateMode.OnPropertyChanged);
            chkSpeedupMovies.DataBindings.Add("Checked", this, "SpeedupMovies", false, DataSourceUpdateMode.OnPropertyChanged);
            chkSpeedupInGameCutscenes.DataBindings.Add("Checked", this, "SpeedupInGameCutscenes", false, DataSourceUpdateMode.OnPropertyChanged);
            chkSpeedupLoadDelays.DataBindings.Add("Checked", this, "SpeedupLoadDelays", false, DataSourceUpdateMode.OnPropertyChanged);
            chkSpeedupLoadPositions.DataBindings.Add("Checked", this, "SpeedupLoadPositions", false, DataSourceUpdateMode.OnPropertyChanged);

            // defaults
            AutoStartEnd = true;

            SpeedupMovies = true;
            SpeedupInGameCutscenes = true;
            SpeedupLoadDelays = true;
            SpeedupLoadPositions = true;
        }

        public XmlNode GetSettings(XmlDocument doc)
        {
            XmlElement settingsNode = doc.CreateElement("Settings");

            settingsNode.AppendChild(ToElement(doc, "Version", Assembly.GetExecutingAssembly().GetName().Version.ToString(3)));

            settingsNode.AppendChild(ToElement(doc, "AutoStartEnd", AutoStartEnd));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitIntroEnd", AutoSplitIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitMissionEnd", AutoSplitMissionEnd));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitPrisonEscape", AutoSplitPrisonEscape));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitOutsidersDream", AutoSplitOutsidersDream));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitWeepers", AutoSplitWeepers));
            settingsNode.AppendChild(ToElement(doc, "AutoSplitDLC06IntroEnd", AutoSplitDLC06IntroEnd));
            settingsNode.AppendChild(ToElement(doc, "EnableSpeedups", EnableSpeedups));
            settingsNode.AppendChild(ToElement(doc, "SpeedupMovies", SpeedupMovies));
            settingsNode.AppendChild(ToElement(doc, "SpeedupInGameCutscenes", SpeedupInGameCutscenes));
            settingsNode.AppendChild(ToElement(doc, "SpeedupLoadDelays", SpeedupLoadDelays));
            settingsNode.AppendChild(ToElement(doc, "SpeedupLoadPositions", SpeedupLoadPositions));

            return settingsNode;
        }

        public void SetSettings(XmlNode settings)
        {
            AutoStartEnd = ParseBool(settings, "AutoStartEnd", true);
            AutoSplitIntroEnd = ParseBool(settings, "AutoSplitIntroEnd");
            AutoSplitMissionEnd = ParseBool(settings, "AutoSplitMissionEnd");
            AutoSplitPrisonEscape = ParseBool(settings, "AutoSplitPrisonEscape");
            AutoSplitOutsidersDream = ParseBool(settings, "AutoSplitOutsidersDream");
            AutoSplitWeepers = ParseBool(settings, "AutoSplitWeepers");
            AutoSplitDLC06IntroEnd = ParseBool(settings, "AutoSplitDLC06IntroEnd");
            EnableSpeedups = ParseBool(settings, "EnableSpeedups");
            SpeedupMovies = ParseBool(settings, "SpeedupMovies", true);
            SpeedupInGameCutscenes = ParseBool(settings, "SpeedupInGameCutscenes", true);
            SpeedupLoadDelays = ParseBool(settings, "SpeedupLoadDelays", true);
            SpeedupLoadPositions = ParseBool(settings, "SpeedupLoadPositions", true);
        }

        static bool ParseBool(XmlNode settings, string setting, bool default_ = false)
        {
            return settings[setting] != null ?
                (Boolean.TryParse(settings[setting].InnerText, out var val) ? val : default_)
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
