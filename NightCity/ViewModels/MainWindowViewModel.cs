using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using NightCity.Core;
using NightCity.Core.Events;
using NightCity.Core.Models.Standard;
using NightCity.Core.Services;
using NightCity.Core.Services.Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NightCity.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        //事件聚合器
        private readonly IEventAggregator eventAggregator;
        //属性服务
        private readonly IPropertyService propertyService;
        //Http服务
        private readonly HttpService httpService;
        public MainWindowViewModel(IEventAggregator eventAggregator, IPropertyService propertyService)
        {
            this.eventAggregator = eventAggregator;
            this.propertyService = propertyService;
            httpService = new HttpService();
            //监听事件 Mqtt连接/断开
            eventAggregator.GetEvent<MqttConnectedEvent>().Subscribe(async (isConnected) =>
            {
                IsMqttConnected = isConnected;
                if (isConnected)
                {
                    await CleanTeAuthorizationInfoAsync();
                }
            }, ThreadOption.UIThread);
            //监听事件 Mqtt信息接收
            eventAggregator.GetEvent<MqttMessageReceivedEvent>().Subscribe((message) =>
            {
                if (!message.IsMastermind) return;
                string command = message.Content;
                if (command == "system exit when protected")
                {
                    Process[] processes = Process.GetProcesses();
                    var daemon = processes.FirstOrDefault(p => p.ProcessName.StartsWith("NightCity.Daemon"));
                    if (daemon != null)
                        Environment.Exit(0);
                }
                else if (command == "system reload")
                {
                    System.Windows.Forms.Application.Restart();
                    Application.Current.Shutdown();
                }
                else if (command == "system clean te authorization info")
                {
                    propertyService.SetProperty("TestEngAuthorizationInfo", null);
                    propertyService.SetProperty("DisplayName", null);
                    propertyService.SetProperty("TestEngAuthorizationUser", null);
                    eventAggregator.GetEvent<AuthorizationInfoChangedEvent>().Publish(new Tuple<string, string>("TestEngAuthorization", "TestEngAuthorizationInfo"));
                    eventAggregator.GetEvent<TemplateClosingEvent>().Publish("TestEngAuthorization");
                }
            }, ThreadOption.UIThread);
            //监听事件 模块列表改变
            eventAggregator.GetEvent<ModulesChangedEvent>().Subscribe((modules) =>
            {
                List<string> authorizedMod = new List<string>();
                foreach (var authMod in AuthorizationModules)
                {
                    if (authMod.Icon == PackIconKind.Fingerprint)
                        authorizedMod.Add(authMod.Name);
                }
                AuthorizationModules.Clear();
                MonitorModules.Clear();
                foreach (var module in modules)
                {
                    if (module.Category == "Authorization")
                    {
                        if (authorizedMod.Contains(module.Name))
                            module.Icon = PackIconKind.Fingerprint;
                        AuthorizationModules.Add(module);
                    }
                    else if (module.Category == "Monitor")
                        MonitorModules.Add(module);
                }
            }, ThreadOption.UIThread);
            //监听事件 权限信息变更
            eventAggregator.GetEvent<AuthorizationInfoChangedEvent>().Subscribe((authorizationInfo) =>
            {
                ModuleInfo module = AuthorizationModules.FirstOrDefault(it => it.Name == authorizationInfo.Item1);
                if (module == null) return;
                object authorizationInfoItem = propertyService.GetProperty(authorizationInfo.Item2);
                if (authorizationInfoItem != null)
                    module.Icon = PackIconKind.Fingerprint;
                else
                    module.Icon = PackIconKind.FingerprintOff;
                object token = propertyService.GetProperty(authorizationInfo.Item2);
                httpService.AddToken(token?.ToString());
            }, ThreadOption.UIThread, true);
            //监听事件 Mqtt未读数量改变
            eventAggregator.GetEvent<MqttNoReadMessageCountChangedEvent>().Subscribe((noReadMessageCount) =>
            {
                HaveNoReadMessage = noReadMessageCount > 0;
            }, ThreadOption.UIThread);
            //监听事件 横幅信息改变
            eventAggregator.GetEvent<BannerMessagesChangedEvent>().Subscribe((bannerMessages) =>
            {
                TopBannerMessage = bannerMessages.Item1;
                BannerMessageCount = bannerMessages.Item2;
                Syncing = false;
            }, ThreadOption.UIThread);
            //监听事件 固定/接触固定连接界面
            eventAggregator.GetEvent<IsConnectionFixChangedEvent>().Subscribe((isConnectionFix) =>
            {
                this.isConnectionFix = isConnectionFix;
            }, ThreadOption.UIThread);
        }

        private async Task CleanTeAuthorizationInfoAsync()
        {
            try
            {
                object TestEngAuthorizationInfo = propertyService.GetProperty("TestEngAuthorizationInfo");
                if (TestEngAuthorizationInfo != null)
                {
                    string mainboard = propertyService.GetProperty("Mainboard").ToString();
                    string user = propertyService.GetProperty("TestEngAuthorizationUser").ToString();
                    ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/basic/account/CheckInvalidLogin", new { User = user, Client = mainboard }));
                    if (!result.Result)
                    {
                        propertyService.SetProperty("TestEngAuthorizationInfo", null);
                        propertyService.SetProperty("DisplayName", null);
                        propertyService.SetProperty("TestEngAuthorizationUser", null);
                        eventAggregator.GetEvent<AuthorizationInfoChangedEvent>().Publish(new Tuple<string, string>("TestEngAuthorization", "TestEngAuthorizationInfo"));
                        eventAggregator.GetEvent<TemplateClosingEvent>().Publish("TestEngAuthorization");
                    }
                }
            }
            catch (Exception e)
            {
                Global.Log($"[MainWindow]:[CleanTeAuthorizationInfoAsync] exception:{e.Message}", true);
            }
        }

        #region 命令集合

        #region 命令：启动模块
        public ICommand LaunchModuleCommand
        {
            get => new DelegateCommand<string>(LaunchModule);
        }
        private void LaunchModule(string moduleName)
        {
            ModuleInfo module = MonitorModules.FirstOrDefault(it => it.Name == moduleName);
            if (module == null)
                module = AuthorizationModules.FirstOrDefault(it => it.Name == moduleName);
            if (module == null) return;
            eventAggregator.GetEvent<TemplateReOpeningEvent>().Publish(moduleName);
        }
        #endregion

        #region 命令：清除信息
        public ICommand RemoveMessageCommand
        {
            get => new DelegateCommand<BannerMessage>(RemoveMessage);
        }
        private void RemoveMessage(BannerMessage message)
        {
            eventAggregator.GetEvent<BannerMessageRemovingEvent>().Publish(message);
            Syncing = true;
        }
        #endregion

        #region 命令：尝试链接模块
        public ICommand TryLinkCommand
        {
            get => new DelegateCommand<object>(TryLink);
        }
        private async void TryLink(object parameter)
        {
            Syncing = true;
            string category = ((object[])parameter)[0]?.ToString();
            string linkCommand = ((object[])parameter)[1]?.ToString();
            eventAggregator.GetEvent<BannerMessageTryLinkingEvent>().Publish(new Tuple<string, string>(category, linkCommand));
            await Task.Delay(1500);
            Syncing = false;

        }
        #endregion

        #endregion

        #region 可视化属性集合

        #region 授权模块集合
        private ObservableCollection<ModuleInfo> authorizationModules = new ObservableCollection<ModuleInfo>();
        public ObservableCollection<ModuleInfo> AuthorizationModules
        {
            get => authorizationModules;
            set
            {
                SetProperty(ref authorizationModules, value);
            }
        }
        #endregion

        #region 监控模块集合
        private ObservableCollection<ModuleInfo> monitorModules = new ObservableCollection<ModuleInfo>();
        public ObservableCollection<ModuleInfo> MonitorModules
        {
            get => monitorModules;
            set
            {
                SetProperty(ref monitorModules, value);
            }
        }
        #endregion

        #region 是否存在未读信息
        private bool haveNoReadMessage;
        public bool HaveNoReadMessage
        {
            get => haveNoReadMessage;
            set
            {
                SetProperty(ref haveNoReadMessage, value);
            }
        }
        #endregion

        #region 第一置顶信息
        private BannerMessage topBannerMessage;
        public BannerMessage TopBannerMessage
        {
            get => topBannerMessage;
            set
            {
                SetProperty(ref topBannerMessage, value);
            }
        }
        #endregion

        #region 置顶信息数量
        private int bannerMessageCount;
        public int BannerMessageCount
        {
            get => bannerMessageCount;
            set
            {
                SetProperty(ref bannerMessageCount, value);
            }
        }
        #endregion

        #region 连接界面是否打开
        private bool isConnectionFix;
        private bool isConnectionOpen;
        public bool IsConnectionOpen
        {
            get => isConnectionOpen;
            set
            {
                if (!isConnectionFix)
                {
                    SetProperty(ref isConnectionOpen, value);
                }
            }
        }
        #endregion

        #region Mqtt是否连接
        private bool isMqttConnected = false;
        public bool IsMqttConnected
        {
            get => isMqttConnected;
            set
            {
                SetProperty(ref isMqttConnected, value);
            }
        }
        #endregion

        #region 是否正在同步
        private bool syncing = false;
        public bool Syncing
        {
            get => syncing;
            set
            {
                SetProperty(ref syncing, value);
            }
        }
        #endregion

        #endregion

    }
}
