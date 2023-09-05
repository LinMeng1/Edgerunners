using Microsoft.Win32;
using NightCity.Core;
using NightCity.Core.Events;
using NightCity.Core.Services.Prism;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Lock.ViewModels
{
    public class MainViewModel : BindableBase, IDisposable
    {
        bool disposable = false;
        IDisposable authorizationInfoChangedEvent;
        public MainViewModel(IEventAggregator e, IPropertyService propertyService)
        {           
            object TestEngAuthorizationInfo = propertyService.GetProperty("TestEngAuthorizationInfo");
            if (TestEngAuthorizationInfo != null)
            {
                TaskManagerEnabled = true;
                ExplorerEnabled = true;
            }
            else
            {
                TaskManagerEnabled = false;
                ExplorerEnabled = false;
            }
            authorizationInfoChangedEvent = e.GetEvent<AuthorizationInfoChangedEvent>().Subscribe((AuthorizationInfo) =>
            {
                if (AuthorizationInfo.Item1 != "TestEngAuthorization") return;
                object authorizationInfo = propertyService.GetProperty(AuthorizationInfo.Item2);
                if (authorizationInfo != null)
                {
                    TaskManagerEnabled = true;
                    ExplorerEnabled = true;
                }
                else
                {
                    TaskManagerEnabled = false;
                    ExplorerEnabled = false;
                }
            }, ThreadOption.UIThread, true);
            SetTaskManager(TaskManagerEnabled);
            MonitorExplorer();
        }
        private bool taskManagerEnabled;
        public bool TaskManagerEnabled
        {
            get => taskManagerEnabled;
            set
            {
                SetProperty(ref taskManagerEnabled, value);
                SetTaskManager(value);
            }
        }

        private bool explorerEnabled;
        public bool ExplorerEnabled
        {
            get => explorerEnabled;
            set
            {
                SetProperty(ref explorerEnabled, value);
            }
        }

        private ObservableCollection<string> availableNetworkLocation = new ObservableCollection<string>()
        {
             @"\\10.114.113.3",@"\\10.114.113.4",@"\\10.114.113.10"
        };
        public ObservableCollection<string> AvailableNetworkLocation
        {
            get => availableNetworkLocation;
            set
            {
                SetProperty(ref availableNetworkLocation, value);
            }
        }

        private ObservableCollection<string> availableDrive = new ObservableCollection<string>()
        {
             "C","D","E","F"
        };
        public ObservableCollection<string> AvailableDrive
        {
            get => availableDrive;
            set
            {
                SetProperty(ref availableDrive, value);
            }
        }

        private ObservableCollection<string> unavailablePanel = new ObservableCollection<string>()
        {
            "网络和共享中心","Network and Sharing Center"
        };
        public ObservableCollection<string> UnavailablePanel
        {
            get => unavailablePanel;
            set
            {
                SetProperty(ref unavailablePanel, value);
            }
        }

        public void MonitorExplorer()
        {
            Task.Run(() =>
            {
                while (!disposable)
                {
                    Thread.Sleep(200);
                    if (ExplorerEnabled) continue;
                    try
                    {
                        foreach (SHDocVw.InternetExplorer window in new SHDocVw.ShellWindows())
                        {
                            if (Path.GetFileNameWithoutExtension(window.FullName).ToLowerInvariant() == "explorer")
                            {
                                if (Uri.IsWellFormedUriString(window.LocationURL, UriKind.Absolute))
                                {
                                    string location = new Uri(window.LocationURL).LocalPath;
                                    if (location.IndexOf(@"/") == 0)
                                    {
                                    }
                                    else if (location.IndexOf(@"\\") == 0)
                                    {
                                        bool available = false;
                                        for (int i = 0; i < AvailableNetworkLocation.Count; i++)
                                        {
                                            int x = location.IndexOf(AvailableNetworkLocation[i]);
                                            if (location.IndexOf(AvailableNetworkLocation[i]) == 0)
                                            {
                                                available = true;
                                                break;
                                            }
                                        }
                                        if (!available)
                                            window.Quit();
                                    }
                                    else
                                    {
                                        bool available = false;
                                        for (int i = 0; i < AvailableDrive.Count; i++)
                                        {
                                            if (location.IndexOf(AvailableDrive[i]) == 0)
                                            {
                                                available = true;
                                                break;
                                            }
                                        }
                                        if (!available)
                                            window.Quit();
                                    }
                                }
                                else
                                {
                                    string panelName = window.LocationName;
                                    bool available = true;
                                    for (int i = 0; i < UnavailablePanel.Count; i++)
                                    {
                                        if (panelName == UnavailablePanel[i])
                                        {
                                            available = false;
                                            break;
                                        }
                                    }
                                    if (!available)
                                        window.Quit();
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Global.Log($"[Lock]:[MainViewModel]:[MonitorExplorer]:exception:{e.Message}", true);
                    }
                }
            });
        }
        public void SetTaskManager(bool isEnabled)
        {
            try
            {
                if (isEnabled)
                {
                    Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System");
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System");
                    Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true).SetValue("DisableTaskMgr", 0, RegistryValueKind.DWord);
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true).SetValue("DisableTaskMgr", 0, RegistryValueKind.DWord);
                }
                else
                {
                    Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System");
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System");
                    Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true).SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true).SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
                }
            }
            catch (Exception e)
            {
                Global.Log($"[Lock]:[MainViewModel]:[SetTaskManager]:exception:{e.Message}", true);
            }
        }
        public void Dispose()
        {
            disposable = true;
            authorizationInfoChangedEvent.Dispose();
            SetTaskManager(true);
        }
    }
}
