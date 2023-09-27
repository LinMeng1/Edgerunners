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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OnCall.ViewModels
{
    public class ShortcutViewModel : BindableBase
    {
        //内置延迟
        private readonly int internalDelay = 500;
        //事件聚合器
        private readonly IEventAggregator eventAggregator;
        //属性服务
        private readonly IPropertyService propertyService;
        //Http服务
        private readonly HttpService httpService;
        //集群缓存
        private List<Connection_GetClusters_Result> clustersCache = new List<Connection_GetClusters_Result>();
        public ShortcutViewModel(IEventAggregator eventAggregator, IPropertyService propertyService)
        {
            //依赖注入及初始化
            this.eventAggregator = eventAggregator;
            this.propertyService = propertyService;
            httpService = new HttpService();
            //监听事件 集群信息同步完成
            eventAggregator.GetEvent<ClustersSyncedEvent>().Subscribe(async (clusters) =>
            {
                if (clusters != null)
                {
                    clustersCache = clusters;
                    await SyncOwnerAsync(clusters);
                }

            }, ThreadOption.UIThread);
            //监听事件 Mqtt信息接收
            eventAggregator.GetEvent<MqttMessageReceivedEvent>().Subscribe(async (message) =>
            {
                if (!message.IsMastermind) return;
                string command = message.Content;
                if (command == "module on-call sync owner")
                    await SyncOwnerAsync(clustersCache);
            }, ThreadOption.UIThread, true);
            SyncOwner();
        }

        /// <summary>
        /// 同步负责人
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        private async Task SyncOwnerAsync(List<Connection_GetClusters_Result> clusters)
        {
            try
            {
                DialogOpen = true;
                DialogCategory = "Syncing";
                await Task.Delay(internalDelay);
                //集群类型优先级 Location > Product
                Connection_GetClusters_Result clusterLocation = clusters.FirstOrDefault(it => it.Category == "Location");
                Connection_GetClusterOwners_Result clusterOwnerLocation = null;
                if (clusterLocation != null)
                {
                    ControllersResult resultLocation = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/connection/GetClusterOwner", new { clusterLocation.Cluster, clusterLocation.Category }));
                    if (resultLocation.Result)
                        clusterOwnerLocation = JsonConvert.DeserializeObject<Connection_GetClusterOwners_Result>(resultLocation.Content.ToString());
                }
                Connection_GetClusters_Result clusterProduct = clusters.FirstOrDefault(it => it.Category == "Product");
                Connection_GetClusterOwners_Result clusterOwnerProduct = null;
                if (clusterProduct != null)
                {
                    ControllersResult resultProduct = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/connection/GetClusterOwner", new { clusterProduct.Cluster, clusterProduct.Category }));
                    if (resultProduct.Result)
                        clusterOwnerProduct = JsonConvert.DeserializeObject<Connection_GetClusterOwners_Result>(resultProduct.Content.ToString());
                }
                if (clusterOwnerLocation != null)
                {
                    ownerEmployeeId = clusterOwnerLocation.OwnerEmployeeId;
                    Owner = clusterOwnerLocation.Owner;
                    Contact = clusterOwnerLocation.Contact;
                    Organization = clusterOwnerLocation.Organization;
                    Leader = clusterOwnerLocation.Leader;
                    LeaderContact = clusterOwnerLocation.LeaderContact;
                    if (clusterOwnerProduct != null && clusterOwnerProduct.Owner != Owner)
                    {
                        AlternativeCluster = clusterProduct.Cluster;
                        AlternativeClusterCategory = clusterProduct.Category;
                        AlternativeOwner = clusterOwnerProduct.Owner;
                        AlternativeContact = clusterOwnerProduct.Contact;
                    }
                }
                else if (clusterOwnerProduct != null)
                {
                    ownerEmployeeId = clusterOwnerProduct.OwnerEmployeeId;
                    Owner = clusterOwnerProduct.Owner;
                    Contact = clusterOwnerProduct.Contact;
                    Organization = clusterOwnerProduct.Organization;
                    Leader = clusterOwnerProduct.Leader;
                    LeaderContact = clusterOwnerProduct.LeaderContact;
                }
                DialogOpen = false;
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[Shortcut]:[SyncOwnerAsync] exception:{e.Message}", true);
                DialogMessage = e.Message;
                DialogCategory = "Message";
            }
        }

        private async Task CallRepairAsync()
        {
            try
            {
                await SyncOwnerAsync(clustersCache);
                DialogOpen = true;
                DialogCategory = "Syncing";
                await Task.Delay(internalDelay);              
                object mainboard = propertyService.GetProperty("Mainboard") ?? throw new Exception("Mainboard is null");
                object hostname = propertyService.GetProperty("HostName") ?? throw new Exception("HostName is null");
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/modules/on-call/CallRepair", new { Mainboard = mainboard.ToString(), HostName = hostname.ToString(), Owner, OwnerEmployeeId = ownerEmployeeId }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                eventAggregator.GetEvent<MqttMessageReceivedEvent>().Publish(new MqttMessage()
                {
                    IsMastermind = true,
                    Content = "system sync banner messages"
                });
                DialogOpen = false;
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[Shortcut]:[CallRepairAsync] exception:{e.Message}", true);
                DialogMessage = e.Message;
                DialogCategory = "Message";
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
            DialogOpen = true;
            DialogCategory = "Syncing";
            Owner = null;
            Contact = null;
            Organization = null;
            Leader = null;
            LeaderContact = null;
            AlternativeCluster = null;
            AlternativeClusterCategory = null;
            AlternativeOwner = null;
            AlternativeContact = null;
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
            DialogOpen = false;
            await Task.Delay(500);
            DialogMessage = string.Empty;
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
        private string ownerEmployeeId;
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

        #region 备选集群类型
        private string alternativeClusterCategory;
        public string AlternativeClusterCategory
        {
            get => alternativeClusterCategory;
            set
            {
                SetProperty(ref alternativeClusterCategory, value);
            }
        }
        #endregion

        #region 备选集群
        private string alternativeCluster;
        public string AlternativeCluster
        {
            get => alternativeCluster;
            set
            {
                SetProperty(ref alternativeCluster, value);
            }
        }
        #endregion

        #region 备选负责人
        private string alternativeOwner;
        public string AlternativeOwner
        {
            get => alternativeOwner;
            set
            {
                SetProperty(ref alternativeOwner, value);
            }
        }
        #endregion

        #region 备选负责人联系方式
        private string alternativeContact;
        public string AlternativeContact
        {
            get => alternativeContact;
            set
            {
                SetProperty(ref alternativeContact, value);
            }
        }
        #endregion

        #region 对话框打开状态
        private bool dialogOpen = false;
        public bool DialogOpen
        {
            get => dialogOpen;
            set
            {
                SetProperty(ref dialogOpen, value);
            }
        }
        #endregion

        #region 对话框类型
        private string dialogCategory = "Syncing";
        public string DialogCategory
        {
            get => dialogCategory;
            set
            {
                SetProperty(ref dialogCategory, value);
            }
        }
        #endregion

        #region 对话框通用信息
        private string dialogMessage = string.Empty;
        public string DialogMessage
        {
            get => dialogMessage;
            set
            {
                SetProperty(ref dialogMessage, value);
            }
        }
        #endregion

        #endregion
    }
}
