using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NightCity.Core;
using NightCity.Core.Events;
using NightCity.Core.Models;
using NightCity.Core.Models.Standard;
using NightCity.Core.Services;
using NightCity.Core.Services.Prism;
using NightCity.Core.Utilities;
using NightCity.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ModuleInfo = NightCity.Core.Models.Standard.ModuleInfo;

namespace NightCity.ViewModels
{
    public class ModuleManagerViewModel : BindableBase
    {
        //事件聚合器
        private readonly IEventAggregator eventAggregator;
        //区域管理器
        private readonly IRegionManager regionManager;
        //模块目录
        private readonly DynamicDirectoryModuleCatalog moduleCatalog;
        //属性服务
        private readonly IPropertyService propertyService;
        //Sftp服务
        private readonly SftpService sftpService;
        //Http服务
        private readonly HttpService httpService;
        //活动优化服务
        private readonly ActionOptimizingService actionOptimizingService;
        //活动窗口集合
        private readonly ConcurrentDictionary<string, Template> windows = new ConcurrentDictionary<string, Template>();
        public ModuleManagerViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, IModuleCatalog moduleCatalog, IPropertyService propertyService)
        {
            //依赖注入及初始化
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.moduleCatalog = (DynamicDirectoryModuleCatalog)moduleCatalog;
            this.propertyService = propertyService;
            //propertyService.SetProperty("ModuleWindows", windows);
            sftpService = new SftpService("10.114.113.101", 2022, Encryption.DecryptString("tonxrWIi+Dq/73qTwIQEKQ=="), Encryption.DecryptString("fxUxc7Rrk3op/6F1bBdmLw=="));
            httpService = new HttpService();
            actionOptimizingService = new ActionOptimizingService();
            //监听事件 Mqtt连接/断开
            eventAggregator.GetEvent<MqttConnectedEvent>().Subscribe(async (isConnected) =>
            {
                if (isConnected)
                {
                    await SyncInstalledModulesAsync();
                    await SyncBrowseModulesAsync();
                }
            }, ThreadOption.UIThread);
            //监听事件 活动窗口关闭
            eventAggregator.GetEvent<TemplateClosingEvent>().Subscribe((module) =>
            {
                windows.TryGetValue(module, out Template currentWindow);
                currentWindow?.Close();
            }, ThreadOption.UIThread, true);
            //监听事件 活动窗口重新打开
            eventAggregator.GetEvent<TemplateReOpeningEvent>().Subscribe(async (moduleName) =>
            {
                await LaunchModuleAsyncBack(moduleName);
            }, ThreadOption.UIThread, true);
            //监听事件 活动窗口显示
            eventAggregator.GetEvent<TemplateShowingEvent>().Subscribe(async (moduleName) =>
            {
                await LaunchModuleAsyncBack(moduleName, true);
            });
            //监听事件 Mqtt信息接收
            eventAggregator.GetEvent<MqttMessageReceivedEvent>().Subscribe(async (message) =>
            {
                if (!message.IsMastermind) return;
                string command = message.Content;
                if (command == "system sync modules")
                    await SyncInstalledModulesAsync();
            }, ThreadOption.UIThread);
            //监听事件 权限信息变更
            eventAggregator.GetEvent<AuthorizationInfoChangedEvent>().Subscribe((authorizationInfo) =>
            {
                object token = propertyService.GetProperty(authorizationInfo.Item2);
                httpService.AddToken(token?.ToString());
            }, ThreadOption.UIThread, true);
            //监听事件 尝试发布链接
            eventAggregator.GetEvent<BannerMessageTryLinkingEvent>().Subscribe((linkInfo) =>
            {
                Link(linkInfo.Item1, linkInfo.Item2);
            }, ThreadOption.UIThread);
            //监听事件 错误信息抛出
            eventAggregator.GetEvent<ErrorMessageShowingEvent>().Subscribe((errorMessage) =>
            {
                Views.MessageBox mb = new Views.MessageBox(errorMessage.Item1, $"Error from {errorMessage.Item2}", MessageBoxButton.OK, MessageBoxImage.Error);
                mb.ShowDialog();
            });
        }

        /// <summary>
        /// 同步已安装模块列表
        /// </summary>
        /// <param name="mainboard">设备SN</param>
        /// <returns></returns>
        private async Task SyncInstalledModulesAsync(string mainboard = "")
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                if (mainboard == string.Empty)
                    mainboard = propertyService.GetProperty("Mainboard").ToString();
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/module-manager/GetModules", new { Mainboard = mainboard }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<ModuleManager_GetModules_Result> mods = JsonConvert.DeserializeObject<List<ModuleManager_GetModules_Result>>(result.Content.ToString());
                Application.Current.Dispatcher.Invoke(() =>
                {
                    lock (InstalledModules)
                    {
                        InstalledModules.Clear();
                        foreach (var mod in mods)
                        {
                            InstalledModules.Add(new ModuleInfo()
                            {
                                Name = mod.Name,
                                Category = mod.Category,
                                Icon = mod.Category == "Authorization" ? (PackIconKind)Enum.Parse(typeof(PackIconKind), "FingerprintOff") : (PackIconKind)Enum.Parse(typeof(PackIconKind), mod.Icon),
                                Author = mod.Author,
                                AuthorItCode = mod.AuthorItCode,
                                IsOfficial = mod.IsOfficial,
                                Description = mod.Description,
                                Manifest = mod.Manifest,
                                Version = mod.Version,
                                IsVisible = true
                            });
                        }
                        FilterInstalledModulesImmediately();
                    }
                });
                eventAggregator.GetEvent<ModulesChangedEvent>().Publish(InstalledModules);
                if (InstalledModules.Count == 0)
                    TabSelectedIndex = 1;
                await SyncLocalModulesAsync();
                if (InstalledSelectedModule != null)
                    await SyncModuleVersionsAsync(InstalledSelectedModule.Name, "Installed");
                await DisposeExpireModuleAsyncBack();
                await OpenModuleAsyncBack();
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[ModuleManager]:[SyncModules] exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 同步模块库模块列表
        /// </summary>
        /// <returns></returns>
        private async Task SyncBrowseModulesAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Get("https://10.114.113.101/api/application/night-city/module-manager/GetAllModules"));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<ModuleManager_GetAllModules_Result> mods = JsonConvert.DeserializeObject<List<ModuleManager_GetAllModules_Result>>(result.Content.ToString());
                Application.Current.Dispatcher.Invoke(() =>
                {
                    lock (BrowseModules)
                    {
                        BrowseModules.Clear();
                        foreach (var mod in mods)
                        {
                            BrowseModules.Add(new ModuleInfo()
                            {
                                Name = mod.Name,
                                Category = mod.Category,
                                Icon = mod.Category == "Authorization" ? (PackIconKind)Enum.Parse(typeof(PackIconKind), "FingerprintOff") : (PackIconKind)Enum.Parse(typeof(PackIconKind), mod.Icon),
                                Author = mod.Author,
                                AuthorItCode = mod.AuthorItCode,
                                IsOfficial = mod.IsOfficial,
                                Description = mod.Description,
                                IsVisible = true
                            });
                        }
                        FilterBrowseModulesImmediately();
                    }
                });
                if (BrowseSelectedModule != null && BrowseSelectedModule.Name != null)
                    BrowseSelectedModule = BrowseModules.FirstOrDefault(it => it.Name == BrowseSelectedModule.Name);
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[ModuleManager]:[SyncModules] exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 同步本地模块文件
        /// </summary>
        /// <returns></returns>
        private async Task SyncLocalModulesAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                await Task.Run(() =>
                {
                    DirectoryInfo directory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules"));
                    if (!directory.Exists)
                        Directory.CreateDirectory(directory.FullName);
                    foreach (DirectoryInfo dir in directory.GetDirectories())
                    {
                        if (InstalledModules.FirstOrDefault(it => it.Name == dir.Name && it.Manifest != null) == null)
                        {
                            try { dir.Delete(true); } catch { }
                            continue;
                        }
                        foreach (DirectoryInfo dirchild in dir.GetDirectories())
                        {
                            if (InstalledModules.FirstOrDefault(it => it.Name == dir.Name && it.Version == dirchild.Name) == null)
                                try { dir.Delete(true); } catch { }
                        }
                    }
                    foreach (ModuleInfo mod in InstalledModules)
                    {
                        if (!Directory.Exists($"{directory}/{mod.Name}/{mod.Version}") || mod.Manifest == null)
                        {
                            sftpService.PullFiles($"/NightCity.Modules/{mod.Name}/{mod.Version}", $"{directory}/{mod.Name}/{mod.Version}");
                            continue;
                        }
                        JToken mainfest = JToken.Parse(mod.Manifest);
                        sftpService.SyncFiles(mainfest, $"/NightCity.Modules/{mod.Name}/{mod.Version}", string.Empty, $"{directory}/{mod.Name}/{mod.Version}");
                    }
                });
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[ModuleManager]:[SyncLocalModulesAsync] exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <param name="mainboard">设备SN</param>
        /// <returns></returns>
        private async Task UninstallModulesAsync(string moduleName)
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                string mainboard = propertyService.GetProperty("Mainboard").ToString();
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/module-manager/UninstallModule", new { Mainboard = mainboard, ModuleName = moduleName }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                await SyncInstalledModulesAsync();
                await SyncBrowseModulesAsync();
                InstalledSelectedModule = null;
                InstalledSelectedModuleVersions.Clear();
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[ModuleManager]:[UninstallModulesAsync] exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 同步已安装模块所选模块版本列表
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task SyncModuleVersionsAsync(string moduleName, string category)
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/module-manager/GetModuleVersions", new { Module = moduleName }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<ModuleManager_GetModuleVersions_Result> versions = JsonConvert.DeserializeObject<List<ModuleManager_GetModuleVersions_Result>>(result.Content.ToString());
                Application.Current.Dispatcher.Invoke(() =>
                {
                    switch (category)
                    {
                        case "Installed":
                            InstalledSelectedModuleVersions.Clear();
                            InstalledSelectedModuleSelectedVersion = new ModuleVersionInfo();
                            foreach (var version in versions)
                            {
                                ModuleVersionInfo versionx = new ModuleVersionInfo()
                                {
                                    Version = version.Version,
                                    ReleaseNotes = version.ReleaseNotes,
                                    Manifest = version.Manifest,
                                    ReleaseTime = version.ReleaseTime,
                                };
                                InstalledSelectedModuleVersions.Add(versionx);
                                if (InstalledModules.FirstOrDefault(it => it.Name == moduleName && it.Version == version.Version) != null)
                                    InstalledSelectedModuleSelectedVersion = versionx;
                            }
                            break;
                        case "Browse":
                            BrowseSelectedModuleVersions.Clear();
                            BrowseSelectedModuleSelectedVersion = new ModuleVersionInfo();
                            foreach (var version in versions)
                            {
                                ModuleVersionInfo versionx = new ModuleVersionInfo()
                                {
                                    Version = version.Version,
                                    ReleaseNotes = version.ReleaseNotes,
                                    Manifest = version.Manifest,
                                    ReleaseTime = version.ReleaseTime,
                                };
                                BrowseSelectedModuleVersions.Add(versionx);
                                if (BrowseModules.FirstOrDefault(it => it.Name == moduleName && it.Version == version.Version) != null)
                                    BrowseSelectedModuleSelectedVersion = versionx;
                            }
                            if (BrowseSelectedModuleSelectedVersion.Version == null && BrowseSelectedModuleVersions.Count > 0)
                                BrowseSelectedModuleSelectedVersion = BrowseSelectedModuleVersions.LastOrDefault();
                            break;
                        default:
                            break;
                    }
                });
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[ModuleManager]:[SyncModuleVersionsAsync] exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 更新模块
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <param name="version">模块版本</param>
        /// <returns></returns>
        private async Task UpdateModuleAsync(string moduleName, string moduleVersion, bool isAuthorize = false)
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                string mainboard = propertyService.GetProperty("Mainboard").ToString();
                ControllersResult result = null;
                if (isAuthorize)
                    result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/module-manager/InstallAuthorizeModule", new { Mainboard = mainboard, ModuleName = moduleName, ModuleVersion = moduleVersion }));
                else
                    result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/module-manager/InstallModule", new { Mainboard = mainboard, ModuleName = moduleName, ModuleVersion = moduleVersion }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                await SyncInstalledModulesAsync();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    InstalledSelectedModule = InstalledModules.FirstOrDefault(it => it.Name == moduleName);
                });
                await SyncModuleVersionsAsync(moduleName, "Installed");
                await SyncBrowseModulesAsync();
                CheckLoadedModuleVersionAsync(moduleName, moduleVersion);               
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[ModuleManager]:[UpdateModuleAsync] exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 静默开启模块
        /// 类型为Monitor的需要默认开启
        /// </summary>
        /// <returns></returns>
        private async Task OpenModuleAsyncBack()
        {
            try
            {
                await Task.Run(() =>
                {
                    foreach (var mod in InstalledModules)
                    {
                        if (mod.Category != "Monitor") continue;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            windows.TryGetValue(mod.Name, out Template currentWindow);
                            if (currentWindow == null)
                            {
                                Template template = new Template(regionManager, moduleCatalog, mod, LoadedModules, out bool loadResult);
                                if (loadResult)
                                    windows.AddOrUpdate(mod.Name, template, (xkey, xvalue) => template);
                            }
                        });
                    }
                });
            }
            catch (Exception e)
            {
                Global.Log($"[ModuleManager]:[OpenModuleAsync] exception:{e.Message}", true);
            }
        }

        /// <summary>
        /// 处理超期模块
        /// 当同步或同步操作导致活动模块处于合法已安装模块列表外时，处理之
        /// </summary>
        /// <returns></returns>
        private async Task DisposeExpireModuleAsyncBack()
        {
            try
            {
                await Task.Run(() =>
                {
                    foreach (var window in windows)
                    {
                        if (InstalledModules.FirstOrDefault(it => it.Name == window.Key && it.Version == window.Value.module.Version) == null)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (window.Value.IsVisible)
                                {
                                    Views.MessageBox mb = new Views.MessageBox($"Module:{window.Value.module.Name} with version:{window.Value.module.Version} error detected, will close the module window", $"Module error detected", MessageBoxButton.OK, MessageBoxImage.Error);
                                    mb.ShowDialog();
                                }
                                window.Value?.Disauthorize();
                                window.Value?.Dispose();
                                window.Value?.Close();
                                windows.TryRemove(window.Key, out Template template);
                            });
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Global.Log($"[ModuleManager]:[DisposeExpireModuleAsync] exception:{e.Message}", true);
            }
        }

        /// <summary>
        /// 检查已载入模块的版本
        /// 若已载入模块版本与设置版本不符，则抛出异常
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <param name="moduleVersion">模块版本</param>
        /// <exception cref="Exception"></exception>
        private void CheckLoadedModuleVersionAsync(string moduleName, string moduleVersion)
        {
            MessageHost.Show();
            ModuleInfo mod = LoadedModules.FirstOrDefault(it => it.Name == moduleName);
            if (mod == null) return;
            if (mod.Version != moduleVersion)
                throw new Exception($"The set version ({moduleVersion}) could not be applied due to an old version ({mod.Version}) of the module being loaded, please reload NightCity in due course to reload module");            
        }

        /// <summary>
        /// 启动模块
        /// </summary>
        /// <param name="module"></param>
        private async Task LaunchModuleAsyncBack(string moduleName, bool showing = false)
        {
            await Task.Run(() =>
            {
                ModuleInfo module = InstalledModules.FirstOrDefault(it => it.Name == moduleName);
                if (module == null) return;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    windows.TryGetValue(moduleName, out Template currentWindow);
                    if (showing)
                        currentWindow?.Show();
                    else
                    {
                        currentWindow?.Dispose();
                        currentWindow?.Close();
                        Template template = new Template(regionManager, moduleCatalog, module, LoadedModules, out bool loadResult);
                        windows.AddOrUpdate(moduleName, template, (xkey, xvalue) => template);
                        template.Show();
                    }
                });
            });
        }

        /// <summary>
        /// 发布链接命令
        /// </summary>
        /// <param name="linkCommand">链接命令</param>
        /// <param name="category">链接类型（模块）</param>
        private void Link(string category, string linkCommand)
        {
            if (linkCommand == null)
            {
                Views.MessageBox mb = new Views.MessageBox("Link command not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                mb.ShowDialog();
                return;
            }
            ModuleInfo mod = InstalledModules.FirstOrDefault(it => it.Name == category);
            if (mod == null)
            {
                Views.MessageBox mb = new Views.MessageBox($"You do not have the required response module:{category} installed", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                mb.ShowDialog();
                return;
            }
            windows.TryGetValue(category, out Template currentWindow);
            if (currentWindow == null)
            {
                Template template = new Template(regionManager, moduleCatalog, mod, LoadedModules, out bool loadResult);
                if (loadResult)
                    windows.AddOrUpdate(mod.Name, template, (xkey, xvalue) => template);
                else
                {
                    Views.MessageBox mb = new Views.MessageBox($"Load module({mod.Name}) maybe fail, You should wait for the module to load or start the module yourself ", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    mb.ShowDialog();
                }
            }
            eventAggregator.GetEvent<BannerMessageLinkingEvent>().Publish(linkCommand);
        }

        /// <summary>
        /// 筛选已安装模块列表
        /// </summary>
        /// <param name="category"></param>
        private void FilterInstalledModulesImmediately()
        {
            foreach (var module in InstalledModules)
            {
                if (!module.Name.ToUpper().Contains(InstalledModulesFilterText == null ? string.Empty : InstalledModulesFilterText.Trim().ToUpper()))
                    module.IsVisible = false;
                else
                    module.IsVisible = true;
            }
        }

        /// <summary>
        /// 筛选模块库模块列表
        /// </summary>
        private void FilterBrowseModulesImmediately()
        {
            foreach (var module in BrowseModules)
            {
                if (!module.Name.ToUpper().Contains(BrowseModulesFilterText == null ? string.Empty : BrowseModulesFilterText.Trim().ToUpper()))
                    module.IsVisible = false;
                else if (!module.IsOfficial && BrowseModulesFilterIsOfficial)
                    module.IsVisible = false;
                else
                    module.IsVisible = true;
            }
        }

        #region 命令集合 

        #region 命令：同步已安装模块列表
        public ICommand SyncInstalledModulesCommand
        {
            get => new DelegateCommand(SyncInstalledModules);
        }
        private async void SyncInstalledModules()
        {
            await SyncInstalledModulesAsync();
        }
        #endregion

        #region 命令：筛选已安装模块列表
        public ICommand FilterInstalledModulesCommand
        {
            get => new DelegateCommand(FilterInstalledModules);
        }
        private void FilterInstalledModules()
        {
            actionOptimizingService.Debounce(200, null, FilterInstalledModulesImmediately);
        }
        #endregion

        #region 命令：同步模块库模块列表
        public ICommand SyncBrowseModulesCommand
        {
            get => new DelegateCommand(SyncBrowseModules);
        }
        private async void SyncBrowseModules()
        {
            await SyncBrowseModulesAsync();
        }
        #endregion

        #region 命令：筛选模块库模块列表
        public ICommand FilterBrowseModulesCommand
        {
            get => new DelegateCommand(FilterBrowseModules);
        }
        private void FilterBrowseModules()
        {
            actionOptimizingService.Debounce(200, null, FilterBrowseModulesImmediately);
        }
        #endregion

        #region 命令：模块库模块列表切换只显示官方模块
        public ICommand SwitchBrowseModulesOfficialOnlyCommand
        {
            get => new DelegateCommand(SwitchBrowseModulesOfficialOnly);
        }
        private void SwitchBrowseModulesOfficialOnly()
        {
            BrowseModulesFilterIsOfficial = !BrowseModulesFilterIsOfficial;
            FilterBrowseModulesImmediately();
        }
        #endregion

        #region 命令：已安装模块选中
        public ICommand InstalledModuleClickCommand
        {
            get => new DelegateCommand<ModuleInfo>(InstalledModuleClick);
        }
        private async void InstalledModuleClick(ModuleInfo mod)
        {
            if (MessageHost.DialogOpen) return;
            InstalledSelectedModule = mod;
            await SyncModuleVersionsAsync(mod.Name, "Installed");
        }
        #endregion

        #region 命令：模块库模块选中
        public ICommand BrowseModuleClickCommand
        {
            get => new DelegateCommand<ModuleInfo>(BrowseModuleClick);
        }
        private async void BrowseModuleClick(ModuleInfo mod)
        {
            BrowseSelectedModule = mod;
            await SyncModuleVersionsAsync(mod.Name, "Browse");
        }
        #endregion

        #region 命令：打开模块窗口
        public ICommand LaunchModuleCommand
        {
            get => new DelegateCommand<string>(LaunchModule);
        }
        private async void LaunchModule(string moduleName)
        {
            await LaunchModuleAsyncBack(moduleName);
        }
        #endregion

        #region 命令：更新模块
        public ICommand UpdateModuleCommand
        {
            get => new DelegateCommand<object>(UpdateModule);
        }
        private async void UpdateModule(object parameter)
        {
            string moduleName = ((object[])parameter)[0].ToString();
            string moduleVersion = ((object[])parameter)[1].ToString();
            string moduleCategory = ((object[])parameter)[2].ToString();
            if (moduleCategory == "Authorization")
                await UpdateModuleAsync(moduleName, moduleVersion, true);
            else
                await UpdateModuleAsync(moduleName, moduleVersion);
        }
        #endregion

        #region 命令：卸载模块询问
        public ICommand TryUninstallModuleCommand
        {
            get => new DelegateCommand(TryUninstallModule);
        }
        private void TryUninstallModule()
        {
            MessageHost.Show();
            MessageHost.DialogMessage = "Are you sure you want to uninstall the module";
            MessageHost.DialogCategory = "Uninstall";
        }
        #endregion

        #region 命令：卸载模块
        public ICommand UninstallModuleCommand
        {
            get => new DelegateCommand(UninstallModule);
        }
        private async void UninstallModule()
        {
            await UninstallModulesAsync(InstalledSelectedModule.Name);
        }
        #endregion

        #region 命令：展示模块文件清单
        public ICommand ShowModuleManifestCommand
        {
            get => new DelegateCommand<string>(ShowModuleManifest);
        }
        private void ShowModuleManifest(string category)
        {
            MessageHost.Show();
            switch (category)
            {
                case "Installed":
                    MessageHost.DialogCategory = "Manifest Installed";
                    break;
                case "Browse":
                    MessageHost.DialogCategory = "Manifest Browse";
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 命令：确认并关闭模块文件清单对话框
        public ICommand ClearMessageAndCopyManifestCommand
        {
            get => new DelegateCommand<string>(ClearMessageAndCopyManifest);
        }
        private void ClearMessageAndCopyManifest(string category)
        {
            switch (category)
            {
                case "Installed":
                    if (InstalledSelectedModuleSelectedVersion.Manifest != null)
                        Clipboard.SetText(InstalledSelectedModuleSelectedVersion.Manifest);
                    break;
                case "Browse":
                    if (BrowseSelectedModuleSelectedVersion.Manifest != null)
                        Clipboard.SetText(BrowseSelectedModuleSelectedVersion.Manifest);
                    break;
                default:
                    break;
            }
            CleanMessage();
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

        #region 命令：重载NightCity
        public ICommand ReloadNightCityCommand
        {
            get => new DelegateCommand(ReloadNightCity);
        }
        public void ReloadNightCity()
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }
        #endregion

        #endregion

        #region 可视化属性集合

        #region 已加载模块列表
        private ObservableCollection<ModuleInfo> loadedModules = new ObservableCollection<ModuleInfo>();
        public ObservableCollection<ModuleInfo> LoadedModules
        {
            get => loadedModules;
            set
            {
                SetProperty(ref loadedModules, value);
            }
        }
        #endregion

        #region 已安装模块列表
        private ObservableCollection<ModuleInfo> installedModules = new ObservableCollection<ModuleInfo>();
        public ObservableCollection<ModuleInfo> InstalledModules
        {
            get => installedModules;
            set
            {
                SetProperty(ref installedModules, value);
            }
        }
        #endregion

        #region 已安装模块筛选文本
        private string installedModulesFilterText;
        public string InstalledModulesFilterText
        {
            get => installedModulesFilterText;
            set
            {
                SetProperty(ref installedModulesFilterText, value);
            }
        }
        #endregion

        #region 模块库模块列表
        private ObservableCollection<ModuleInfo> browseModules = new ObservableCollection<ModuleInfo>();
        public ObservableCollection<ModuleInfo> BrowseModules
        {
            get => browseModules;
            set
            {
                SetProperty(ref browseModules, value);
            }
        }
        #endregion

        #region 模块库模块筛选文本
        private string browseModulesFilterText;
        public string BrowseModulesFilterText
        {
            get => browseModulesFilterText;
            set
            {
                SetProperty(ref browseModulesFilterText, value);
            }
        }
        #endregion

        #region 模块库模块官方模块筛选
        private bool browseModulesFilterIsOfficial;
        public bool BrowseModulesFilterIsOfficial
        {
            get => browseModulesFilterIsOfficial;
            set
            {
                SetProperty(ref browseModulesFilterIsOfficial, value);
            }
        }
        #endregion

        #region 已安装模块所选模块
        private ModuleInfo installedSelectedModule;
        public ModuleInfo InstalledSelectedModule
        {
            get => installedSelectedModule;
            set
            {
                SetProperty(ref installedSelectedModule, value);
            }
        }
        #endregion

        #region 模块库模块所选模块
        private ModuleInfo browseSelectedModule;
        public ModuleInfo BrowseSelectedModule
        {
            get => browseSelectedModule;
            set
            {
                SetProperty(ref browseSelectedModule, value);
            }
        }
        #endregion

        #region 已安装模块所选模块版本列表
        private ObservableCollection<ModuleVersionInfo> installedSelectedModuleVersions = new ObservableCollection<ModuleVersionInfo>();
        public ObservableCollection<ModuleVersionInfo> InstalledSelectedModuleVersions
        {
            get => installedSelectedModuleVersions;
            set
            {
                SetProperty(ref installedSelectedModuleVersions, value);
            }
        }
        #endregion

        #region 模块库模块所选模块版本列表
        private ObservableCollection<ModuleVersionInfo> browseSelectedModuleVersions = new ObservableCollection<ModuleVersionInfo>();
        public ObservableCollection<ModuleVersionInfo> BrowseSelectedModuleVersions
        {
            get => browseSelectedModuleVersions;
            set
            {
                SetProperty(ref browseSelectedModuleVersions, value);
            }
        }

        #endregion

        #region 已安装模块所选模块所选版本
        private ModuleVersionInfo installedSelectedModuleSelectedVersion = new ModuleVersionInfo();
        public ModuleVersionInfo InstalledSelectedModuleSelectedVersion
        {
            get => installedSelectedModuleSelectedVersion;
            set
            {
                SetProperty(ref installedSelectedModuleSelectedVersion, value);
            }
        }
        #endregion

        #region 模块库模块所选模块所选版本
        private ModuleVersionInfo browseSelectedModuleSelectedVersion = new ModuleVersionInfo();
        public ModuleVersionInfo BrowseSelectedModuleSelectedVersion
        {
            get => browseSelectedModuleSelectedVersion;
            set
            {
                SetProperty(ref browseSelectedModuleSelectedVersion, value);
            }
        }

        #endregion

        #region 标签选中索引
        private int tabSelectedIndex = 0;
        public int TabSelectedIndex
        {
            get => tabSelectedIndex;
            set
            {
                SetProperty(ref tabSelectedIndex, value);
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
