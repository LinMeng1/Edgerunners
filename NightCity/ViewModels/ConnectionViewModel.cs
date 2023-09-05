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

namespace NightCity.ViewModels
{
    public class ConnectionViewModel : BindableBase
    {
        //内置延迟
        private readonly int internalDelay = 500;
        //事件聚合器
        private readonly IEventAggregator eventAggregator;
        //属性服务
        private readonly IPropertyService propertyService;
        //Mqtt服务
        private MqttService mqttService;
        //Http服务
        private readonly HttpService httpService;
        public ConnectionViewModel(IEventAggregator eventAggregator, IPropertyService propertyService)
        {
            //依赖注入及初始化
            this.eventAggregator = eventAggregator;
            this.propertyService = propertyService;
            httpService = new HttpService();
            //监听事件 Mqtt信息接受
            eventAggregator.GetEvent<MqttMessageReceivedEvent>().Subscribe(async (message) =>
            {
                if (!message.IsMastermind) return;
                string command = message.Content;
                if (string.IsNullOrEmpty(command)) return;
                if (command == "sync clusters")
                    await SyncClustersAsync();
            }, ThreadOption.UIThread);
            //监听事件 权限信息变更
            eventAggregator.GetEvent<AuthorizationInfoChangedEvent>().Subscribe((authorizationInfo) =>
            {
                object token = propertyService.GetProperty(authorizationInfo.Item2);
                httpService.AddToken(token?.ToString());
            }, ThreadOption.UIThread, true);
            //等待设备SN获取后
            Task.Run(async () =>
            {
                object mainboard = null;
                while (mainboard == null)
                {
                    mainboard = propertyService.GetProperty("Mainboard");
                }
                mqttService = new MqttService(mainboard.ToString(), "10.114.113.101", 1883);
                mqttService.ConnectionChanged += (IsConnected) =>
                {
                    this.IsConnected = IsConnected;
                };
                mqttService.ApplicationMessageReceived += (message) =>
                {
                    eventAggregator.GetEvent<MqttMessageReceivedEvent>().Publish(message);
                };
                mqttService.TopicCollection.NoReadMessageCountChanged += (noReadMessageCount) =>
                {
                    eventAggregator.GetEvent<MqttNoReadMessageCountChangedEvent>().Publish(noReadMessageCount);
                };
                TopicCollection = mqttService.TopicCollection;
                await SyncClustersAsync();
            });
        }

        /// <summary>
        /// 同步集群
        /// </summary>   
        /// <returns></returns>
        private async Task SyncClustersAsync()
        {
            try
            {
                DialogOpen = true;
                DialogCategory = "Syncing";
                await Task.Delay(internalDelay);
                string mainboard = propertyService.GetProperty("Mainboard").ToString();
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/connection/GetClusters", new { Mainboard = mainboard }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<Connection_GetClusters_Result> clusters = JsonConvert.DeserializeObject<List<Connection_GetClusters_Result>>(result.Content.ToString());
                await mqttService.RemoveAllClusterTopic();
                foreach (var cluster in clusters)
                {
                    await mqttService.AddTopic(cluster.Cluster, cluster.Category);
                }
                DialogOpen = false;
            }
            catch (Exception e)
            {
                Global.Log($"[Connection]:[SyncClustersAsync] exception:{e.Message}", true);
                DialogMessage = e.Message;
                DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 添加至集群
        /// </summary>
        /// <param name="cluster">集群名</param>
        /// <param name="category">类别</param>
        /// <returns></returns>
        private async Task AddToClusterAsync(string cluster, string category = null)
        {
            try
            {
                DialogOpen = true;
                DialogCategory = "Syncing";
                await Task.Delay(internalDelay);
                string mainboard = propertyService.GetProperty("Mainboard").ToString();
                if (cluster == string.Empty)
                    throw new Exception("Cluster name can not be empty");
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/connection/SetCluster", new { Mainboard = mainboard, Category = category, Cluster = cluster, }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                await SyncClustersAsync();
                DialogOpen = false;
            }
            catch (Exception e)
            {
                Global.Log($"[Connection]:[AddToClusterAsync] exception:{e.Message}", true);
                DialogMessage = e.Message;
                DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 从集群退出
        /// </summary>
        /// <param name="cluster">集群名</param>
        /// <returns></returns>
        private async Task RemoveFromClusterAsync(string cluster)
        {
            try
            {
                DialogOpen = true;
                DialogCategory = "Syncing";
                await Task.Delay(internalDelay);
                string mainboard = propertyService.GetProperty("Mainboard").ToString();
                if (cluster == string.Empty)
                    throw new Exception("Cluster name can not be empty");
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/connection/RemoveCluster", new { Mainboard = mainboard, Cluster = cluster }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                await SyncClustersAsync();
                DialogOpen = false;
            }
            catch (Exception e)
            {
                Global.Log($"[Connection]:[RemoveFromClusterAsync] exception:{e.Message}", true);
                DialogMessage = e.Message;
                DialogCategory = "Message";
            }
        }

        #region 命令集合

        #region 命令：同步集群信息
        public ICommand SyncClustersCommand
        {
            get => new DelegateCommand(SyncClusters);
        }
        private async void SyncClusters()
        {
            await SyncClustersAsync();
        }
        #endregion

        #region 命令：清理主题下信息
        public ICommand ClearMessageCommand
        {
            get => new DelegateCommand(ClearMessage);
        }
        private void ClearMessage()
        {
            if (SelectedTopic == null) return;
            mqttService.ClearTopic(SelectedTopic.Topic);
        }
        #endregion

        #region 命令：发送主题下信息
        public ICommand SendMessageCommand
        {
            get => new DelegateCommand(SendMessage);
        }
        private async void SendMessage()
        {
            if (SelectedTopic == null) return;
            mqttService.ReadTopic(SelectedTopic.Topic);
            object name = propertyService.GetProperty("DisplayName");
            string topic = SelectedTopic.Category == null ? SelectedTopic.Topic : $"{SelectedTopic.Category}/{SelectedTopic.Topic}";
            await mqttService.Publish(IsMastermind, topic, name == null ? "The Nameless" : name.ToString(), EditingMessage);
            EditingMessage = string.Empty;
        }
        #endregion

        #region 命令：将主题下信息设为已读
        public ICommand ReadMessageCommand
        {
            get => new DelegateCommand<MqttTopic>(ReadMessage);
        }
        private void ReadMessage(MqttTopic topic)
        {
            if (topic == null) return;
            mqttService.ReadTopic(topic.Topic);
        }
        #endregion

        #region 命令：添加集群询问
        public ICommand TryAddToClusterCommand
        {
            get => new DelegateCommand(TryAddToCluster);
        }
        private void TryAddToCluster()
        {
            DialogOpen = true;
            DialogCategory = "AddToCluster";
            EditingCluster = string.Empty;
            IsEditingClusterWithCategory = false;
            EditingClusterCategory = string.Empty;

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

        #region 命令：添加集群
        public ICommand AddToClusterCommand
        {
            get => new DelegateCommand(AddToClusterAsync);
        }
        private async void AddToClusterAsync()
        {
            if (IsEditingClusterWithCategory)
                await AddToClusterAsync(EditingCluster, EditingClusterCategory);
            else
                await AddToClusterAsync(EditingCluster);
        }
        #endregion

        #region 命令：退出集群询问
        public ICommand TryRemoveFromClusterCommand
        {
            get => new DelegateCommand(TryRemoveFromCluster);
        }
        private void TryRemoveFromCluster()
        {
            DialogOpen = true;
            DialogMessage = "Are you sure you want to remove this PC from this cluster";
            DialogCategory = "RemoveFromCluster";
        }
        #endregion

        #region 命令：退出集群
        public ICommand RemoveFromClusterCommand
        {
            get => new DelegateCommand(RemoveFromCluster);
        }
        private async void RemoveFromCluster()
        {
            await RemoveFromClusterAsync(SelectedTopic.Topic);
        }
        #endregion

        #endregion

        #region 可视化属性集合

        #region 是否连接
        private bool isConnected = false;
        public bool IsConnected
        {
            get => isConnected;
            set
            {
                SetProperty(ref isConnected, value);
                Task.Run(() =>
                {
                    eventAggregator.GetEvent<MqttConnectedEvent>().Publish(value);
                });
            }
        }
        #endregion

        #region 是否固定
        private bool isFix;
        public bool IsFix
        {
            get => isFix;
            set
            {
                SetProperty(ref isFix, value);
                eventAggregator.GetEvent<IsConnectionFixChangedEvent>().Publish(value);
            }
        }
        #endregion

        #region 主题集合
        private MqttTopicCollection topicCollection;
        public MqttTopicCollection TopicCollection
        {
            get => topicCollection;
            set
            {
                SetProperty(ref topicCollection, value);
            }
        }
        #endregion

        #region 所选主题
        private MqttTopic selectedTopic;
        public MqttTopic SelectedTopic
        {
            get => selectedTopic;
            set
            {
                SetProperty(ref selectedTopic, value);
            }
        }
        #endregion

        #region 是否以控制台的身份
        private bool isMastermind = false;
        public bool IsMastermind
        {
            get => isMastermind;
            set
            {
                SetProperty(ref isMastermind, value);
            }
        }
        #endregion

        #region 编辑中信息
        private string editingMessage;
        public string EditingMessage
        {
            get => editingMessage;
            set
            {
                SetProperty(ref editingMessage, value);
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

        #region 编辑中集群
        private string editingCluster;
        public string EditingCluster
        {
            get => editingCluster;
            set
            {
                SetProperty(ref editingCluster, value);
            }
        }
        #endregion

        #region 编辑中集群是否携带类别
        private bool isEditingClusterWithCategory;
        public bool IsEditingClusterWithCategory
        {
            get => isEditingClusterWithCategory;
            set
            {
                SetProperty(ref isEditingClusterWithCategory, value);
            }
        }
        #endregion

        #region 编辑中集群类别
        private string editingClusterCategory;
        public string EditingClusterCategory
        {
            get => editingClusterCategory;
            set
            {
                SetProperty(ref editingClusterCategory, value);
            }
        }
        #endregion

        #endregion
    }
}
