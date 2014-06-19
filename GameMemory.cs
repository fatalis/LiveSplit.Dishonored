using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.Dishonored
{
    class GameMemory
    {
        public enum AreaCompletionType
        {
            None,
            IntroEnd,
            MissionEnd,
            PrisonEscape,
            OutsidersDream,
            Weepers
        }

        public event EventHandler OnFirstLevelLoading;
        public event EventHandler OnPlayerGainedControl;
        public event EventHandler OnLoadStarted;
        public event EventHandler OnLoadFinished;
        public event EventHandler OnPlayerLostControl;
        public delegate void AreaCompletedEventHandler(object sender, AreaCompletionType type);
        public event AreaCompletedEventHandler OnAreaCompleted;

        private Task _thread;
        private CancellationTokenSource _cancelSource;
        private SynchronizationContext _uiThread;

        private DeepPointer _currentLevelPtr;
        private DeepPointer _isLoadingPtr;
        private DeepPointer _currentBikMoviePtr;
        private DeepPointer _cutsceneActivePtr;
        private DeepPointer _playerPosPtr;
        private DeepPointer _missionStatsScreenFlagsPtr;

        private enum ExpectedDllSizes
        {
            DishonoredExe12Reloaded = 18219008,
            BinkW32Dll = 241664
        }

        private Dictionary<string, AreaCompletionType> _areaCompletions = new Dictionary<string, AreaCompletionType>
        {
            {"LoadingSewers|L_Prison_",    AreaCompletionType.PrisonEscape},
            {"LoadingStreets|L_Pub_Dusk_", AreaCompletionType.OutsidersDream},
            {"LoadingStreets|L_Pub_Day_",  AreaCompletionType.Weepers},
        };

        public GameMemory()
        {
            _currentLevelPtr = new DeepPointer(0xFB7838, 0x2c0, 0x314, 0, 0x38);
            _isLoadingPtr = new DeepPointer("binkw32.dll", 0x312F4);
            _currentBikMoviePtr = new DeepPointer(0xFC6AD4, 0x48, 0);
            _cutsceneActivePtr = new DeepPointer(0xFB51CC, 0x744);
            _playerPosPtr = new DeepPointer(0xFCCBDC, 0xC4);
            _missionStatsScreenFlagsPtr = new DeepPointer(0xFDEB08, 0x24, 0x41C, 0x2E0, 0xC4);
        }

        public void StartMonitoring()
        {
            if (_thread != null && _thread.Status == TaskStatus.Running)
                throw new InvalidOperationException();
            if (!(SynchronizationContext.Current is WindowsFormsSynchronizationContext))
                throw new InvalidOperationException("SynchronizationContext.Current is not a UI thread.");

            _uiThread = SynchronizationContext.Current;
            _cancelSource = new CancellationTokenSource();
            _thread = Task.Factory.StartNew(MemoryReadThread);
        }

        public void Stop()
        {
            if (_cancelSource == null || _thread == null)
                throw new InvalidOperationException();

            if (_thread.Status != TaskStatus.Running)
                return;

            _cancelSource.Cancel();
            _thread.Wait();
        }

        void MemoryReadThread()
        {
            Trace.WriteLine("[NoLoads] MemoryReadThread");

            while (!_cancelSource.IsCancellationRequested)
            {
                try
                {
                    Trace.WriteLine("[NoLoads] Waiting for dishonored.exe...");

                    Process game;
                    var ignorePIDs = new List<int>();
                    while (true)
                    {
                        game = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.ToLower() == "dishonored"
                            && !p.HasExited && !ignorePIDs.Contains(p.Id));

                        if (game != null)
                        {
                            ProcessModule binkw32 = game.Modules.Cast<ProcessModule>().FirstOrDefault(p => p.ModuleName.ToLower() == "binkw32.dll");
                            if (binkw32 != null)
                            {
                                if (game.MainModule.ModuleMemorySize != (int)ExpectedDllSizes.DishonoredExe12Reloaded)
                                {
                                    ignorePIDs.Add(game.Id);
                                    _uiThread.Send(d => MessageBox.Show("Unexpected game version. Dishonored 1.2 is required.", "LiveSplit.Dishonored",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error), null);
                                }
                                else if (binkw32.ModuleMemorySize != (int) ExpectedDllSizes.BinkW32Dll)
                                {
                                    ignorePIDs.Add(game.Id);
                                    _uiThread.Send(d => MessageBox.Show("binkw32.dll was not the expected version.", "LiveSplit.Dishonored",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error), null);
                                }
                                else
                                    break;
                            }
                        }

                        Thread.Sleep(250);

                        if (_cancelSource.IsCancellationRequested)
                            return;
                    }

                    Trace.WriteLine("[NoLoads] Got dishonored.exe!");

                    int prevCurrentLevel = 0;
                    bool prevIsLoading = false;
                    bool prevCutsceneActive = false;
                    bool prevMissionStatsScreenActive = false;
                    uint frameCounter = 0;
                    string prevCurrentMovie = String.Empty;
                    bool loadingStarted = false;
                    while (!game.HasExited)
                    {
                        int currentLevel;
                        _currentLevelPtr.Deref(game, out currentLevel);
                        string currentLevelStr = GetEngineStringByID(game, currentLevel);

                        bool isLoading;
                        _isLoadingPtr.Deref(game, out isLoading);

                        bool cutsceneActive;
                        _cutsceneActivePtr.Deref(game, out cutsceneActive);

                        Vector3f playerPos;
                        _playerPosPtr.Deref(game, out playerPos);

                        int missionStatsScreenFlags;
                        _missionStatsScreenFlagsPtr.Deref(game, out missionStatsScreenFlags);
                        bool missionStatsScreenActive = (missionStatsScreenFlags & 1) != 0;

                        string currentMovie;
                        _currentBikMoviePtr.Deref(game, out currentMovie, 64);

                        if (currentMovie != prevCurrentMovie && prevCurrentMovie != String.Empty)
                        {
                            Trace.WriteLine(String.Format("{0} [NoLoads] Movie Changed - {1} -> {2}", frameCounter, prevCurrentMovie, currentMovie));

                            // special case for Intro End split because two movies play back-to-back
                            // which can cause isLoading to not detect changes
                            if (currentMovie == "LoadingPrison" && prevCurrentMovie == "Dishonored")
                            {
                                loadingStarted = true;

                                _uiThread.Post(d => {
                                    if (this.OnLoadStarted != null)
                                        this.OnLoadStarted(this, EventArgs.Empty);
                                }, null);

                                _uiThread.Post(d => {
                                    if (this.OnAreaCompleted != null)
                                        this.OnAreaCompleted(this, AreaCompletionType.IntroEnd);
                                }, null);
                            }
                        }

                        if (currentLevel != prevCurrentLevel)
                        {
                            Trace.WriteLine(String.Format("{0} [NoLoads] Level Changed - {1} -> {2} '{3}'", frameCounter, prevCurrentLevel, currentLevel, currentLevelStr));

                            if (currentLevelStr == "l_tower_p")
                            {
                                _uiThread.Post(d => {
                                    if (this.OnFirstLevelLoading != null)
                                        this.OnFirstLevelLoading(this, EventArgs.Empty);
                                }, null);
                            }
                        }

                        if (isLoading != prevIsLoading)
                        {
                            if (isLoading)
                            {
                                Trace.WriteLine(String.Format("{0} [NoLoads] Load Start - {1} - Pos={2}", frameCounter, currentMovie + "|" + currentLevelStr, playerPos));

                                // ignore the beginning load screen and the dishonored logo screen
                                if (currentMovie != "LoadingEmpressTower" && currentMovie != "Dishonored" && currentMovie != "INTRO_LOC")
                                {
                                    // ignore intro end if it happens, see special case above
                                    if (!(currentMovie == "LoadingPrison" && currentLevelStr.ToLower().StartsWith("L_Tower_")))
                                    {
                                        loadingStarted = true;

                                        _uiThread.Post(d => {
                                            if (this.OnLoadStarted != null)
                                                this.OnLoadStarted(this, EventArgs.Empty);
                                        }, null);
                                    }
                                }

                                AreaCompletionType completionType = _areaCompletions.Where(c => (currentMovie.ToLower() + "|" + currentLevelStr.ToLower()).StartsWith(c.Key.ToLower())).Select(c => c.Value).FirstOrDefault();
                                if (completionType != AreaCompletionType.None)
                                {
                                    _uiThread.Post(d => {
                                        if (this.OnAreaCompleted != null)
                                            this.OnAreaCompleted(this, completionType);
                                    }, null);
                                }
                            }
                            else
                            {
                                Trace.WriteLine(String.Format("{0} [NoLoads] Load End - {1}", frameCounter, currentMovie + "|" + currentLevelStr));

                                if (loadingStarted)
                                {
                                    loadingStarted = false;

                                    _uiThread.Post(d => {
                                        if (this.OnLoadFinished != null)
                                            this.OnLoadFinished(this, EventArgs.Empty);
                                    }, null);
                                }

                                if ((currentMovie == "LoadingEmpressTower" || currentMovie == "INTRO_LOC") && currentLevelStr == "l_tower_p")
                                {
                                    _uiThread.Post(d => {
                                        if (this.OnPlayerGainedControl != null)
                                            this.OnPlayerGainedControl(this, EventArgs.Empty);
                                    }, null);
                                }
                            }
                        }

                        if (cutsceneActive != prevCutsceneActive)
                        {
                            Trace.WriteLine(String.Format("{0} [NoLoads] In-Game Cutscene {1}", frameCounter, cutsceneActive ? "Start" : "End"));

                            if (cutsceneActive && currentLevelStr == "L_LightH_LowChaos_P")
                            {
                                _uiThread.Post(d => {
                                    if (this.OnPlayerLostControl != null)
                                        this.OnPlayerLostControl(this, EventArgs.Empty);
                                }, null);
                            }
                        }

                        if (missionStatsScreenActive != prevMissionStatsScreenActive && missionStatsScreenActive)
                        {
                            Trace.WriteLine(String.Format("{0} [NoLoads] Mission End", frameCounter));

                            _uiThread.Post(d => {
                                if (this.OnAreaCompleted != null)
                                    this.OnAreaCompleted(this, AreaCompletionType.MissionEnd);
                            }, null);
                        }

                        prevCurrentLevel = currentLevel;
                        prevIsLoading = isLoading;
                        prevCutsceneActive = cutsceneActive;
                        prevMissionStatsScreenActive = missionStatsScreenActive;
                        prevCurrentMovie = currentMovie;
                        frameCounter++;

                        Thread.Sleep(15);

                        if (_cancelSource.IsCancellationRequested)
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                    Thread.Sleep(1000);
                }
            }
        }

        string GetEngineStringByID(Process p, int id)
        {
            string str;
            var ptr = new DeepPointer(0xFA3624, (id*4), 0x10);
            ptr.Deref(p, out str, 32);
            return str;
        }
    }
}
