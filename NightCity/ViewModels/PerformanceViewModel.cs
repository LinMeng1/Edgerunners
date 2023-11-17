using Newtonsoft.Json;
using NightCity.Core;
using NightCity.Core.Models.Standard;
using NightCity.Core.Services;
using NightCity.Core.Services.Prism;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NightCity.ViewModels
{
    public class PerformanceViewModel : BindableBase
    {
        //属性服务
        private readonly IPropertyService propertyService;
        //Http服务
        private readonly HttpService httpService;
        //基础信息服务
        private readonly BasicInfomationService basicInfomationService;
        public PerformanceViewModel(IPropertyService propertyService)
        {
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //依赖注入及初始化
            this.propertyService = propertyService;
            httpService = new HttpService();
            basicInfomationService = new BasicInfomationService(30, 2 * 60 * 60);
            basicInfomationService.MainboardChanged += (mainboard) =>
            {
                Mainboard = mainboard;
            };
            basicInfomationService.HostNameChanged += (hostname) =>
            {
                HostName = hostname;
            };
            basicInfomationService.HostMacChanged += (hostmac) =>
            {
                HostMac = hostmac;
            };
            basicInfomationService.HostAddressChanged += (address) =>
            {
                HostAddress = address;
            };
            basicInfomationService.DomainChanged += (domain) =>
            {
                Domain = domain;
            };
            basicInfomationService.CpuChanged += (cpu) =>
            {
                Cpu = cpu;
            };
            basicInfomationService.DiskChanged += (disk) =>
            {
                Disk = disk;
            };
            basicInfomationService.MemoryChanged += (memory) =>
            {
                Memory = memory;
            };
            basicInfomationService.LastUploadTimeChanged += (lastUploadTime) =>
            {
                LastUploadTime = lastUploadTime;
            };
            //等待设备SN获取后
            Task.Run(async () =>
            {
                object mainboard = null;
                while (mainboard == null)
                {
                    mainboard = propertyService.GetProperty("Mainboard");
                    await Task.Delay(MessageHost.InternalDelay);
                }
                await UploadLaunchHistoryAsyncBack();
            });
        }

        /// <summary>
        /// 上传启动记录
        /// </summary>
        /// <returns></returns>
        private async Task UploadLaunchHistoryAsyncBack()
        {
            try
            {
                await Task.Delay(MessageHost.InternalDelay);
                string mainboard = propertyService.GetProperty("Mainboard").ToString();
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/performance/UploadNightCityVersion", new { Mainboard = mainboard, Version }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[UploadLaunchHistoryAsyncBack]:exception:{e.Message}", true);
            }
        }

        #region 命令集合

        #region 命令：上传基础信息
        public ICommand UploadBasicInfoCommand
        {
            get => new DelegateCommand(UploadBasicInfoAsync);
        }
        private async void UploadBasicInfoAsync()
        {
            MessageHost.Show();
            MessageHost.DialogCategory = "Syncing";
            await Task.Delay(MessageHost.InternalDelay);
            await basicInfomationService.UploadAsync();
            MessageHost.Hide();
        }
        #endregion

        #endregion

        #region 可视化属性集合

        #region 主板SN
        private string mainboard = "Mainboard";
        public string Mainboard
        {
            get => mainboard;
            set
            {
                SetProperty(ref mainboard, value);
                propertyService.SetProperty("Mainboard", value);
            }
        }
        #endregion

        #region 计算机名
        private string hostName = "HostName";
        public string HostName
        {
            get => hostName;
            set
            {
                SetProperty(ref hostName, value);
                propertyService.SetProperty("HostName", value);
            }
        }
        #endregion

        #region MAC地址
        private string hostMac = "HostMac";
        public string HostMac
        {
            get => hostMac;
            set
            {
                SetProperty(ref hostMac, value);
            }
        }
        #endregion

        #region IP地址
        private string hostAddress = "HostAddress";
        public string HostAddress
        {
            get => hostAddress;
            set
            {
                SetProperty(ref hostAddress, value);
            }
        }
        #endregion

        #region 工作域
        private string domain = "Domain";
        public string Domain
        {
            get => domain;
            set
            {
                SetProperty(ref domain, value);
            }
        }
        #endregion

        #region CPU型号
        private string cpu = "Cpu";
        public string Cpu
        {
            get => cpu;
            set
            {
                SetProperty(ref cpu, value);
            }
        }
        #endregion

        #region 硬盘型号
        private string disk = "Disk";
        public string Disk
        {
            get => disk;
            set
            {
                SetProperty(ref disk, value);
            }
        }
        #endregion

        #region 内存分布
        private string memory = "Memory";
        public string Memory
        {
            get => memory;
            set
            {
                SetProperty(ref memory, value);
            }
        }
        #endregion

        #region 上次上传时间
        private DateTime lastUploadTime;
        public DateTime LastUploadTime
        {
            get => lastUploadTime;
            set
            {
                SetProperty(ref lastUploadTime, value);
            }
        }
        #endregion

        #region 版本信息      
        private string version;
        public string Version
        {
            get => version;
            set
            {
                SetProperty(ref version, value);
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
