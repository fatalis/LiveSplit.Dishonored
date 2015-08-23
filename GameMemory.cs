using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveSplit.ComponentUtil;

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
        private List<int> _ignorePIDs; 

        private DeepPointer _currentLevelPtr;
        private DeepPointer _isLoadingPtr;
        private DeepPointer _currentBikMoviePtr;
        private DeepPointer _cutsceneActivePtr;
        private DeepPointer _missionStatsScreenFlagsPtr;
        private int _stringBase;

        private enum ExpectedDllSizes
        {
            DishonoredExe12 = 18219008,
            DishonoredExe14Reloaded = 18862080,
            DishonoredExe14Steam = 19427328,
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
            _isLoadingPtr = new DeepPointer("binkw32.dll", 0x312F4);
            _ignorePIDs = new List<int>();
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
            if (_cancelSource == null || _thread == null || _thread.Status != TaskStatus.Running)
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
                    while ((game = GetGameProcess()) == null)
                    {
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
                    bool oncePerLevelFlag = false;
                    while (!game.HasExited)
                    {
                        int currentLevel;
                        _currentLevelPtr.Deref(game, out currentLevel);
                        string currentLevelStr = GetEngineStringByID(game, currentLevel);

                        bool isLoading;
                        _isLoadingPtr.Deref(game, out isLoading);

                        bool cutsceneActive;
                        _cutsceneActivePtr.Deref(game, out cutsceneActive);

                        int missionStatsScreenFlags;
                        _missionStatsScreenFlagsPtr.Deref(game, out missionStatsScreenFlags);
                        bool missionStatsScreenActive = (missionStatsScreenFlags & 1) != 0;

                        string currentMovie;
                        _currentBikMoviePtr.Deref(game, out currentMovie, 64);

                        if (currentMovie != prevCurrentMovie && prevCurrentMovie != String.Empty)
                        {
                            Trace.WriteLine($"{frameCounter} [NoLoads] Movie Changed - {prevCurrentMovie} -> {currentMovie}");

                            // special case for Intro End split because two movies play back-to-back
                            // which can cause isLoading to not detect changes
                            if (currentMovie == "LoadingPrison" && prevCurrentMovie == "Dishonored")
                            {
                                loadingStarted = true;

                                _uiThread.Post(d => this.OnLoadStarted?.Invoke(this, EventArgs.Empty), null);
                                _uiThread.Post(d => this.OnAreaCompleted?.Invoke(this, AreaCompletionType.IntroEnd), null);
                            }
                        }

                        if (currentLevel != prevCurrentLevel)
                        {
                            Trace.WriteLine($"{frameCounter} [NoLoads] Level Changed - {prevCurrentLevel} -> {currentLevel} '{currentLevelStr}'");

                            if (currentLevelStr == "l_tower_p" || currentLevelStr == "L_DLC07_BaseIntro_P" || currentLevelStr == "DLC06_Tower_P")
                            {
                                _uiThread.Post(d => this.OnFirstLevelLoading?.Invoke(this, EventArgs.Empty), null);
                            }

                            oncePerLevelFlag = true;
                        }

                        if (isLoading != prevIsLoading)
                        {
                            if (isLoading)
                            {
                                Trace.WriteLine($"{frameCounter} [NoLoads] Load Start - {currentMovie + "|" + currentLevelStr}");

                                // ignore the beginning load screen and the dishonored logo screen
                                if (currentMovie != "LoadingEmpressTower" && currentMovie != "Dishonored" && currentMovie != "INTRO_LOC")
                                {
                                    // ignore intro end if it happens, see special case above
                                    if (!(currentMovie == "LoadingPrison" && currentLevelStr.ToLower().StartsWith("L_Tower_")))
                                    {
                                        loadingStarted = true;
                                        _uiThread.Post(d => this.OnLoadStarted?.Invoke(this, EventArgs.Empty), null);
                                    }
                                }

                                AreaCompletionType completionType = _areaCompletions.Where(c => (currentMovie.ToLower() + "|" + currentLevelStr.ToLower()).StartsWith(c.Key.ToLower())).Select(c => c.Value).FirstOrDefault();
                                if (completionType != AreaCompletionType.None)
                                {
                                    _uiThread.Post(d => this.OnAreaCompleted?.Invoke(this, completionType), null);
                                }
                            }
                            else
                            {
                                Trace.WriteLine($"{frameCounter} [NoLoads] Load End - {currentMovie + "|" + currentLevelStr}");

                                if (loadingStarted)
                                {
                                    loadingStarted = false;

                                    _uiThread.Post(d => this.OnLoadFinished?.Invoke(this, EventArgs.Empty), null);
                                }

                                if (((currentMovie == "LoadingEmpressTower" || currentMovie == "INTRO_LOC") && currentLevelStr == "l_tower_p")
                                    || (currentMovie == "Loading" || currentMovie == "LoadingDLC06Tower") && currentLevelStr == "DLC06_Tower_P") // KoD
                                {
                                    _uiThread.Post(d => this.OnPlayerGainedControl?.Invoke(this, EventArgs.Empty), null);
                                }
                            }
                        }

                        if (cutsceneActive != prevCutsceneActive)
                        {
                            Trace.WriteLine($"{frameCounter} [NoLoads] In-Game Cutscene {(cutsceneActive ? "Start" : "End")}");

                            if (cutsceneActive && currentLevelStr == "L_LightH_LowChaos_P")
                            {
                                _uiThread.Post(d => this.OnPlayerLostControl?.Invoke(this, EventArgs.Empty), null);
                            }
                            else if (!cutsceneActive && currentLevelStr == "L_DLC07_BaseIntro_P" && oncePerLevelFlag)
                            {
                                oncePerLevelFlag = false;
                                _uiThread.Post(d => this.OnPlayerGainedControl?.Invoke(this, EventArgs.Empty), null);
                            }
                        }

                        if (missionStatsScreenActive != prevMissionStatsScreenActive && missionStatsScreenActive)
                        {
                            Trace.WriteLine($"{frameCounter} [NoLoads] Mission End");
                            _uiThread.Post(d => this.OnAreaCompleted?.Invoke(this, AreaCompletionType.MissionEnd), null);
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

        Process GetGameProcess()
        {
            Process game = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.ToLower() == "dishonored"
                && !p.HasExited && !_ignorePIDs.Contains(p.Id));
            if (game == null)
                return null;

            ProcessModuleWow64Safe binkw32 = game.ModulesWow64Safe().FirstOrDefault(p => p.ModuleName.ToLower() == "binkw32.dll");
            if (binkw32 == null)
                return null;

            if (binkw32.ModuleMemorySize != (int)ExpectedDllSizes.BinkW32Dll)
            {
                _ignorePIDs.Add(game.Id);
                _uiThread.Send(d => MessageBox.Show("binkw32.dll was not the expected version.", "LiveSplit.Dishonored",
                    MessageBoxButtons.OK, MessageBoxIcon.Error), null);
                return null;
            }

            if (game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.DishonoredExe12)
            {
                _currentLevelPtr = new DeepPointer(0xFB7838, 0x2c0, 0x314, 0, 0x38);
                _currentBikMoviePtr = new DeepPointer(0xFC6AD4, 0x48, 0);
                _cutsceneActivePtr = new DeepPointer(0xFB51CC, 0x744);
                _missionStatsScreenFlagsPtr = new DeepPointer(0xFDEB08, 0x24, 0x41C, 0x2E0, 0xC4);
                _stringBase = 0xFA3624;
            }
            else if (game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.DishonoredExe14Reloaded || game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.DishonoredExe14Steam)
            {
                _currentLevelPtr = new DeepPointer(0x103D878, 0x2c0, 0x314, 0, 0x38);
                _currentBikMoviePtr = new DeepPointer(0x104CB18, 0x48, 0);
                _cutsceneActivePtr = new DeepPointer(0x103B20C, 0x744);
                _missionStatsScreenFlagsPtr = new DeepPointer(0x1065184, 0x24, 0x41C, 0x2F4, 0xC4);
                _stringBase = 0x1029664;
            }
            else
            {
                _ignorePIDs.Add(game.Id);
                _uiThread.Send(d => MessageBox.Show("Unexpected game version. Dishonored 1.2 or 1.4 is required.", "LiveSplit.Dishonored",
                    MessageBoxButtons.OK, MessageBoxIcon.Error), null);
                return null;
            }

            return game;
        }

        string GetEngineStringByID(Process p, int id)
        {
            string str;
            var ptr = new DeepPointer(_stringBase, (id*4), 0x10);
            ptr.Deref(p, out str, 32);
            return str;
        }
    }
}
