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

        public bool EnabledIntro { get; set; }
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

        public int DelayIntro { get; set; }
        public int DelayIntroEnd { get; set; }
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
        public float XIntroEnd { get; set; }
        public float XPrison { get; set; }
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
        public float XFloodedCell { get; set; }
        public float XKingsparrow { get; set; }
        public float XLighthouse { get; set; }

        public float YIntro { get; set; }
        public float YIntroEnd { get; set; }
        public float YPrison { get; set; }
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
        public float YFloodedCell { get; set; }
        public float YKingsparrow { get; set; }
        public float YLighthouse { get; set; }

        public float ZIntro { get; set; }
        public float ZIntroEnd { get; set; }
        public float ZPrison { get; set; }
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
        public float ZFloodedCell { get; set; }
        public float ZKingsparrow { get; set; }
        public float ZLighthouse { get; set; }

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

            this.chkEnabledIntro.DataBindings.Add("Checked", this, "EnabledIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledIntroEnd.DataBindings.Add("Checked", this, "EnabledIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledPrison.DataBindings.Add("Checked", this, "EnabledPrison", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledPostSewers.DataBindings.Add("Checked", this, "EnabledPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledCampbell.DataBindings.Add("Checked", this, "EnabledCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledPostCampbell.DataBindings.Add("Checked", this, "EnabledPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledCat.DataBindings.Add("Checked", this, "EnabledCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledPostCat.DataBindings.Add("Checked", this, "EnabledPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledPostCat2.DataBindings.Add("Checked", this, "EnabledPostCat2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledPostCat3.DataBindings.Add("Checked", this, "EnabledPostCat3", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledBridge.DataBindings.Add("Checked", this, "EnabledBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledPostBridge.DataBindings.Add("Checked", this, "EnabledPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledBoyle.DataBindings.Add("Checked", this, "EnabledBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledPostBoyle.DataBindings.Add("Checked", this, "EnabledPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledTower.DataBindings.Add("Checked", this, "EnabledTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledPostTower.DataBindings.Add("Checked", this, "EnabledPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledFlooded.DataBindings.Add("Checked", this, "EnabledFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledFlooded2.DataBindings.Add("Checked", this, "EnabledFlooded2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledFloodedCell.DataBindings.Add("Checked", this, "EnabledFloodedCell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledKingsparrow.DataBindings.Add("Checked", this, "EnabledKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkEnabledLighthouse.DataBindings.Add("Checked", this, "EnabledLighthouse", false, DataSourceUpdateMode.OnPropertyChanged);

            this.txtSpeedupIntro.DataBindings.Add("Text", this, "SpeedupIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupIntroEnd.DataBindings.Add("Text", this, "SpeedupIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPrison.DataBindings.Add("Text", this, "SpeedupPrison", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostSewers.DataBindings.Add("Text", this, "SpeedupPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupCampbell.DataBindings.Add("Text", this, "SpeedupCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostCampbell.DataBindings.Add("Text", this, "SpeedupPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupCat.DataBindings.Add("Text", this, "SpeedupCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostCat.DataBindings.Add("Text", this, "SpeedupPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostCat2.DataBindings.Add("Text", this, "SpeedupPostCat2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostCat3.DataBindings.Add("Text", this, "SpeedupPostCat3", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupBridge.DataBindings.Add("Text", this, "SpeedupBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostBridge.DataBindings.Add("Text", this, "SpeedupPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupBoyle.DataBindings.Add("Text", this, "SpeedupBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostBoyle.DataBindings.Add("Text", this, "SpeedupPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupTower.DataBindings.Add("Text", this, "SpeedupTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupPostTower.DataBindings.Add("Text", this, "SpeedupPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupFlooded.DataBindings.Add("Text", this, "SpeedupFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupFlooded2.DataBindings.Add("Text", this, "SpeedupFlooded2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupFloodedCell.DataBindings.Add("Text", this, "SpeedupFloodedCell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupKingsparrow.DataBindings.Add("Text", this, "SpeedupKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtSpeedupLighthouse.DataBindings.Add("Text", this, "SpeedupLighthouse", false, DataSourceUpdateMode.OnPropertyChanged);

            this.txtDelayIntro.DataBindings.Add("Text", this, "DelayIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayIntroEnd.DataBindings.Add("Text", this, "DelayIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPrison.DataBindings.Add("Text", this, "DelayPrison", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostSewers.DataBindings.Add("Text", this, "DelayPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayCampbell.DataBindings.Add("Text", this, "DelayCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostCampbell.DataBindings.Add("Text", this, "DelayPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayCat.DataBindings.Add("Text", this, "DelayCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostCat.DataBindings.Add("Text", this, "DelayPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostCat2.DataBindings.Add("Text", this, "DelayPostCat2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostCat3.DataBindings.Add("Text", this, "DelayPostCat3", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayBridge.DataBindings.Add("Text", this, "DelayBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostBridge.DataBindings.Add("Text", this, "DelayPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayBoyle.DataBindings.Add("Text", this, "DelayBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostBoyle.DataBindings.Add("Text", this, "DelayPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayTower.DataBindings.Add("Text", this, "DelayTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayPostTower.DataBindings.Add("Text", this, "DelayPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayFlooded.DataBindings.Add("Text", this, "DelayFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayFlooded2.DataBindings.Add("Text", this, "DelayFlooded2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayFloodedCell.DataBindings.Add("Text", this, "DelayFloodedCell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayKingsparrow.DataBindings.Add("Text", this, "DelayKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDelayLighthouse.DataBindings.Add("Text", this, "DelayLighthouse", false, DataSourceUpdateMode.OnPropertyChanged);

            this.txtXIntro.DataBindings.Add("Text", this, "XIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXIntroEnd.DataBindings.Add("Text", this, "XIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXPrison.DataBindings.Add("Text", this, "XPrison", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXPostSewers.DataBindings.Add("Text", this, "XPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXCampbell.DataBindings.Add("Text", this, "XCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXPostCampbell.DataBindings.Add("Text", this, "XPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXCat.DataBindings.Add("Text", this, "XCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXPostCat.DataBindings.Add("Text", this, "XPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXBridge.DataBindings.Add("Text", this, "XBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXPostBridge.DataBindings.Add("Text", this, "XPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXBoyle.DataBindings.Add("Text", this, "XBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXPostBoyle.DataBindings.Add("Text", this, "XPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXTower.DataBindings.Add("Text", this, "XTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXPostTower.DataBindings.Add("Text", this, "XPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXFlooded.DataBindings.Add("Text", this, "XFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXFloodedCell.DataBindings.Add("Text", this, "XFloodedCell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXKingsparrow.DataBindings.Add("Text", this, "XKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtXLighthouse.DataBindings.Add("Text", this, "XLighthouse", false, DataSourceUpdateMode.OnPropertyChanged);

            this.txtYIntro.DataBindings.Add("Text", this, "YIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYIntroEnd.DataBindings.Add("Text", this, "YIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYPrison.DataBindings.Add("Text", this, "YPrison", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYPostSewers.DataBindings.Add("Text", this, "YPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYCampbell.DataBindings.Add("Text", this, "YCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYPostCampbell.DataBindings.Add("Text", this, "YPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYCat.DataBindings.Add("Text", this, "YCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYPostCat.DataBindings.Add("Text", this, "YPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYBridge.DataBindings.Add("Text", this, "YBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYPostBridge.DataBindings.Add("Text", this, "YPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYBoyle.DataBindings.Add("Text", this, "YBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYPostBoyle.DataBindings.Add("Text", this, "YPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYTower.DataBindings.Add("Text", this, "YTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYPostTower.DataBindings.Add("Text", this, "YPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYFlooded.DataBindings.Add("Text", this, "YFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYFloodedCell.DataBindings.Add("Text", this, "YFloodedCell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYKingsparrow.DataBindings.Add("Text", this, "YKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtYLighthouse.DataBindings.Add("Text", this, "YLighthouse", false, DataSourceUpdateMode.OnPropertyChanged);

            this.txtZIntro.DataBindings.Add("Text", this, "ZIntro", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZIntroEnd.DataBindings.Add("Text", this, "ZIntroEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZPrison.DataBindings.Add("Text", this, "ZPrison", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZPostSewers.DataBindings.Add("Text", this, "ZPostSewers", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZCampbell.DataBindings.Add("Text", this, "ZCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZPostCampbell.DataBindings.Add("Text", this, "ZPostCampbell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZCat.DataBindings.Add("Text", this, "ZCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZPostCat.DataBindings.Add("Text", this, "ZPostCat", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZBridge.DataBindings.Add("Text", this, "ZBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZPostBridge.DataBindings.Add("Text", this, "ZPostBridge", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZBoyle.DataBindings.Add("Text", this, "ZBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZPostBoyle.DataBindings.Add("Text", this, "ZPostBoyle", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZTower.DataBindings.Add("Text", this, "ZTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZPostTower.DataBindings.Add("Text", this, "ZPostTower", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZFlooded.DataBindings.Add("Text", this, "ZFlooded", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZFloodedCell.DataBindings.Add("Text", this, "ZFloodedCell", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZKingsparrow.DataBindings.Add("Text", this, "ZKingsparrow", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtZLighthouse.DataBindings.Add("Text", this, "ZLighthouse", false, DataSourceUpdateMode.OnPropertyChanged);

            // defaults
            this.AutoStartEnd = true;

            this.EnabledIntro = true;
            this.EnabledIntroEnd = true;
            this.EnabledPrison = true;
            this.EnabledPostSewers = true;
            this.EnabledCampbell = true;
            this.EnabledPostCampbell = true;
            this.EnabledCat = true;
            this.EnabledPostCat = true;
            this.EnabledPostCat2 = true;
            this.EnabledPostCat3 = true;
            this.EnabledBridge = true;
            this.EnabledPostBridge = true;
            this.EnabledBoyle = true;
            this.EnabledPostBoyle = true;
            this.EnabledTower = true;
            this.EnabledPostTower = true;
            this.EnabledFlooded = true;
            this.EnabledFlooded2 = true;
            this.EnabledFloodedCell = true;
            this.EnabledKingsparrow = true;
            this.EnabledLighthouse  = true;

            this.SpeedupIntro = 9500;
            this.SpeedupIntroEnd = 6350;
            this.SpeedupPrison = 770;
            this.SpeedupPostSewers = 5640;
            this.SpeedupCampbell = 4450;
            this.SpeedupPostCampbell = 2050;
            this.SpeedupCat = 4550;
            this.SpeedupPostCat = 4700;
            this.SpeedupPostCat2 = 1500;
            this.SpeedupPostCat3 = 900;
            this.SpeedupBridge = 4040;
            this.SpeedupPostBridge = 4000;
            this.SpeedupBoyle = 2400;
            this.SpeedupPostBoyle = 3130;
            this.SpeedupTower = 5250;
            this.SpeedupPostTower = 3340;
            this.SpeedupFlooded = 5000;
            this.SpeedupFlooded2 = 6700;
            this.SpeedupFloodedCell = 1600;
            this.SpeedupKingsparrow = 4550;
            this.SpeedupLighthouse = 1100;

            this.DelayIntro = 1500;
            this.DelayIntroEnd = 500;
            this.DelayPrison = 200;
            this.DelayPostSewers = 1100;
            this.DelayCampbell = 1500;
            this.DelayPostCampbell = 1500;
            this.DelayCat = 2000;
            this.DelayPostCat = 1000;
            this.DelayPostCat2 = 9200;
            this.DelayPostCat3 = 1200;
            this.DelayBridge = 1100;
            this.DelayPostBridge = 1000;
            this.DelayBoyle = 1500;
            this.DelayPostBoyle = 700;
            this.DelayTower = 1500;
            this.DelayPostTower = 1100;
            this.DelayFlooded = 1500;
            this.DelayFlooded2 = 8000;
            this.DelayFloodedCell = 1500;
            this.DelayKingsparrow = 1500;
            this.DelayLighthouse = 1000;

            this.XIntro = 0;
            this.XIntroEnd = 0;
            this.XPrison = 0;
            this.XPostSewers = 0;
            this.XCampbell = 12809f;
            this.XPostCampbell = 0;
            this.XCat = 0;
            this.XPostCat = -11185f;
            this.XBridge = 0;
            this.XPostBridge = 0;
            this.XBoyle = 0;
            this.XPostBoyle = 0;
            this.XTower = 0;
            this.XPostTower = 0;
            this.XFlooded = -23249f;
            this.XFloodedCell = 0;
            this.XKingsparrow = 0;
            this.XLighthouse = 0;

            this.YIntro = 0;
            this.YIntroEnd = 0;
            this.YPrison = 0;
            this.YPostSewers = 0;
            this.YCampbell = 0;
            this.YPostCampbell = 0;
            this.YCat = 0;
            this.YPostCat = 0;
            this.YBridge = 0;
            this.YPostBridge = 0;
            this.YBoyle = 0;
            this.YPostBoyle = 0;
            this.YTower = 0;
            this.YPostTower = 0;
            this.YFlooded = 0;
            this.YFloodedCell = 0;
            this.YKingsparrow = 0;
            this.YLighthouse = 0;

            this.ZIntro = 0;
            this.ZIntroEnd = 0;
            this.ZPrison = 0;
            this.ZPostSewers = 0;
            this.ZCampbell = 0;
            this.ZPostCampbell = 0;
            this.ZCat = 0;
            this.ZPostCat = 0;
            this.ZBridge = 0;
            this.ZPostBridge = 0;
            this.ZBoyle = 0;
            this.ZPostBoyle = 0;
            this.ZTower = 0;
            this.ZPostTower = 0;
            this.ZFlooded = 0;
            this.ZFloodedCell = 0;
            this.ZKingsparrow = 0;
            this.ZLighthouse = 0;
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

            settingsNode.AppendChild(ToElement(doc, "EnabledIntro", this.EnabledIntro));
            settingsNode.AppendChild(ToElement(doc, "EnabledIntroEnd", this.EnabledIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "EnabledPrison", this.EnabledPrison));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostSewers", this.EnabledPostSewers));
            settingsNode.AppendChild(ToElement(doc, "EnabledCampbell", this.EnabledCampbell));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostCampbell", this.EnabledPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "EnabledCat", this.EnabledCat));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostCat", this.EnabledPostCat));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostCat2", this.EnabledPostCat2));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostCat3", this.EnabledPostCat3));
            settingsNode.AppendChild(ToElement(doc, "EnabledBridge", this.EnabledBridge));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostBridge", this.EnabledPostBridge));
            settingsNode.AppendChild(ToElement(doc, "EnabledBoyle", this.EnabledBoyle));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostBoyle", this.EnabledPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "EnabledTower", this.EnabledTower));
            settingsNode.AppendChild(ToElement(doc, "EnabledPostTower", this.EnabledPostTower));
            settingsNode.AppendChild(ToElement(doc, "EnabledFlooded", this.EnabledFlooded));
            settingsNode.AppendChild(ToElement(doc, "EnabledFlooded2", this.EnabledFlooded2));
            settingsNode.AppendChild(ToElement(doc, "EnabledFloodedCell", this.EnabledFloodedCell));
            settingsNode.AppendChild(ToElement(doc, "EnabledKingsparrow", this.EnabledKingsparrow));
            settingsNode.AppendChild(ToElement(doc, "EnabledLighthouse", this.EnabledLighthouse));

            settingsNode.AppendChild(ToElement(doc, "SpeedupIntro", this.SpeedupIntro));
            settingsNode.AppendChild(ToElement(doc, "SpeedupIntroEnd", this.SpeedupIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPrison", this.SpeedupPrison));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostSewers", this.SpeedupPostSewers));
            settingsNode.AppendChild(ToElement(doc, "SpeedupCampbell", this.SpeedupCampbell));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostCampbell", this.SpeedupPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "SpeedupCat", this.SpeedupCat));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostCat", this.SpeedupPostCat));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostCat2", this.SpeedupPostCat2));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostCat3", this.SpeedupPostCat3));
            settingsNode.AppendChild(ToElement(doc, "SpeedupBridge", this.SpeedupBridge));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostBridge", this.SpeedupPostBridge));
            settingsNode.AppendChild(ToElement(doc, "SpeedupBoyle", this.SpeedupBoyle));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostBoyle", this.SpeedupPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "SpeedupTower", this.SpeedupTower));
            settingsNode.AppendChild(ToElement(doc, "SpeedupPostTower", this.SpeedupPostTower));
            settingsNode.AppendChild(ToElement(doc, "SpeedupFlooded", this.SpeedupFlooded));
            settingsNode.AppendChild(ToElement(doc, "SpeedupFlooded2", this.SpeedupFlooded2));
            settingsNode.AppendChild(ToElement(doc, "SpeedupFloodedCell", this.SpeedupFloodedCell));
            settingsNode.AppendChild(ToElement(doc, "SpeedupKingsparrow", this.SpeedupKingsparrow));
            settingsNode.AppendChild(ToElement(doc, "SpeedupLighthouse", this.SpeedupLighthouse));

            settingsNode.AppendChild(ToElement(doc, "DelayIntro", this.DelayIntro));
            settingsNode.AppendChild(ToElement(doc, "DelayIntroEnd", this.DelayIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "DelayPrison", this.DelayPrison));
            settingsNode.AppendChild(ToElement(doc, "DelayPostSewers", this.DelayPostSewers));
            settingsNode.AppendChild(ToElement(doc, "DelayCampbell", this.DelayCampbell));
            settingsNode.AppendChild(ToElement(doc, "DelayPostCampbell", this.DelayPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "DelayCat", this.DelayCat));
            settingsNode.AppendChild(ToElement(doc, "DelayPostCat", this.DelayPostCat));
            settingsNode.AppendChild(ToElement(doc, "DelayPostCat2", this.DelayPostCat2));
            settingsNode.AppendChild(ToElement(doc, "DelayPostCat3", this.DelayPostCat3));
            settingsNode.AppendChild(ToElement(doc, "DelayBridge", this.DelayBridge));
            settingsNode.AppendChild(ToElement(doc, "DelayPostBridge", this.DelayPostBridge));
            settingsNode.AppendChild(ToElement(doc, "DelayBoyle", this.DelayBoyle));
            settingsNode.AppendChild(ToElement(doc, "DelayPostBoyle", this.DelayPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "DelayTower", this.DelayTower));
            settingsNode.AppendChild(ToElement(doc, "DelayPostTower", this.DelayPostTower));
            settingsNode.AppendChild(ToElement(doc, "DelayFlooded", this.DelayFlooded));
            settingsNode.AppendChild(ToElement(doc, "DelayFlooded2", this.DelayFlooded2));
            settingsNode.AppendChild(ToElement(doc, "DelayFloodedCell", this.DelayFloodedCell));
            settingsNode.AppendChild(ToElement(doc, "DelayKingsparrow", this.DelayKingsparrow));
            settingsNode.AppendChild(ToElement(doc, "DelayLighthouse", this.DelayLighthouse));

            settingsNode.AppendChild(ToElement(doc, "XIntro", this.XIntro));
            settingsNode.AppendChild(ToElement(doc, "XIntroEnd", this.XIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "XPrison", this.XPrison));
            settingsNode.AppendChild(ToElement(doc, "XPostSewers", this.XPostSewers));
            settingsNode.AppendChild(ToElement(doc, "XCampbell", this.XCampbell));
            settingsNode.AppendChild(ToElement(doc, "XPostCampbell", this.XPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "XCat", this.XCat));
            settingsNode.AppendChild(ToElement(doc, "XPostCat", this.XPostCat));
            settingsNode.AppendChild(ToElement(doc, "XBridge", this.XBridge));
            settingsNode.AppendChild(ToElement(doc, "XPostBridge", this.XPostBridge));
            settingsNode.AppendChild(ToElement(doc, "XBoyle", this.XBoyle));
            settingsNode.AppendChild(ToElement(doc, "XPostBoyle", this.XPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "XTower", this.XTower));
            settingsNode.AppendChild(ToElement(doc, "XPostTower", this.XPostTower));
            settingsNode.AppendChild(ToElement(doc, "XFlooded", this.XFlooded));
            settingsNode.AppendChild(ToElement(doc, "XFloodedCell", this.XFloodedCell));
            settingsNode.AppendChild(ToElement(doc, "XKingsparrow", this.XKingsparrow));
            settingsNode.AppendChild(ToElement(doc, "XLighthouse", this.XLighthouse));

            settingsNode.AppendChild(ToElement(doc, "YIntro", this.YIntro));
            settingsNode.AppendChild(ToElement(doc, "YIntroEnd", this.YIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "YPrison", this.YPrison));
            settingsNode.AppendChild(ToElement(doc, "YPostSewers", this.YPostSewers));
            settingsNode.AppendChild(ToElement(doc, "YCampbell", this.YCampbell));
            settingsNode.AppendChild(ToElement(doc, "YPostCampbell", this.YPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "YCat", this.YCat));
            settingsNode.AppendChild(ToElement(doc, "YPostCat", this.YPostCat));
            settingsNode.AppendChild(ToElement(doc, "YBridge", this.YBridge));
            settingsNode.AppendChild(ToElement(doc, "YPostBridge", this.YPostBridge));
            settingsNode.AppendChild(ToElement(doc, "YBoyle", this.YBoyle));
            settingsNode.AppendChild(ToElement(doc, "YPostBoyle", this.YPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "YTower", this.YTower));
            settingsNode.AppendChild(ToElement(doc, "YPostTower", this.YPostTower));
            settingsNode.AppendChild(ToElement(doc, "YFlooded", this.YFlooded));
            settingsNode.AppendChild(ToElement(doc, "YFloodedCell", this.YFloodedCell));
            settingsNode.AppendChild(ToElement(doc, "YKingsparrow", this.YKingsparrow));
            settingsNode.AppendChild(ToElement(doc, "YLighthouse", this.YLighthouse));

            settingsNode.AppendChild(ToElement(doc, "ZIntro", this.ZIntro));
            settingsNode.AppendChild(ToElement(doc, "ZIntroEnd", this.ZIntroEnd));
            settingsNode.AppendChild(ToElement(doc, "ZPrison", this.ZPrison));
            settingsNode.AppendChild(ToElement(doc, "ZPostSewers", this.ZPostSewers));
            settingsNode.AppendChild(ToElement(doc, "ZCampbell", this.ZCampbell));
            settingsNode.AppendChild(ToElement(doc, "ZPostCampbell", this.ZPostCampbell));
            settingsNode.AppendChild(ToElement(doc, "ZCat", this.ZCat));
            settingsNode.AppendChild(ToElement(doc, "ZPostCat", this.ZPostCat));
            settingsNode.AppendChild(ToElement(doc, "ZBridge", this.ZBridge));
            settingsNode.AppendChild(ToElement(doc, "ZPostBridge", this.ZPostBridge));
            settingsNode.AppendChild(ToElement(doc, "ZBoyle", this.ZBoyle));
            settingsNode.AppendChild(ToElement(doc, "ZPostBoyle", this.ZPostBoyle));
            settingsNode.AppendChild(ToElement(doc, "ZTower", this.ZTower));
            settingsNode.AppendChild(ToElement(doc, "ZPostTower", this.ZPostTower));
            settingsNode.AppendChild(ToElement(doc, "ZFlooded", this.ZFlooded));
            settingsNode.AppendChild(ToElement(doc, "ZFloodedCell", this.ZFloodedCell));
            settingsNode.AppendChild(ToElement(doc, "ZKingsparrow", this.ZKingsparrow));
            settingsNode.AppendChild(ToElement(doc, "ZLighthouse", this.ZLighthouse));

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

            this.EnabledIntro = ParseBool(settings, "EnabledIntro", true);
            this.EnabledIntroEnd = ParseBool(settings, "EnabledIntroEnd", true);
            this.EnabledPrison = ParseBool(settings, "EnabledPrison", true);
            this.EnabledPostSewers = ParseBool(settings, "EnabledPostSewers", true);
            this.EnabledCampbell = ParseBool(settings, "EnabledCampbell", true);
            this.EnabledPostCampbell = ParseBool(settings, "EnabledPostCampbell", true);
            this.EnabledCat = ParseBool(settings, "EnabledCat", true);
            this.EnabledPostCat = ParseBool(settings, "EnabledPostCat", true);
            this.EnabledPostCat2 = ParseBool(settings, "EnabledPostCat2", true);
            this.EnabledPostCat3 = ParseBool(settings, "EnabledPostCat3", true);
            this.EnabledBridge = ParseBool(settings, "EnabledBridge", true);
            this.EnabledPostBridge = ParseBool(settings, "EnabledPostBridge", true);
            this.EnabledBoyle = ParseBool(settings, "EnabledBoyle", true);
            this.EnabledPostBoyle = ParseBool(settings, "EnabledPostBoyle", true);
            this.EnabledTower = ParseBool(settings, "EnabledTower", true);
            this.EnabledPostTower = ParseBool(settings, "EnabledPostTower", true);
            this.EnabledFlooded = ParseBool(settings, "EnabledFlooded", true);
            this.EnabledFlooded2 = ParseBool(settings, "EnabledFlooded2", true);
            this.EnabledFloodedCell = ParseBool(settings, "EnabledFloodedCell", true);
            this.EnabledKingsparrow = ParseBool(settings, "EnabledKingsparrow", true);
            this.EnabledLighthouse = ParseBool(settings, "EnabledLighthouse", true);

            this.SpeedupIntro = ParseInt(settings, "SpeedupIntro", 9500);
            this.SpeedupIntroEnd = ParseInt(settings, "SpeedupIntroEnd", 6350);
            this.SpeedupPrison = ParseInt(settings, "SpeedupPrison", 770);
            this.SpeedupPostSewers = ParseInt(settings, "SpeedupPostSewers", 5640);
            this.SpeedupCampbell = ParseInt(settings, "SpeedupCampbell", 4450);
            this.SpeedupPostCampbell = ParseInt(settings, "SpeedupPostCampbell", 2050);
            this.SpeedupCat = ParseInt(settings, "SpeedupCat", 4550);
            this.SpeedupPostCat = ParseInt(settings, "SpeedupPostCat", 4700);
            this.SpeedupPostCat2 = ParseInt(settings, "SpeedupPostCat2", 1500);
            this.SpeedupPostCat3 = ParseInt(settings, "SpeedupPostCat3", 900);
            this.SpeedupBridge = ParseInt(settings, "SpeedupBridge", 4040);
            this.SpeedupPostBridge = ParseInt(settings, "SpeedupPostBridge", 4000);
            this.SpeedupBoyle = ParseInt(settings, "SpeedupBoyle", 2400);
            this.SpeedupPostBoyle = ParseInt(settings, "SpeedupPostBoyle", 3130);
            this.SpeedupTower = ParseInt(settings, "SpeedupTower", 5250);
            this.SpeedupPostTower = ParseInt(settings, "SpeedupPostTower", 3340);
            this.SpeedupFlooded = ParseInt(settings, "SpeedupFlooded", 5000);
            this.SpeedupFlooded2 = ParseInt(settings, "SpeedupFlooded2", 6700);
            this.SpeedupFloodedCell = ParseInt(settings, "SpeedupFloodedCell", 1600);
            this.SpeedupKingsparrow = ParseInt(settings, "SpeedupKingsparrow", 4550);
            this.SpeedupLighthouse = ParseInt(settings, "SpeedupLighthouse", 1100);

            this.DelayIntro = ParseInt(settings, "DelayIntro", 1500);
            this.DelayIntroEnd = ParseInt(settings, "DelayIntroEnd", 500);
            this.DelayPrison = ParseInt(settings, "DelayPrison", 200);
            this.DelayPostSewers = ParseInt(settings, "DelayPostSewers", 1100);
            this.DelayCampbell = ParseInt(settings, "DelayCampbell", 1500);
            this.DelayPostCampbell = ParseInt(settings, "DelayPostCampbell", 1500);
            this.DelayCat = ParseInt(settings, "DelayCat", 2000);
            this.DelayPostCat = ParseInt(settings, "DelayPostCat", 1000);
            this.DelayPostCat2 = ParseInt(settings, "DelayPostCat2", 9200);
            this.DelayPostCat3 = ParseInt(settings, "DelayPostCat3", 1200);
            this.DelayBridge = ParseInt(settings, "DelayBridge", 1100);
            this.DelayPostBridge = ParseInt(settings, "DelayPostBridge", 1000);
            this.DelayBoyle = ParseInt(settings, "DelayBoyle", 1500);
            this.DelayPostBoyle = ParseInt(settings, "DelayPostBoyle", 700);
            this.DelayTower = ParseInt(settings, "DelayTower", 1500);
            this.DelayPostTower = ParseInt(settings, "DelayPostTower", 1100);
            this.DelayFlooded = ParseInt(settings, "DelayFlooded", 1500);
            this.DelayFlooded2 = ParseInt(settings, "DelayFlooded2", 8000);
            this.DelayFloodedCell = ParseInt(settings, "DelayFloodedCell", 1500);
            this.DelayKingsparrow = ParseInt(settings, "DelayKingsparrow", 1500);
            this.DelayLighthouse = ParseInt(settings, "DelayLighthouse", 1000);

            this.XIntro = ParseFloat(settings, "XIntro");
            this.XIntroEnd = ParseFloat(settings, "XIntroEnd");
            this.XPrison = ParseFloat(settings, "XPrison");
            this.XPostSewers = ParseFloat(settings, "XPostSewers");
            this.XCampbell = ParseFloat(settings, "XCampbell", 12809f);
            this.XPostCampbell = ParseFloat(settings, "XPostCampbell");
            this.XCat = ParseFloat(settings, "XCat");
            this.XPostCat = ParseFloat(settings, "XPostCat", -11185f);
            this.XBridge = ParseFloat(settings, "XBridge");
            this.XPostBridge = ParseFloat(settings, "XPostBridge");
            this.XBoyle = ParseFloat(settings, "XBoyle");
            this.XPostBoyle = ParseFloat(settings, "XPostBoyle");
            this.XTower = ParseFloat(settings, "XTower");
            this.XPostTower = ParseFloat(settings, "XPostTower");
            this.XFlooded = ParseFloat(settings, "XFlooded", -23249f);
            this.XFloodedCell = ParseFloat(settings, "XFloodedCell");
            this.XKingsparrow = ParseFloat(settings, "XKingsparrow");
            this.XLighthouse = ParseFloat(settings, "XLighthouse");

            this.YIntro = ParseFloat(settings, "YIntro");
            this.YIntroEnd = ParseFloat(settings, "YIntroEnd");
            this.YPrison = ParseFloat(settings, "YPrison");
            this.YPostSewers = ParseFloat(settings, "YPostSewers");
            this.YCampbell = ParseFloat(settings, "YCampbell");
            this.YPostCampbell = ParseFloat(settings, "YPostCampbell");
            this.YCat = ParseFloat(settings, "YCat");
            this.YPostCat = ParseFloat(settings, "YPostCat");
            this.YBridge = ParseFloat(settings, "YBridge");
            this.YPostBridge = ParseFloat(settings, "YPostBridge");
            this.YBoyle = ParseFloat(settings, "YBoyle");
            this.YPostBoyle = ParseFloat(settings, "YPostBoyle");
            this.YTower = ParseFloat(settings, "YTower");
            this.YPostTower = ParseFloat(settings, "YPostTower");
            this.YFlooded = ParseFloat(settings, "YFlooded");
            this.YFloodedCell = ParseFloat(settings, "YFloodedCell");
            this.YKingsparrow = ParseFloat(settings, "YKingsparrow");
            this.YLighthouse = ParseFloat(settings, "YLighthouse");

            this.ZIntro = ParseFloat(settings, "ZIntro");
            this.ZIntroEnd = ParseFloat(settings, "ZIntroEnd");
            this.ZPrison = ParseFloat(settings, "ZPrison");
            this.ZPostSewers = ParseFloat(settings, "ZPostSewers");
            this.ZCampbell = ParseFloat(settings, "ZCampbell");
            this.ZPostCampbell = ParseFloat(settings, "ZPostCampbell");
            this.ZCat = ParseFloat(settings, "ZCat");
            this.ZPostCat = ParseFloat(settings, "ZPostCat");
            this.ZBridge = ParseFloat(settings, "ZBridge");
            this.ZPostBridge = ParseFloat(settings, "ZPostBridge");
            this.ZBoyle = ParseFloat(settings, "ZBoyle");
            this.ZPostBoyle = ParseFloat(settings, "ZPostBoyle");
            this.ZTower = ParseFloat(settings, "ZTower");
            this.ZPostTower = ParseFloat(settings, "ZPostTower");
            this.ZFlooded = ParseFloat(settings, "ZFlooded");
            this.ZFloodedCell = ParseFloat(settings, "ZFloodedCell");
            this.ZKingsparrow = ParseFloat(settings, "ZKingsparrow");
            this.ZLighthouse = ParseFloat(settings, "ZLighthouse");
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

        static float ParseFloat(XmlNode settings, string setting, float default_ = 0)
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
