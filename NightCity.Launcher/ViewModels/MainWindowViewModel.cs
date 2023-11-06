using Newtonsoft.Json;
using NightCity.Core.Models.Standard;
using NightCity.Core.Services;
using NightCity.Launcher.Utilities;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NightCity.Launcher.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        //Http服务
        private readonly HttpService httpService;
        //活动优化服务
        private readonly ActionOptimizingService actionOptimizingService;
        public MainWindowViewModel()
        {
            //依赖注入及初始化
            httpService = new HttpService();
            actionOptimizingService = new ActionOptimizingService();
            Config = ConfigHelper.GetConfig(config);
            Config.ConfigChanged += ConfigChanged;
            Task.Run(async () =>
            {
                await SyncDeveloperNewsAsync();
            });
        }
        private async Task SyncDeveloperNewsAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Get($"{Config.DataSource}/api/application/max-tac/publish/GetDeveloperNews"));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<DeveloperNews> developerNewsList = JsonConvert.DeserializeObject<List<DeveloperNews>>(result.Content.ToString());
                Application.Current.Dispatcher.Invoke(() =>
                {
                    lock (DeveloperNewsList)
                    {
                        DeveloperNewsList.Clear();
                        foreach (var developerNews in developerNewsList)
                        {
                            DeveloperNewsList.Add(developerNews);
                        }
                    }
                });

                MessageHost.Hide();
            }
            catch (Exception e)
            {
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        #region 命令集合

        #region 命令：同步本地信息
        public ICommand SyncLocalInformationCommand
        {
            get => new DelegateCommand(SyncLocalInformation);
        }
        private void SyncLocalInformation()
        {

        }
        #endregion

        #region 命令：同步开发者新闻
        public ICommand SyncDeveloperNewsCommand
        {
            get => new DelegateCommand(SyncDeveloperNews);
        }
        private async void SyncDeveloperNews()
        {
            await SyncDeveloperNewsAsync();
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

        #endregion

        #region 可视化属性集合

        #region 配置
        private Config config = new Config
        {
            DataSource = "https://10.114.113.101"
        };
        public Config Config
        {
            get => config;
            set
            {
                SetProperty(ref config, value);
            }
        }
        #endregion

        #region 开发者新闻列表
        private ObservableCollection<DeveloperNews> developerNewsList = new ObservableCollection<DeveloperNews>();
        public ObservableCollection<DeveloperNews> DeveloperNewsList
        {
            get => developerNewsList;
            set
            {
                SetProperty(ref developerNewsList, value);
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

        private void ConfigChanged()
        {
            actionOptimizingService.Debounce(200, null, SaveConfigImmediately);
        }
        private void SaveConfigImmediately()
        {
            ConfigHelper.SetConfig(Config);
        }
    }
}
