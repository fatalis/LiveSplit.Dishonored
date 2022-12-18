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
        public bool LogCoords { get; set; }

        public bool EnabledIntro { get; set; }
        public bool EnabledBlindIntro { get; set; }
        public bool EnabledIntroEnd { get; set; }
        public bool EnabledPrison { get; set; }
        public bool EnabledPostSewers { get; set; }
        public bool EnabledCampbell { get; set; }
        public bool EnabledPostCampbell { get; set; }
        public bool EnabledCat { get; set; }
        public bool EnabledPostCat { get; set; }
        public bool EnabledPostCat2 { get; set; }
        public bool EnabledPostCat3 { get; set; }
        public bool EnabledBridge { get; set; }
        public bool EnabledPostBridge { get; set; }
        public bool EnabledBoyle { get; set; }
        public bool EnabledPostBoyle { get; set; }
        public bool EnabledTower { get; set; }
        public bool EnabledPostTower { get; set; }
        public bool EnabledFlooded { get; set; }
        public bool EnabledFlooded2 { get; set; }
        public bool EnabledFloodedCell { get; set; }
        public bool EnabledKingsparrow { get; set; }
        public bool EnabledLighthouse { get; set; }

        public int SpeedupIntro { get; set; }
        public int SpeedupBlindIntro { get; set; }
        public int SpeedupIntroEnd { get; set; }
        public int SpeedupPrison { get; set; }
        public int SpeedupPostSewers { get; set; }
        public int SpeedupCampbell { get; set; }
        public int SpeedupPostCampbell { get; set; }
        public int SpeedupCat { get; set; }
        public int SpeedupPostCat { get; set; }
        public int SpeedupPostCat2 { get; set; }
        public int SpeedupPostCat3 { get; set; }
        public int SpeedupBridge { get; set; }
        public int SpeedupPostBridge { get; set; }
        public int SpeedupBoyle { get; set; }
        public int SpeedupPostBoyle { get; set; }
        public int SpeedupTower { get; set; }
        public int SpeedupPostTower { get; set; }
        public int SpeedupFlooded { get; set; }
        public int SpeedupFlooded2 { get; set; }
        public int SpeedupFloodedCell { get; set; }
        public int SpeedupKingsparrow { get; set; }
        public int SpeedupLighthouse { get; set; }

        public int DelayPrison { get; set; }
        public int DelayPostSewers { get; set; }
        public int DelayCampbell { get; set; }
        public int DelayPostCampbell { get; set; }
        public int DelayCat { get; set; }
        public int DelayPostCat { get; set; }
        public int DelayPostCat2 { get; set; }
        public int DelayPostCat3 { get; set; }
        public int DelayBridge { get; set; }
        public int DelayPostBridge { get; set; }
        public int DelayBoyle { get; set; }
        public int DelayPostBoyle { get; set; }
        public int DelayTower { get; set; }
        public int DelayPostTower { get; set; }
        public int DelayFlooded { get; set; }
        public int DelayFlooded2 { get; set; }
        public int DelayFloodedCell { get; set; }
        public int DelayKingsparrow { get; set; }
        public int DelayLighthouse { get; set; }

        public float XIntro { get; set; }
        public float XBlindIntro { get; set; }
        public float XIntroEnd { get; set; }
        public float XPostSewers { get; set; }
        public float XCampbell { get; set; }
        public float XPostCampbell { get; set; }
        public float XCat { get; set; }
        public float XPostCat { get; set; }
        public float XBridge { get; set; }
        public float XPostBridge { get; set; }
        public float XBoyle { get; set; }
        public float XPostBoyle { get; set; }
        public float XTower { get; set; }
        public float XPostTower { get; set; }
        public float XFlooded { get; set; }
        public float XKingsparrow { get; set; }

        public float YIntro { get; set; }
        public float YBlindIntro { get; set; }
        public float YIntroEnd { get; set; }
        public float YPostSewers { get; set; }
        public float YCampbell { get; set; }
        public float YPostCampbell { get; set; }
        public float YCat { get; set; }
        public float YPostCat { get; set; }
        public float YBridge { get; set; }
        public float YPostBridge { get; set; }
        public float YBoyle { get; set; }
        public float YPostBoyle { get; set; }
        public float YTower { get; set; }
        public float YPostTower { get; set; }
        public float YFlooded { get; set; }
        public float YKingsparrow { get; set; }

        public float ZIntro { get; set; }
        public float ZBlindIntro { get; set; }
        public float ZIntroEnd { get; set; }
        public float ZPostSewers { get; set; }
        public float ZCampbell { get; set; }
        public float ZPostCampbell { get; set; }
        public float ZCat { get; set; }
        public float ZPostCat { get; set; }
        public float ZBridge { get; set; }
        public float ZPostBridge { get; set; }
        public float ZBoyle { get; set; }
        public float ZPostBoyle { get; set; }
        public float ZTower { get; set; }
        public float ZPostTower { get; set; }
        public float ZFlooded { get; set; }
        public float ZKingsparrow { get; set; }

        public DishonoredSettings()
        {
            InitializeComponent();

            chkAutoStartEnd.DataBindings.Add("Checked", this, "AutoStartEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoSplitIntroEnd.DataBindings.Add("Checked", this, "AutoSplitIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoSplitMissionEnd.DataBindings.Add("Checked", this, "AutoSplitMissionEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoSplitPrisonEscape.DataBindings.Add("Checked", this, "AutoSplitPrisonEscape", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoSplitOutsidersDream.DataBindings.Add("Checked", this, "AutoSplitOutsidersDream", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoSplitWeepers.DataBindings.Add("Checked", this, "AutoSplitWeepers", false, DataSourceUpdateMode.OnPropertyChanged);

            chkCutsceneSpeedup.DataBindings.Add("Checked", this, "CutsceneSpeedup", false, DataSourceUpdateMode.OnPropertyChanged);
            chkLogCoords.DataBindings.Add("Checked", this, "LogCoords", false, DataSourceUpdateMode.OnPropertyChanged);

            chkEnabledIntro.DataBindings.Add("Checked", this, "EnabledIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledBlindIntro.DataBindings.Add("Checked", this, "EnabledBlindIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledIntroEnd.DataBindings.Add("Checked", this, "EnabledIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledPrison.DataBindings.Add("Checked", this, "EnabledPrison", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledPostSewers.DataBindings.Add("Checked", this, "EnabledPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledCampbell.DataBindings.Add("Checked", this, "EnabledCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledPostCampbell.DataBindings.Add("Checked", this, "EnabledPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledCat.DataBindings.Add("Checked", this, "EnabledCat", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledPostCat.DataBindings.Add("Checked", this, "EnabledPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledPostCat2.DataBindings.Add("Checked", this, "EnabledPostCat2", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledPostCat3.DataBindings.Add("Checked", this, "EnabledPostCat3", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledBridge.DataBindings.Add("Checked", this, "EnabledBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledPostBridge.DataBindings.Add("Checked", this, "EnabledPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledBoyle.DataBindings.Add("Checked", this, "EnabledBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledPostBoyle.DataBindings.Add("Checked", this, "EnabledPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledTower.DataBindings.Add("Checked", this, "EnabledTower", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledPostTower.DataBindings.Add("Checked", this, "EnabledPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledFlooded.DataBindings.Add("Checked", this, "EnabledFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledFlooded2.DataBindings.Add("Checked", this, "EnabledFlooded2", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledFloodedCell.DataBindings.Add("Checked", this, "EnabledFloodedCell", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledKingsparrow.DataBindings.Add("Checked", this, "EnabledKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabledLighthouse.DataBindings.Add("Checked", this, "EnabledLighthouse", false, DataSourceUpdateMode.OnPropertyChanged);

            txtSpeedupIntro.DataBindings.Add("Text", this, "SpeedupIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupBlindIntro.DataBindings.Add("Text", this, "SpeedupBlindIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupIntroEnd.DataBindings.Add("Text", this, "SpeedupIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupPrison.DataBindings.Add("Text", this, "SpeedupPrison", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupPostSewers.DataBindings.Add("Text", this, "SpeedupPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupCampbell.DataBindings.Add("Text", this, "SpeedupCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupPostCampbell.DataBindings.Add("Text", this, "SpeedupPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupCat.DataBindings.Add("Text", this, "SpeedupCat", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupPostCat.DataBindings.Add("Text", this, "SpeedupPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupPostCat2.DataBindings.Add("Text", this, "SpeedupPostCat2", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupPostCat3.DataBindings.Add("Text", this, "SpeedupPostCat3", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupBridge.DataBindings.Add("Text", this, "SpeedupBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupPostBridge.DataBindings.Add("Text", this, "SpeedupPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupBoyle.DataBindings.Add("Text", this, "SpeedupBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupPostBoyle.DataBindings.Add("Text", this, "SpeedupPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupTower.DataBindings.Add("Text", this, "SpeedupTower", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupPostTower.DataBindings.Add("Text", this, "SpeedupPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupFlooded.DataBindings.Add("Text", this, "SpeedupFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupFlooded2.DataBindings.Add("Text", this, "SpeedupFlooded2", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupFloodedCell.DataBindings.Add("Text", this, "SpeedupFloodedCell", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupKingsparrow.DataBindings.Add("Text", this, "SpeedupKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSpeedupLighthouse.DataBindings.Add("Text", this, "SpeedupLighthouse", false, DataSourceUpdateMode.OnPropertyChanged);

            txtDelayPrison.DataBindings.Add("Text", this, "DelayPrison", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayPostSewers.DataBindings.Add("Text", this, "DelayPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayCampbell.DataBindings.Add("Text", this, "DelayCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayPostCampbell.DataBindings.Add("Text", this, "DelayPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayCat.DataBindings.Add("Text", this, "DelayCat", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayPostCat.DataBindings.Add("Text", this, "DelayPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayPostCat2.DataBindings.Add("Text", this, "DelayPostCat2", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayPostCat3.DataBindings.Add("Text", this, "DelayPostCat3", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayBridge.DataBindings.Add("Text", this, "DelayBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayPostBridge.DataBindings.Add("Text", this, "DelayPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayBoyle.DataBindings.Add("Text", this, "DelayBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayPostBoyle.DataBindings.Add("Text", this, "DelayPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayTower.DataBindings.Add("Text", this, "DelayTower", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayPostTower.DataBindings.Add("Text", this, "DelayPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayFlooded.DataBindings.Add("Text", this, "DelayFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayFlooded2.DataBindings.Add("Text", this, "DelayFlooded2", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayFloodedCell.DataBindings.Add("Text", this, "DelayFloodedCell", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayKingsparrow.DataBindings.Add("Text", this, "DelayKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);
            txtDelayLighthouse.DataBindings.Add("Text", this, "DelayLighthouse", false, DataSourceUpdateMode.OnPropertyChanged);

            txtXIntro.DataBindings.Add("Text", this, "XIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXBlindIntro.DataBindings.Add("Text", this, "XBlindIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXIntroEnd.DataBindings.Add("Text", this, "XIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXPostSewers.DataBindings.Add("Text", this, "XPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXCampbell.DataBindings.Add("Text", this, "XCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXPostCampbell.DataBindings.Add("Text", this, "XPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXCat.DataBindings.Add("Text", this, "XCat", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXPostCat.DataBindings.Add("Text", this, "XPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXBridge.DataBindings.Add("Text", this, "XBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXPostBridge.DataBindings.Add("Text", this, "XPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXBoyle.DataBindings.Add("Text", this, "XBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXPostBoyle.DataBindings.Add("Text", this, "XPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXTower.DataBindings.Add("Text", this, "XTower", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXPostTower.DataBindings.Add("Text", this, "XPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXFlooded.DataBindings.Add("Text", this, "XFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            txtXKingsparrow.DataBindings.Add("Text", this, "XKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);

            txtYIntro.DataBindings.Add("Text", this, "YIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYBlindIntro.DataBindings.Add("Text", this, "YBlindIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYIntroEnd.DataBindings.Add("Text", this, "YIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYPostSewers.DataBindings.Add("Text", this, "YPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYCampbell.DataBindings.Add("Text", this, "YCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYPostCampbell.DataBindings.Add("Text", this, "YPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYCat.DataBindings.Add("Text", this, "YCat", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYPostCat.DataBindings.Add("Text", this, "YPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYBridge.DataBindings.Add("Text", this, "YBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYPostBridge.DataBindings.Add("Text", this, "YPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYBoyle.DataBindings.Add("Text", this, "YBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYPostBoyle.DataBindings.Add("Text", this, "YPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYTower.DataBindings.Add("Text", this, "YTower", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYPostTower.DataBindings.Add("Text", this, "YPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYFlooded.DataBindings.Add("Text", this, "YFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            txtYKingsparrow.DataBindings.Add("Text", this, "YKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);

            txtZIntro.DataBindings.Add("Text", this, "ZIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZBlindIntro.DataBindings.Add("Text", this, "ZBlindIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZIntroEnd.DataBindings.Add("Text", this, "ZIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZPostSewers.DataBindings.Add("Text", this, "ZPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZCampbell.DataBindings.Add("Text", this, "ZCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZPostCampbell.DataBindings.Add("Text", this, "ZPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZCat.DataBindings.Add("Text", this, "ZCat", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZPostCat.DataBindings.Add("Text", this, "ZPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZBridge.DataBindings.Add("Text", this, "ZBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZPostBridge.DataBindings.Add("Text", this, "ZPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZBoyle.DataBindings.Add("Text", this, "ZBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZPostBoyle.DataBindings.Add("Text", this, "ZPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZTower.DataBindings.Add("Text", this, "ZTower", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZPostTower.DataBindings.Add("Text", this, "ZPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZFlooded.DataBindings.Add("Text", this, "ZFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            txtZKingsparrow.DataBindings.Add("Text", this, "ZKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);

            // defaults
            AutoStartEnd = true;

            EnabledIntro = true;
            EnabledBlindIntro = true;
            EnabledIntroEnd = true;
            EnabledPrison = true;
            EnabledPostSewers = true;
            EnabledCampbell = true;
            EnabledPostCampbell = true;
            EnabledCat = true;
            EnabledPostCat = true;
            EnabledPostCat2 = true;
            EnabledPostCat3 = true;
            EnabledBridge = true;
            EnabledPostBridge = true;
            EnabledBoyle = true;
            EnabledPostBoyle = true;
            EnabledTower = true;
            EnabledPostTower = true;
            EnabledFlooded = true;
            EnabledFlooded2 = true;
            EnabledFloodedCell = true;
            EnabledKingsparrow = true;
            EnabledLighthouse  = true;

            SpeedupIntro = 9200;
            SpeedupBlindIntro = 3800;
            SpeedupIntroEnd = 6350;
            SpeedupPrison = 770;
            SpeedupPostSewers = 5640;
            SpeedupCampbell = 4450;
            SpeedupPostCampbell = 2050;
            SpeedupCat = 4550;
            SpeedupPostCat = 4700;
            SpeedupPostCat2 = 1500;
            SpeedupPostCat3 = 900;
            SpeedupBridge = 4040;
            SpeedupPostBridge = 4000;
            SpeedupBoyle = 2400;
            SpeedupPostBoyle = 3130;
            SpeedupTower = 5250;
            SpeedupPostTower = 3340;
            SpeedupFlooded = 5000;
            SpeedupFlooded2 = 6700;
            SpeedupFloodedCell = 1600;
            SpeedupKingsparrow = 4550;
            SpeedupLighthouse = 1100;

            DelayPrison = 200;
            DelayPostSewers = 1100;
            DelayCampbell = 1500;
            DelayPostCampbell = 1500;
            DelayCat = 2000;
            DelayPostCat = 1000;
            DelayPostCat2 = 9200;
            DelayPostCat3 = 1200;
            DelayBridge = 1100;
            DelayPostBridge = 1000;
            DelayBoyle = 1500;
            DelayPostBoyle = 700;
            DelayTower = 1500;
            DelayPostTower = 1100;
            DelayFlooded = 1500;
            DelayFlooded2 = 8000;
            DelayFloodedCell = 1500;
            DelayKingsparrow = 1500;
            DelayLighthouse = 1000;

            XIntro = -3908f;
            XBlindIntro = 16242.7f;
            XIntroEnd = 15591.5f;
            XPostSewers = 0f;
            XCampbell = 12809f;
            XPostCampbell = -2374f;
            XCat = 4471f;
            XPostCat = -11185f;
            XBridge = -12312f;
            XPostBridge = -11186f;
            XBoyle = -9340f;
            XPostBoyle = 0f;
            XTower = -9253f;
            XPostTower = 0f;
            XFlooded = -23249f;
            XKingsparrow = 0f;

            YIntro = 0f;
            YBlindIntro = 21620f;
            YIntroEnd = 21025.2f;
            YPostSewers = 0f;
            YCampbell = 0f;
            YPostCampbell = 0f;
            YCat = 0f;
            YPostCat = 0f;
            YBridge = 0f;
            YPostBridge = 0f;
            YBoyle = 0f;
            YPostBoyle = -9518f;
            YTower = 33007f;
            YPostTower = -10593f;
            YFlooded = 0f;
            YKingsparrow = 0f;

            ZIntro = -231f;
            ZBlindIntro = 3354.9f;
            ZIntroEnd = 3425.3f;
            ZPostSewers = -599f;
            ZCampbell = 0f;
            ZPostCampbell = -601f;
            ZCat = 1848f;
            ZPostCat = 0f;
            ZBridge = -583f;
            ZPostBridge = -584f;
            ZBoyle = -1951.4f;
            ZPostBoyle = -609f;
            ZTower = 0f;
            ZPostTower = -584f;
            ZFlooded = 0f;
            ZKingsparrow = 1043f;
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

            settingsNode.AppendChild(ToElement(doc, "CutsceneSpeedup", CutsceneSpeedup));
            settingsNode.AppendChild(ToElement(doc, "LogCoords", LogCoords));

            settingsNode.AppendChild(ToElement(doc, "EnabledIntro", EnabledIntro));
            settingsNode.AppendChild(ToElement(doc, "EnabledBlindIntro", EnabledBlindIntro));
            settingsNode.AppendChild(ToElement(doc, "EnabledIntroEnd", EnabledIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "EnabledPrison", EnabledPrison));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostSewers", EnabledPostSewers));
            settingsNode.AppendChild(ToElement(doc, "EnabledCampbell", EnabledCampbell));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostCampbell", EnabledPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "EnabledCat", EnabledCat));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostCat", EnabledPostCat));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostCat2", EnabledPostCat2));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostCat3", EnabledPostCat3));
            settingsNode.AppendChild(ToElement(doc, "EnabledBridge", EnabledBridge));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostBridge", EnabledPostBridge));
            settingsNode.AppendChild(ToElement(doc, "EnabledBoyle", EnabledBoyle));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostBoyle", EnabledPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "EnabledTower", EnabledTower));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostTower", EnabledPostTower));
            settingsNode.AppendChild(ToElement(doc, "EnabledFlooded", EnabledFlooded));
            settingsNode.AppendChild(ToElement(doc, "EnabledFlooded2", EnabledFlooded2));
            settingsNode.AppendChild(ToElement(doc, "EnabledFloodedCell", EnabledFloodedCell));
            settingsNode.AppendChild(ToElement(doc, "EnabledKingsparrow", EnabledKingsparrow));
            settingsNode.AppendChild(ToElement(doc, "EnabledLighthouse", EnabledLighthouse));

            settingsNode.AppendChild(ToElement(doc, "SpeedupIntro", SpeedupIntro));
            settingsNode.AppendChild(ToElement(doc, "SpeedupBlindIntro", SpeedupBlindIntro));
            settingsNode.AppendChild(ToElement(doc, "SpeedupIntroEnd", SpeedupIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPrison", SpeedupPrison));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostSewers", SpeedupPostSewers));
            settingsNode.AppendChild(ToElement(doc, "SpeedupCampbell", SpeedupCampbell));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostCampbell", SpeedupPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "SpeedupCat", SpeedupCat));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostCat", SpeedupPostCat));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostCat2", SpeedupPostCat2));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostCat3", SpeedupPostCat3));
            settingsNode.AppendChild(ToElement(doc, "SpeedupBridge", SpeedupBridge));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostBridge", SpeedupPostBridge));
            settingsNode.AppendChild(ToElement(doc, "SpeedupBoyle", SpeedupBoyle));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostBoyle", SpeedupPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "SpeedupTower", SpeedupTower));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostTower", SpeedupPostTower));
            settingsNode.AppendChild(ToElement(doc, "SpeedupFlooded", SpeedupFlooded));
            settingsNode.AppendChild(ToElement(doc, "SpeedupFlooded2", SpeedupFlooded2));
            settingsNode.AppendChild(ToElement(doc, "SpeedupFloodedCell", SpeedupFloodedCell));
            settingsNode.AppendChild(ToElement(doc, "SpeedupKingsparrow", SpeedupKingsparrow));
            settingsNode.AppendChild(ToElement(doc, "SpeedupLighthouse", SpeedupLighthouse));

            settingsNode.AppendChild(ToElement(doc, "DelayPrison", DelayPrison));
            settingsNode.AppendChild(ToElement(doc, "DelayPostSewers", DelayPostSewers));
            settingsNode.AppendChild(ToElement(doc, "DelayCampbell", DelayCampbell));
            settingsNode.AppendChild(ToElement(doc, "DelayPostCampbell", DelayPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "DelayCat", DelayCat));
            settingsNode.AppendChild(ToElement(doc, "DelayPostCat", DelayPostCat));
            settingsNode.AppendChild(ToElement(doc, "DelayPostCat2", DelayPostCat2));
            settingsNode.AppendChild(ToElement(doc, "DelayPostCat3", DelayPostCat3));
            settingsNode.AppendChild(ToElement(doc, "DelayBridge", DelayBridge));
            settingsNode.AppendChild(ToElement(doc, "DelayPostBridge", DelayPostBridge));
            settingsNode.AppendChild(ToElement(doc, "DelayBoyle", DelayBoyle));
            settingsNode.AppendChild(ToElement(doc, "DelayPostBoyle", DelayPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "DelayTower", DelayTower));
            settingsNode.AppendChild(ToElement(doc, "DelayPostTower", DelayPostTower));
            settingsNode.AppendChild(ToElement(doc, "DelayFlooded", DelayFlooded));
            settingsNode.AppendChild(ToElement(doc, "DelayFlooded2", DelayFlooded2));
            settingsNode.AppendChild(ToElement(doc, "DelayFloodedCell", DelayFloodedCell));
            settingsNode.AppendChild(ToElement(doc, "DelayKingsparrow", DelayKingsparrow));
            settingsNode.AppendChild(ToElement(doc, "DelayLighthouse", DelayLighthouse));

            settingsNode.AppendChild(ToElement(doc, "XIntro", XIntro));
            settingsNode.AppendChild(ToElement(doc, "XBlindIntro", XBlindIntro));
            settingsNode.AppendChild(ToElement(doc, "XIntroEnd", XIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "XPostSewers", XPostSewers));
            settingsNode.AppendChild(ToElement(doc, "XCampbell", XCampbell));
            settingsNode.AppendChild(ToElement(doc, "XPostCampbell", XPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "XCat", XCat));
            settingsNode.AppendChild(ToElement(doc, "XPostCat", XPostCat));
            settingsNode.AppendChild(ToElement(doc, "XBridge", XBridge));
            settingsNode.AppendChild(ToElement(doc, "XPostBridge", XPostBridge));
            settingsNode.AppendChild(ToElement(doc, "XBoyle", XBoyle));
            settingsNode.AppendChild(ToElement(doc, "XPostBoyle", XPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "XTower", XTower));
            settingsNode.AppendChild(ToElement(doc, "XPostTower", XPostTower));
            settingsNode.AppendChild(ToElement(doc, "XFlooded", XFlooded));
            settingsNode.AppendChild(ToElement(doc, "XKingsparrow", XKingsparrow));

            settingsNode.AppendChild(ToElement(doc, "YIntro", YIntro));
            settingsNode.AppendChild(ToElement(doc, "YBlindIntro", YBlindIntro));
            settingsNode.AppendChild(ToElement(doc, "YIntroEnd", YIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "YPostSewers", YPostSewers));
            settingsNode.AppendChild(ToElement(doc, "YCampbell", YCampbell));
            settingsNode.AppendChild(ToElement(doc, "YPostCampbell", YPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "YCat", YCat));
            settingsNode.AppendChild(ToElement(doc, "YPostCat", YPostCat));
            settingsNode.AppendChild(ToElement(doc, "YBridge", YBridge));
            settingsNode.AppendChild(ToElement(doc, "YPostBridge", YPostBridge));
            settingsNode.AppendChild(ToElement(doc, "YBoyle", YBoyle));
            settingsNode.AppendChild(ToElement(doc, "YPostBoyle", YPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "YTower", YTower));
            settingsNode.AppendChild(ToElement(doc, "YPostTower", YPostTower));
            settingsNode.AppendChild(ToElement(doc, "YFlooded", YFlooded));
            settingsNode.AppendChild(ToElement(doc, "YKingsparrow", YKingsparrow));

            settingsNode.AppendChild(ToElement(doc, "ZIntro", ZIntro));
            settingsNode.AppendChild(ToElement(doc, "ZBlindIntro", ZBlindIntro));
            settingsNode.AppendChild(ToElement(doc, "ZIntroEnd", ZIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "ZPostSewers", ZPostSewers));
            settingsNode.AppendChild(ToElement(doc, "ZCampbell", ZCampbell));
            settingsNode.AppendChild(ToElement(doc, "ZPostCampbell", ZPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "ZCat", ZCat));
            settingsNode.AppendChild(ToElement(doc, "ZPostCat", ZPostCat));
            settingsNode.AppendChild(ToElement(doc, "ZBridge", ZBridge));
            settingsNode.AppendChild(ToElement(doc, "ZPostBridge", ZPostBridge));
            settingsNode.AppendChild(ToElement(doc, "ZBoyle", ZBoyle));
            settingsNode.AppendChild(ToElement(doc, "ZPostBoyle", ZPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "ZTower", ZTower));
            settingsNode.AppendChild(ToElement(doc, "ZPostTower", ZPostTower));
            settingsNode.AppendChild(ToElement(doc, "ZFlooded", ZFlooded));
            settingsNode.AppendChild(ToElement(doc, "ZKingsparrow", ZKingsparrow));

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

            CutsceneSpeedup = ParseBool(settings, "CutsceneSpeedup");
            LogCoords = ParseBool(settings, "LogCoords");

            EnabledIntro = ParseBool(settings, "EnabledIntro", true);
            EnabledBlindIntro = ParseBool(settings, "EnabledBlindIntro", true);
            EnabledIntroEnd = ParseBool(settings, "EnabledIntroEnd", true);
            EnabledPrison = ParseBool(settings, "EnabledPrison", true);
            EnabledPostSewers = ParseBool(settings, "EnabledPostSewers", true);
            EnabledCampbell = ParseBool(settings, "EnabledCampbell", true);
            EnabledPostCampbell = ParseBool(settings, "EnabledPostCampbell", true);
            EnabledCat = ParseBool(settings, "EnabledCat", true);
            EnabledPostCat = ParseBool(settings, "EnabledPostCat", true);
            EnabledPostCat2 = ParseBool(settings, "EnabledPostCat2", true);
            EnabledPostCat3 = ParseBool(settings, "EnabledPostCat3", true);
            EnabledBridge = ParseBool(settings, "EnabledBridge", true);
            EnabledPostBridge = ParseBool(settings, "EnabledPostBridge", true);
            EnabledBoyle = ParseBool(settings, "EnabledBoyle", true);
            EnabledPostBoyle = ParseBool(settings, "EnabledPostBoyle", true);
            EnabledTower = ParseBool(settings, "EnabledTower", true);
            EnabledPostTower = ParseBool(settings, "EnabledPostTower", true);
            EnabledFlooded = ParseBool(settings, "EnabledFlooded", true);
            EnabledFlooded2 = ParseBool(settings, "EnabledFlooded2", true);
            EnabledFloodedCell = ParseBool(settings, "EnabledFloodedCell", true);
            EnabledKingsparrow = ParseBool(settings, "EnabledKingsparrow", true);
            EnabledLighthouse = ParseBool(settings, "EnabledLighthouse", true);

            SpeedupIntro = ParseInt(settings, "SpeedupIntro", 9200);
            SpeedupBlindIntro = ParseInt(settings, "SpeedupBlindIntro", 3800);
            SpeedupIntroEnd = ParseInt(settings, "SpeedupIntroEnd", 6350);
            SpeedupPrison = ParseInt(settings, "SpeedupPrison", 770);
            SpeedupPostSewers = ParseInt(settings, "SpeedupPostSewers", 5640);
            SpeedupCampbell = ParseInt(settings, "SpeedupCampbell", 4450);
            SpeedupPostCampbell = ParseInt(settings, "SpeedupPostCampbell", 2050);
            SpeedupCat = ParseInt(settings, "SpeedupCat", 4550);
            SpeedupPostCat = ParseInt(settings, "SpeedupPostCat", 4700);
            SpeedupPostCat2 = ParseInt(settings, "SpeedupPostCat2", 1500);
            SpeedupPostCat3 = ParseInt(settings, "SpeedupPostCat3", 900);
            SpeedupBridge = ParseInt(settings, "SpeedupBridge", 4040);
            SpeedupPostBridge = ParseInt(settings, "SpeedupPostBridge", 4000);
            SpeedupBoyle = ParseInt(settings, "SpeedupBoyle", 2400);
            SpeedupPostBoyle = ParseInt(settings, "SpeedupPostBoyle", 3130);
            SpeedupTower = ParseInt(settings, "SpeedupTower", 5250);
            SpeedupPostTower = ParseInt(settings, "SpeedupPostTower", 3340);
            SpeedupFlooded = ParseInt(settings, "SpeedupFlooded", 5000);
            SpeedupFlooded2 = ParseInt(settings, "SpeedupFlooded2", 6700);
            SpeedupFloodedCell = ParseInt(settings, "SpeedupFloodedCell", 1600);
            SpeedupKingsparrow = ParseInt(settings, "SpeedupKingsparrow", 4550);
            SpeedupLighthouse = ParseInt(settings, "SpeedupLighthouse", 1100);

            DelayPrison = ParseInt(settings, "DelayPrison", 200);
            DelayPostSewers = ParseInt(settings, "DelayPostSewers", 1100);
            DelayCampbell = ParseInt(settings, "DelayCampbell", 1500);
            DelayPostCampbell = ParseInt(settings, "DelayPostCampbell", 1500);
            DelayCat = ParseInt(settings, "DelayCat", 2000);
            DelayPostCat = ParseInt(settings, "DelayPostCat", 1000);
            DelayPostCat2 = ParseInt(settings, "DelayPostCat2", 9200);
            DelayPostCat3 = ParseInt(settings, "DelayPostCat3", 1200);
            DelayBridge = ParseInt(settings, "DelayBridge", 1100);
            DelayPostBridge = ParseInt(settings, "DelayPostBridge", 1000);
            DelayBoyle = ParseInt(settings, "DelayBoyle", 1500);
            DelayPostBoyle = ParseInt(settings, "DelayPostBoyle", 700);
            DelayTower = ParseInt(settings, "DelayTower", 1500);
            DelayPostTower = ParseInt(settings, "DelayPostTower", 1100);
            DelayFlooded = ParseInt(settings, "DelayFlooded", 1500);
            DelayFlooded2 = ParseInt(settings, "DelayFlooded2", 8000);
            DelayFloodedCell = ParseInt(settings, "DelayFloodedCell", 1500);
            DelayKingsparrow = ParseInt(settings, "DelayKingsparrow", 1500);
            DelayLighthouse = ParseInt(settings, "DelayLighthouse", 1000);

            XIntro = ParseFloat(settings, "XIntro", -3908f);
            XBlindIntro = ParseFloat(settings, "XBlindIntro", 16242.7f);
            XIntroEnd = ParseFloat(settings, "XIntroEnd", 15591.5f);
            XPostSewers = ParseFloat(settings, "XPostSewers", 0f);
            XCampbell = ParseFloat(settings, "XCampbell", 12809f);
            XPostCampbell = ParseFloat(settings, "XPostCampbell", -2374f);
            XCat = ParseFloat(settings, "XCat", 4471f);
            XPostCat = ParseFloat(settings, "XPostCat", -11185f);
            XBridge = ParseFloat(settings, "XBridge", -12312f);
            XPostBridge = ParseFloat(settings, "XPostBridge", -11186f);
            XBoyle = ParseFloat(settings, "XBoyle", -9340f);
            XPostBoyle = ParseFloat(settings, "XPostBoyle", 0f);
            XTower = ParseFloat(settings, "XTower", -9253f);
            XPostTower = ParseFloat(settings, "XPostTower", 0f);
            XFlooded = ParseFloat(settings, "XFlooded", -23249f);
            XKingsparrow = ParseFloat(settings, "XKingsparrow", 0f);

            YIntro = ParseFloat(settings, "YIntro", 0f);
            YBlindIntro = ParseFloat(settings, "YBlindIntro", 21620f);
            YIntroEnd = ParseFloat(settings, "YIntroEnd", 21025.2f);
            YPostSewers = ParseFloat(settings, "YPostSewers", 0f);
            YCampbell = ParseFloat(settings, "YCampbell", 0f);
            YPostCampbell = ParseFloat(settings, "YPostCampbell", 0f);
            YCat = ParseFloat(settings, "YCat", 0f);
            YPostCat = ParseFloat(settings, "YPostCat", 0f);
            YBridge = ParseFloat(settings, "YBridge", 0f);
            YPostBridge = ParseFloat(settings, "YPostBridge", 0f);
            YBoyle = ParseFloat(settings, "YBoyle", 0f);
            YPostBoyle = ParseFloat(settings, "YPostBoyle", -9518f);
            YTower = ParseFloat(settings, "YTower", 33007f);
            YPostTower = ParseFloat(settings, "YPostTower", -10593f);
            YFlooded = ParseFloat(settings, "YFlooded", 0f);
            YKingsparrow = ParseFloat(settings, "YKingsparrow", 0f);

            ZIntro = ParseFloat(settings, "ZIntro", -231f);
            ZBlindIntro = ParseFloat(settings, "ZBlindIntro", 3354.9f);
            ZIntroEnd = ParseFloat(settings, "ZIntroEnd", 3425.3f);
            ZPostSewers = ParseFloat(settings, "ZPostSewers", -599f);
            ZCampbell = ParseFloat(settings, "ZCampbell", 0f);
            ZPostCampbell = ParseFloat(settings, "ZPostCampbell", -601f);
            ZCat = ParseFloat(settings, "ZCat", 1848f);
            ZPostCat = ParseFloat(settings, "ZPostCat", 0f);
            ZBridge = ParseFloat(settings, "ZBridge", -583f);
            ZPostBridge = ParseFloat(settings, "ZPostBridge", -584f);
            ZBoyle = ParseFloat(settings, "ZBoyle", -1951.4f);
            ZPostBoyle = ParseFloat(settings, "ZPostBoyle", -609f);
            ZTower = ParseFloat(settings, "ZTower", 0f);
            ZPostTower = ParseFloat(settings, "ZPostTower", -584f);
            ZFlooded = ParseFloat(settings, "ZFlooded", 0f);
            ZKingsparrow = ParseFloat(settings, "ZKingsparrow", 1043f);
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

        static float ParseFloat(XmlNode settings, string setting, float default_ = 0f)
        {
            float val;
            return settings[setting] != null ?
                (float.TryParse(settings[setting].InnerText, out val) ? val : default_)
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
