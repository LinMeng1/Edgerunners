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
using System.Threading.Tasks;
using System.Windows.Input;

namespace OnCall.ViewModels
{
    public class ShortcutViewModel : BindableBase, IDisposable
    {
        //事件聚合器
        private IEventAggregator eventAggregator;
        //属性服务
        private IPropertyService propertyService;
        //Http服务
        private HttpService httpService;
        //集群缓存
        private List<Connection_GetClusters_Result> clustersCache = new List<Connection_GetClusters_Result>();
        //监听token列表
        List<SubscriptionToken> eventTokens = new List<SubscriptionToken>();
        public ShortcutViewModel(IEventAggregator eventAggregator, IPropertyService propertyService)
        {
            //依赖注入及初始化
            this.eventAggregator = eventAggregator;
            this.propertyService = propertyService;
            httpService = new HttpService();
            //监听事件 集群信息同步完成
            eventTokens.Add(eventAggregator.GetEvent<ClustersSyncedEvent>().Subscribe(async (clusters) =>
            {
                if (clusters != null)
                {
                    clustersCache = clusters;
                    await SyncJurisdictionalClusterOwnerAsync();
                }
            }, ThreadOption.UIThread));
            //监听事件 Mqtt信息接收
            eventTokens.Add(eventAggregator.GetEvent<MqttMessageReceivedEvent>().Subscribe(async (message) =>
            {
                if (!message.IsMastermind) return;
                string command = message.Content;
                if (command == "module on-call sync owner")
                    await SyncJurisdictionalClusterOwnerAsync();
            }, ThreadOption.UIThread, true));
            SyncOwner();
        }

        /// <summary>
        /// 同步负责人
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        private async Task SyncJurisdictionalClusterOwnerAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                Owner = null;
                Contact = null;
                Organization = null;
                Leader = null;
                LeaderContact = null;
                await Task.Delay(MessageHost.InternalDelay);
                object mainboard = propertyService.GetProperty("Mainboard") ?? throw new Exception("Mainboard is null");
                ControllersResult resultLocation = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/connection/GetJurisdictionalClusterOwner", new { Mainboard = mainboard.ToString() }));
                Connection_GetJurisdictionalClusterOwner_Result jurisdictionalClusterOwner = null;
                if (resultLocation.Result)
                    jurisdictionalClusterOwner = JsonConvert.DeserializeObject<Connection_GetJurisdictionalClusterOwner_Result>(resultLocation.Content.ToString());
                if (jurisdictionalClusterOwner != null)
                {
                    Owner = jurisdictionalClusterOwner.Owner;
                    Contact = jurisdictionalClusterOwner.Contact;
                    Organization = jurisdictionalClusterOwner.Organization;
                    Leader = jurisdictionalClusterOwner.Leader;
                    LeaderContact = jurisdictionalClusterOwner.LeaderContact;
                }
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[Shortcut]:[SyncOwnerAsync] exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 异常报修
        /// </summary>
        /// <returns></returns>
        private async Task CallRepairAsync()
        {
            try
            {
                await SyncJurisdictionalClusterOwnerAsync();
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                object mainboard = propertyService.GetProperty("Mainboard") ?? throw new Exception("Mainboard is null");
                object hostname = propertyService.GetProperty("HostName") ?? throw new Exception("HostName is null");
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/modules/on-call/CallReport", new { Mainboard = mainboard.ToString(), HostName = hostname.ToString() }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<string> jurisdictionalClusterOwner = JsonConvert.DeserializeObject<List<string>>(result.Content.ToString());
                eventAggregator.GetEvent<MqttMessageSendingEvent>().Publish(new Tuple<List<string>, MqttMessage>(jurisdictionalClusterOwner, new MqttMessage()
                {
                    IsMastermind = true,
                    Content = "system sync banner messages"
                }));
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[Shortcut]:[CallRepairAsync] exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        #region 命令集合

        #region 命令：同步负责人
        public ICommand SyncOwnerCommand
        {
            get => new DelegateCommand(SyncOwner);
        }
        private void SyncOwner()
        {
            MessageHost.Show();
            MessageHost.DialogCategory = "Syncing";            
            eventAggregator.GetEvent<MqttMessageReceivedEvent>().Publish(new MqttMessage()
            {
                IsMastermind = true,
                Content = "system sync clusters"
            });
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
            MessageHost.Hide();
        }
        #endregion

        #region 命令：异常报修
        public ICommand CallRepairCommand
        {
            get => new DelegateCommand(CallRepair);
        }
        public async void CallRepair()
        {
            await CallRepairAsync();
        }
        #endregion

        #endregion

        #region 可视化属性集合

        #region 负责人
        private string owner;
        public string Owner
        {
            get => owner;
            set
            {
                SetProperty(ref owner, value);
            }
        }
        #endregion

        #region 联系方式
        private string contact;
        public string Contact
        {
            get => contact;
            set
            {
                SetProperty(ref contact, value);
            }
        }
        #endregion

        #region 组织
        private string organization;
        public string Organization
        {
            get => organization;
            set
            {
                SetProperty(ref organization, value);
            }
        }
        #endregion

        #region 上级
        private string leader;
        public string Leader
        {
            get => leader;
            set
            {
                SetProperty(ref leader, value);
            }
        }
        #endregion

        #region 上级联系方式
        private string leaderContact;
        public string LeaderContact
        {
            get => leaderContact;
            set
            {
                SetProperty(ref leaderContact, value);
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

        public void Dispose()
        {
            foreach (var eventToken in eventTokens)
            {
                eventToken.Dispose();
            }
            eventAggregator = null;
            propertyService = null;
            httpService = null;
        }
    }
}
