using Newtonsoft.Json;
using NightCity.Core;
using NightCity.Core.Events;
using NightCity.Core.Models;
using NightCity.Core.Models.Standard;
using NightCity.Core.Services;
using NightCity.Core.Services.Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NightCity.ViewModels
{
    public class BannerViewModel : BindableBase
    {
        //事件聚合器
        private readonly IEventAggregator eventAggregator;
        //属性服务
        private readonly IPropertyService propertyService;
        //Http服务
        private readonly HttpService httpService;
        public BannerViewModel(IEventAggregator eventAggregator, IPropertyService propertyService)
        {
            //依赖注入及初始化
            this.eventAggregator = eventAggregator;
            this.propertyService = propertyService;
            httpService = new HttpService();
            Messages.CollectionChanged += (sender, e) =>
            {
                eventAggregator.GetEvent<BannerMessagesChangedEvent>().Publish(new Tuple<BannerMessage, int>(messages.FirstOrDefault(), Messages.Count));
            };
            //监听事件 Mqtt连接/断开
            eventAggregator.GetEvent<MqttConnectedEvent>().Subscribe(async (isConnected) =>
            {
                if (isConnected) await SyncMessagesAsync();
            }, ThreadOption.UIThread);
            //监听事件 Mqtt信息接收
            eventAggregator.GetEvent<MqttMessageReceivedEvent>().Subscribe(async (message) =>
            {
                if (!message.IsMastermind) return;
                string command = message.Content;
                if (command == "system sync banner messages")
                    await SyncMessagesAsync();
            }, ThreadOption.UIThread);
            //监听事件 权限信息变更
            eventAggregator.GetEvent<AuthorizationInfoChangedEvent>().Subscribe((authorizationInfo) =>
            {
                object token = propertyService.GetProperty(authorizationInfo.Item2);
                httpService.AddToken(token?.ToString());
            }, ThreadOption.UIThread, true);
            //监听事件 横幅信息接收
            eventAggregator.GetEvent<BannerMessageRemovingEvent>().Subscribe((message) =>
            {
                RemoveMessage(message);
            }, ThreadOption.UIThread);
            //监听事件 横幅信息同步请求
            eventAggregator.GetEvent<BannerMessageSyncingEvent>().Subscribe(async () =>
            {
                await SyncMessagesAsync();
            }, ThreadOption.UIThread);
        }

        /// <summary>
        /// 同步消息
        /// </summary>
        /// <returns></returns>
        private async Task SyncMessagesAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                string mainboard = propertyService.GetProperty("Mainboard").ToString();
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/banner/GetMessages", new { Mainboard = mainboard }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<Banner_GetMessages_Result> messages = JsonConvert.DeserializeObject<List<Banner_GetMessages_Result>>(result.Content.ToString());
                List<Banner_GetMessages_Result> sorted = messages.OrderBy(it => it.Urgency == "Inform").ThenBy(it => it.Urgency == "Plan").ThenBy(it => it.Urgency == "Execute").ThenByDescending(it => it.Priority).ThenByDescending(it => it.CreateTime).ToList();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    lock (Messages)
                    {
                        Messages.Clear();
                        foreach (var message in sorted)
                        {
                            Messages.Add(new BannerMessage()
                            {
                                Id = message.Id,
                                Urgency = message.Urgency,
                                Priority = message.Priority,
                                Category = message.Category,
                                Content = message.Content,
                                LinkCommand = message.LinkCommand,
                                LinkInfomation = message.LinkInfomation,
                                Extensible = message.Extensible,
                                CreateTime = message.CreateTime,
                            });
                        }
                    }
                });
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[Banner]:[SyncMessagesAsync] exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 同步同辖管理集群消息
        /// </summary>
        /// <returns></returns>
        private async Task SyncJurisdictionalClustersMessageAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                string mainboard = propertyService.GetProperty("Mainboard").ToString();
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/connection/GetJurisdictionalClusterOwnersClusters", new { Mainboard = mainboard }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<string> jurisdictionalClusters = JsonConvert.DeserializeObject<List<string>>(result.Content.ToString());
                eventAggregator.GetEvent<MqttMessageSendingEvent>().Publish(new Tuple<List<string>, MqttMessage>(jurisdictionalClusters, new MqttMessage()
                {
                    IsMastermind = true,
                    Content = "system sync banner messages"
                }));
            }
            catch (Exception e)
            {
                Global.Log($"[Banner]:[SyncJurisdictionalClustersMessageAsync] exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task RemoveMessageAsync(string id)
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                string mainboard = propertyService.GetProperty("Mainboard").ToString();
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/banner/RemoveMessage", new { Id = id }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                await SyncMessagesAsync();
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[Banner]:[RemoveMessageAsync] exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        #region 命令集合

        #region 命令：同步消息
        public ICommand SyncMessagesCommand
        {
            get => new DelegateCommand(SyncMessages);
        }
        private async void SyncMessages()
        {
            await SyncMessagesAsync();
        }
        #endregion

        #region 命令：同步同辖消息
        public ICommand SyncJurisdictionalClustersMessageCommand
        {
            get => new DelegateCommand(SyncJurisdictionalClustersMessage);
        }
        private async void SyncJurisdictionalClustersMessage()
        {
            await SyncJurisdictionalClustersMessageAsync();
        }
        #endregion

        #region 命令：确认/取消并关闭对话框
        public ICommand CleanMessageCommand
        {
            get => new DelegateCommand(CleanMessage);
        }
        public async void CleanMessage()
        {
            MessageHost.HideImmediately();
            await Task.Delay(500);
            MessageHost.DialogMessage = string.Empty;
        }
        #endregion

        #region 命令：删除消息
        public ICommand RemoveMessageCommand
        {
            get => new DelegateCommand<BannerMessage>(RemoveMessage);
        }
        private async void RemoveMessage(BannerMessage message)
        {
            if (message.Urgency != "Inform") return;
            await RemoveMessageAsync(message.Id);
        }
        #endregion

        #region 命令：发布链接请求
        public ICommand TryLinkCommand
        {
            get => new DelegateCommand<object>(TryLink);
        }
        private void TryLink(object parameter)
        {
            string category = ((object[])parameter)[0]?.ToString();
            string linkCommand = ((object[])parameter)[1]?.ToString();
            eventAggregator.GetEvent<BannerMessageTryLinkingEvent>().Publish(new Tuple<string, string>(category, linkCommand));
        }
        #endregion

        #endregion

        #region 可视化属性集合

        #region 消息集合
        private ObservableCollection<BannerMessage> messages = new ObservableCollection<BannerMessage>();
        public ObservableCollection<BannerMessage> Messages
        {
            get => messages;
            set
            {
                SetProperty(ref messages, value);
            }
        }

        #endregion

        #region 对话框设置
        private DialogSetting messageHost = new DialogSetting();
        public DialogSetting MessageHost
        {
            get => messageHost;
            set
            {
                SetProperty(ref messageHost, value);
            }
        }
        #endregion

        #endregion
    }
}
