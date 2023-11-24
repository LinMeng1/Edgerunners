using Microsoft.Win32;
using NightCity.Core;
using NightCity.Core.Events;
using NightCity.Core.Models.Standard;
using NightCity.Core.Services.Prism;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Lock.ViewModels
{
    public class MainViewModel : BindableBase, IDisposable
    {
        //事件聚合器
        private IEventAggregator eventAggregator;
        //属性服务
        private IPropertyService propertyService;
        //监听token列表
        List<SubscriptionToken> eventTokens = new List<SubscriptionToken>();
        //线程释放标志
        private bool disposable = false;
        public MainViewModel(IEventAggregator eventAggregator, IPropertyService propertyService)
        {
            //依赖注入及初始化
            this.eventAggregator = eventAggregator;
            this.propertyService = propertyService;
            //判断token
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
            //监听事件 权限信息变更
            eventTokens.Add(eventAggregator.GetEvent<AuthorizationInfoChangedEvent>().Subscribe((AuthorizationInfo) =>
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

            }, ThreadOption.UIThread, true));
            Task.Run(async () =>
            {
                await SetTaskManagerAsyncBack();
                MonitorExplorerThread();
            });
        }

        /// <summary>
        /// 设置任务管理器状态
        /// </summary>
        /// <returns></returns>
        private async Task SetTaskManagerAsyncBack()
        {
            try
            {
                await Task.Run(() =>
                 {
                     if (TaskManagerEnabled)
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
                         foreach (Process p in Process.GetProcesses())
                         {
                             if (p.ProcessName.CompareTo("Taskmgr") == 0)
                             {
                                 p.Kill();
                                 break;
                             }
                         }
                     }
                 });
            }
            catch (Exception e)
            {
                Global.Log($"[Lock]:[MainViewModel]:[SetTaskManagerAsync]:exception:{e.Message}", true);
            }
        }
        /// <summary>
        /// 监控资源管理器（线程）
        /// </summary>
        private void MonitorExplorerThread()
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
                        Global.Log($"[Lock]:[MainViewModel]:[MonitorExplorerThread]:exception:{e.Message}", true);
                    }
                }
            });
        }

        #region 可视化属性集合

        #region 任务管理器是否可用
        private bool taskManagerEnabled;
        public bool TaskManagerEnabled
        {
            get => taskManagerEnabled;
            set
            {
                SetProperty(ref taskManagerEnabled, value);
                _ = SetTaskManagerAsyncBack();
            }
        }
        #endregion

        #region 资源管理器是否可用
        private bool explorerEnabled;
        public bool ExplorerEnabled
        {
            get => explorerEnabled;
            set
            {
                SetProperty(ref explorerEnabled, value);
            }
        }
        #endregion

        #region 可用网络位置列表
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
        #endregion

        #region 可用盘符列表
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
        #endregion

        #region 不可用面板列表
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
        #endregion

        #endregion

        public void Dispose()
        {
            foreach (var eventToken in eventTokens)
            {
                eventToken.Dispose();
            }
            eventAggregator = null;
            propertyService = null;
            TaskManagerEnabled = true;
            disposable = true;
        }
    }
}
