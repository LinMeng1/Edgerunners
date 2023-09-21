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
        //内置延迟
        private readonly int internalDelay = 500;
        //属性服务
        private readonly IPropertyService propertyService;
        //基础信息服务
        private readonly BasicInfomationService basicInfomationService;
        public PerformanceViewModel(IPropertyService propertyService)
        {
            this.propertyService = propertyService;
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
        }

        #region 命令集合

        #region 命令：上传基础信息
        public ICommand UploadBasicInfoCommand
        {
            get => new DelegateCommand(UploadBasicInfoAsync);
        }
        private async void UploadBasicInfoAsync()
        {
            DialogOpen = true;
            DialogCategory = "Syncing";
            await Task.Delay(internalDelay);
            await basicInfomationService.UploadAsync();
            DialogOpen = false;

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
