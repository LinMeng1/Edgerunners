using Calibration.Models.Standard;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calibration.ViewModels
{
    public class ShortcutViewModel : BindableBase, IDisposable
    {
        //事件聚合器
        private IEventAggregator eventAggregator;
        //Http服务
        private HttpService httpService;
        //集群缓存
        private List<Connection_GetClusters_Result> clustersCache = new List<Connection_GetClusters_Result>();
        //监听token列表
        List<SubscriptionToken> eventTokens = new List<SubscriptionToken>();
        public ShortcutViewModel(IEventAggregator eventAggregator)
        {
            //依赖注入及初始化
            this.eventAggregator = eventAggregator;
            httpService = new HttpService();
            //监听事件 集群信息同步完成
            eventTokens.Add(eventAggregator.GetEvent<ClustersSyncedEvent>().Subscribe(async (clusters) =>
            {
                if (clusters != null)
                {
                    clustersCache = clusters;
                    await SyncCalibrationTermsAsync();
                }
            }, ThreadOption.UIThread));
            //监听事件 Mqtt信息接收
            eventTokens.Add(eventAggregator.GetEvent<MqttMessageReceivedEvent>().Subscribe(async (message) =>
            {
                if (!message.IsMastermind) return;
                string command = message.Content;
                if (command == "module calibration sync terms")
                    await SyncCalibrationTermsAsync();
            }, ThreadOption.UIThread, true));
            SyncCalibrationTerms();
        }

        /// <summary>
        /// 同步校准项目
        /// </summary>
        /// <returns></returns>
        private async Task SyncCalibrationTermsAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);

                if (Terms == null || Terms.Count == 0)
                {
                    Terms = new List<CalibrationTerm>()
                {
                    new CalibrationTerm()
                    {
                            Name="校准项1",
                FileDirectory="目录1",
                FileName="文件名1",
                ValidityPeriod="14",
                Optional=true
            },
                    new CalibrationTerm()
            {
                Name="校准项2",
                FileDirectory="目录3",
                FileName="文件名2",
                ValidityPeriod="90",
                Optional=false
            }
                };
                }
                else
                {
                    Terms = null;
                }

                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[Calibration]:[Shortcut]:[SyncCalibrationTermsAsync] exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        #region 命令集合

        #region 命令：同步校准项目
        public ICommand SyncCalibrationTermsCommand
        {
            get => new DelegateCommand(SyncCalibrationTerms);
        }
        private void SyncCalibrationTerms()
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

        #endregion

        #region 可视化属性集合

        #region 校准项列表
        private List<CalibrationTerm> terms = new List<CalibrationTerm>();
        public List<CalibrationTerm> Terms
        {
            get => terms;
            set
            {
                SetProperty(ref terms, value);
                IsTermsVisible = value == null || value.Count == 0;
            }
        }
        #endregion

        #region 是否显示校准项
        private bool isTermsVisible = false;
        public bool IsTermsVisible
        {
            get => isTermsVisible;
            set
            {
                SetProperty(ref isTermsVisible, value);
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
            httpService = null;
        }
    }
}
