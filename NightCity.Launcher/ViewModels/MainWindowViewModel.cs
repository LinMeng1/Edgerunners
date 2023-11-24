using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NightCity.Core;
using NightCity.Core.Models.Standard;
using NightCity.Core.Services;
using NightCity.Core.Utilities;
using NightCity.Launcher.Models;
using NightCity.Launcher.Utilities;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NightCity.Launcher.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        //Http服务
        private readonly HttpService httpService;
        //Sftp服务
        private readonly SftpService sftpService;
        //活动优化服务
        private readonly ActionOptimizingService actionOptimizingService;
        public MainWindowViewModel()
        {
            //依赖注入及初始化
            httpService = new HttpService();
            sftpService = new SftpService("10.114.113.101", 2022, Encryption.DecryptString("tonxrWIi+Dq/73qTwIQEKQ=="), Encryption.DecryptString("fxUxc7Rrk3op/6F1bBdmLw=="));
            actionOptimizingService = new ActionOptimizingService();
            Config = ConfigHelper.GetConfig(config);
            Config.ConfigChanged += ConfigChanged;
            Task.Run(async () =>
            {
                await ScanLocalInstallInformationAsync();
                await SyncDeveloperNewsAsync();
            });
        }
        /// <summary>
        /// 同步开发者新闻
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 扫描本地安装信息
        /// </summary>
        /// <returns></returns>
        private async Task ScanLocalInstallInformationAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                List<string> productList = LocalInstallInformationList.Select(it => it.DisplayName).ToList();
                List<LocalInstallInformation> localInstallInformationList = new List<LocalInstallInformation>();
                await Task.Run(() =>
                {
                    List<LocalInstallInformation> apps = InstalledPrograms.GetInstalledPrograms();
                    foreach (var app in apps)
                    {
                        if (productList.Contains(app.DisplayName))
                        {
                            localInstallInformationList.Add(app);
                        }
                    }
                });
                foreach (var localInstallInformation in localInstallInformationList)
                {
                    var info = LocalInstallInformationList.FirstOrDefault(it => it.DisplayName == localInstallInformation.DisplayName);
                    if (info != null)
                    {
                        info.DisplayIcon = localInstallInformation.DisplayIcon;
                        info.IconImage = localInstallInformation.IconImage ?? info.IconImage;
                        info.DisplayVersion = localInstallInformation.DisplayVersion;
                        info.Publisher = localInstallInformation.Publisher;
                        info.UninstallString = localInstallInformation.UninstallString;
                        info.LatestVersion = localInstallInformation.LatestVersion ?? info.LatestVersion;
                        info.IsInstalled = true;
                    }
                }
                foreach (var app in LocalInstallInformationList)
                {
                    ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post($"{Config.DataSource}/api/application/max-tac/publish/GetLatestRelease", new { Project = app.DisplayName }));
                    if (!result.Result) continue;
                    PublishInformation publish = JsonConvert.DeserializeObject<PublishInformation>(result.Content.ToString());
                    app.LatestVersion = publish.Version;
                    if (localInstallInformationList.FirstOrDefault(it => it.DisplayName == app.DisplayName) == null)
                    {
                        app.IsInstalled = false;
                        app.DisplayIcon = null;
                    }

                }
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }
        /// <summary>
        /// 扫描本地安装信息(后台)
        /// </summary>
        /// <returns></returns>
        private async Task ScanLocalInstallInformationAsyncBack()
        {
            try
            {
                await Task.Delay(MessageHost.InternalDelay);
                List<string> productList = LocalInstallInformationList.Select(it => it.DisplayName).ToList();
                List<LocalInstallInformation> localInstallInformationList = new List<LocalInstallInformation>();
                await Task.Run(() =>
                {
                    List<LocalInstallInformation> apps = InstalledPrograms.GetInstalledPrograms();
                    foreach (var app in apps)
                    {
                        if (productList.Contains(app.DisplayName))
                        {
                            localInstallInformationList.Add(app);
                        }
                    }
                });
                foreach (var localInstallInformation in localInstallInformationList)
                {
                    var info = LocalInstallInformationList.FirstOrDefault(it => it.DisplayName == localInstallInformation.DisplayName);
                    if (info != null)
                    {
                        info.DisplayIcon = localInstallInformation.DisplayIcon;
                        info.IconImage = localInstallInformation.IconImage ?? info.IconImage;
                        info.DisplayVersion = localInstallInformation.DisplayVersion;
                        info.Publisher = localInstallInformation.Publisher;
                        info.UninstallString = localInstallInformation.UninstallString;
                        info.LatestVersion = localInstallInformation.LatestVersion ?? info.LatestVersion;
                        info.IsInstalled = true;
                    }
                }
                foreach (var app in LocalInstallInformationList)
                {
                    ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post($"{Config.DataSource}/api/application/max-tac/publish/GetLatestRelease", new { Project = app.DisplayName }));
                    if (!result.Result) continue;
                    PublishInformation publish = JsonConvert.DeserializeObject<PublishInformation>(result.Content.ToString());
                    app.LatestVersion = publish.Version;
                    if (localInstallInformationList.FirstOrDefault(it => it.DisplayName == app.DisplayName) == null)
                    {
                        app.IsInstalled = false;
                        app.DisplayIcon = null;
                    }

                }
            }
            catch { }
        }

        /// <summary>
        /// 启动产品
        /// </summary>
        /// <param name="information"></param>
        /// <returns></returns>
        private async Task LaunchProductAsync(LocalInstallInformation information)
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                Process.Start(information.DisplayIcon);
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }
        /// <summary>
        /// 更新产品
        /// </summary>
        /// <param name="information"></param>
        /// <returns></returns>
        private async Task UpdateProductAsync(LocalInstallInformation information)
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post($"{Config.DataSource}/api/application/max-tac/publish/GetLatestRelease", new { Project = information.DisplayName }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                PublishInformation publish = JsonConvert.DeserializeObject<PublishInformation>(result.Content.ToString());
                string installPath = string.Empty;
                if (information == null || information.DisplayIcon == null)
                    installPath = Path.Combine(Config.InstallPath, "TECC", information.DisplayName);
                else
                    installPath = Path.GetDirectoryName(information.DisplayIcon);
                await KillProductAsync(information.DisplayName);
                await Task.Delay(1000);
                await Task.Run(() =>
                {
                    Directory.CreateDirectory(installPath);
                    JToken mainfest = JToken.Parse(publish.Manifest);
                    sftpService.SyncFiles(mainfest, publish.ReleaseAddress, string.Empty, installPath);
                    information.DisplayVersion = publish.Version;
                    information.Publisher = "LinMeng";
                    information.DisplayIcon = Path.Combine(installPath, $"{information.DisplayName}.exe");
                    information.UninstallString = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NightCity Launcher.exe"));
                    InstalledPrograms.CreateUninstallInRegistry(information);
                });
                if (information.DisplayName == "NightCity.Daemon")
                {
                    DeleteDeprecatedTaskScheduler();
                    CreateDaemonTaskScheduler(information);
                }
                await ScanLocalInstallInformationAsync();
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }
        /// <summary>
        /// 卸载产品
        /// </summary>
        /// <param name="information"></param>
        /// <returns></returns>
        private async Task RemoveProductAsync(LocalInstallInformation information)
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                string installPath = string.Empty;
                if (information == null || information.DisplayIcon == null)
                    throw new Exception("Installation path not found");
                else
                    installPath = Path.GetDirectoryName(information.DisplayIcon);
                InstalledPrograms.RemoveUninstallInRegistry(information);
                await KillProductAsync(information.DisplayName);
                await Task.Delay(1000);
                Directory.Delete(installPath, true);              
                if (information.DisplayName == "NightCity.Daemon")
                    DeleteDaemonTaskScheduler();
                await ScanLocalInstallInformationAsyncBack();
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }
        /// <summary>
        /// 集成启动
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        private async Task IntegratedLaunchAsync(string environment)
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                if (environment == "Office")
                {
                    DeleteDaemonTaskScheduler();
                    await KillProductAsync("NightCity.Daemon");
                    await ScanLocalInstallInformationAsync();
                    LocalInstallInformation nightCityInfo = LocalInstallInformationList.FirstOrDefault(it => it.DisplayName == "NightCity");
                    if (!nightCityInfo.IsInstalled)
                    {
                        await UpdateProductAsync(nightCityInfo);
                        await ScanLocalInstallInformationAsync();
                    }
                    await LaunchProductAsync(nightCityInfo);
                }
                else if (environment == "Production Line")
                {
                    LocalInstallInformation nightCityDaemonInfo = LocalInstallInformationList.FirstOrDefault(it => it.DisplayName == "NightCity.Daemon");
                    if (!nightCityDaemonInfo.IsInstalled)
                    {
                        await UpdateProductAsync(nightCityDaemonInfo);
                        await ScanLocalInstallInformationAsync();
                    }
                    CreateDaemonTaskScheduler(nightCityDaemonInfo);
                    await LaunchProductAsync(nightCityDaemonInfo);
                }
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }
        /// <summary>
        /// 集成卸载
        /// </summary>
        /// <returns></returns>
        private async Task IntegratedRemoveAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                await ScanLocalInstallInformationAsyncBack();
                foreach (var info in LocalInstallInformationList)
                {
                    if (info.IsInstalled)
                        await RemoveProductAsync(info);
                }
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }
        /// <summary>
        /// 关闭产品进程
        /// </summary>
        /// <param name="information"></param>
        private async Task KillProductAsync(string displayName)
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (displayName == "NightCity")
                        {
                            NamedPipeClientStream pipeClient = new NamedPipeClientStream("localhost", "NightCityPipe", PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.None);
                            pipeClient.Connect(1000);
                            StreamWriter sw = new StreamWriter(pipeClient)
                            {
                                AutoFlush = true
                            };
                            sw.WriteLine("NightCity Exit");
                            pipeClient.Dispose();
                        }

                    }
                    catch { }
                    foreach (Process p in Process.GetProcesses())
                    {
                        if (p.ProcessName.CompareTo(displayName) == 0)
                        {
                            p.Kill();
                            break;
                        }
                    }
                });
            }
            catch { }
        }
        /// <summary>
        /// 创建守护进程任务计划
        /// </summary>
        /// <param name="information"></param>
        /// <exception cref="Exception"></exception>
        private void CreateDaemonTaskScheduler(LocalInstallInformation information)
        {
            try
            {


                Microsoft.Win32.TaskScheduler.TaskFolder tf = Microsoft.Win32.TaskScheduler.TaskService.Instance.RootFolder;
                var localTask = tf.Tasks.FirstOrDefault(it => it.Name == "NightCity.Daemon Protection");
                if (localTask == null)
                {
                    Microsoft.Win32.TaskScheduler.TaskDefinition td = Microsoft.Win32.TaskScheduler.TaskService.Instance.NewTask();
                    td.RegistrationInfo.Description = "Please make sure you are using the latest version of NightCity software. If you disable or interrupt this task, your NightCity software won't be updated, which means potential security vulnerabilities won't be fixed and some features won't work properly. If no NightCity software uses this task, the task will uninstall itself.";
                    Microsoft.Win32.TaskScheduler.DailyTrigger dt = new Microsoft.Win32.TaskScheduler.DailyTrigger();
                    dt.StartBoundary = DateTime.Today;
                    dt.Repetition.Interval = TimeSpan.FromMinutes(1);
                    td.Triggers.Add(dt);
                    td.Actions.Add(information.DisplayIcon);
                    Microsoft.Win32.TaskScheduler.TaskService.Instance.RootFolder.RegisterTaskDefinition("NightCity.Daemon Protection", td);
                }
            }
            catch { }
        }
        /// <summary>
        /// 删除过期的任务计划
        /// </summary>
        private void DeleteDeprecatedTaskScheduler()
        {
            try
            {
                Microsoft.Win32.TaskScheduler.TaskFolder tf = Microsoft.Win32.TaskScheduler.TaskService.Instance.RootFolder;
                List<string> deprecatedTaskList = new List<string>();
                foreach (var task in tf.Tasks)
                {
                    if (task.Name.Contains("Guard2"))
                        deprecatedTaskList.Add(task.Name);
                }
                foreach (string deprecatedTask in deprecatedTaskList)
                {
                    tf.DeleteTask(deprecatedTask);
                }
            }
            catch { }
        }
        /// <summary>
        /// 删除守护进程任务计划
        /// </summary>
        private void DeleteDaemonTaskScheduler()
        {
            try
            {
                Microsoft.Win32.TaskScheduler.TaskFolder tf = Microsoft.Win32.TaskScheduler.TaskService.Instance.RootFolder;
                tf.DeleteTask("NightCity.Daemon Protection");
            }
            catch { }
        }

        #region 命令集合

        #region 命令：同步本地信息
        public ICommand SyncLocalInformationCommand
        {
            get => new DelegateCommand(SyncLocalInformation);
        }
        private async void SyncLocalInformation()
        {
            await ScanLocalInstallInformationAsync();
        }
        #endregion

        #region 命令：预设集成启动
        public ICommand IntegratedLaunchCommand
        {
            get => new DelegateCommand<string>(IntegratedLaunch);
        }
        private async void IntegratedLaunch(string environment)
        {
            await IntegratedLaunchAsync(environment);
        }
        #endregion

        #region 命令：预设集成卸载
        public ICommand IntegratedRemoveCommand
        {
            get => new DelegateCommand(IntegratedRemove);
        }
        private async void IntegratedRemove()
        {
            await IntegratedRemoveAsync();
        }
        #endregion

        #region 命令：启动产品
        public ICommand LaunchProductCommand
        {
            get => new DelegateCommand<LocalInstallInformation>(LaunchProduct);
        }
        private async void LaunchProduct(LocalInstallInformation information)
        {
            await LaunchProductAsync(information);
        }
        #endregion

        #region 命令：更新产品
        public ICommand UpdateProductCommand
        {
            get => new DelegateCommand<LocalInstallInformation>(UpdateProduct);
        }
        private async void UpdateProduct(LocalInstallInformation information)
        {
            await UpdateProductAsync(information);
        }
        #endregion

        #region 命令：卸载产品
        public ICommand RemoveProductCommand
        {
            get => new DelegateCommand<LocalInstallInformation>(RemoveProduct);
        }
        private async void RemoveProduct(LocalInstallInformation information)
        {
            await RemoveProductAsync(information);
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
            DataSource = @"https://10.114.113.101",
            InstallPath = @"C:\Program Files"
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

        #region 本地安装信息列表
        private ObservableCollection<LocalInstallInformation> localInstallInformationList = new ObservableCollection<LocalInstallInformation>()
        {
            new LocalInstallInformation()
            {
                DisplayName="NightCity.Daemon",
                IconImage=InstalledPrograms.GetStaticIcomImage(Properties.Resources.favicony),
                LatestVersion="Undefined"
            },
            new LocalInstallInformation()
            {
                DisplayName="NightCity",
                IconImage=InstalledPrograms.GetStaticIcomImage(Properties.Resources.favicon),
                LatestVersion="Undefined"
            }
        };
        public ObservableCollection<LocalInstallInformation> LocalInstallInformationList
        {
            get => localInstallInformationList;
            set
            {
                SetProperty(ref localInstallInformationList, value);
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
