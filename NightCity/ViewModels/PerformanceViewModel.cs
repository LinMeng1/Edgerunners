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
        BasicInfomationService basicInfomationService;
        IPropertyService propertyService;
        public PerformanceViewModel(IPropertyService propertyService)
        {
            this.propertyService = propertyService;
            Task.Run(() =>
            {
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
            });
        }
        public ICommand UploadBasicInfomationCommand
        {
            get => new DelegateCommand(UploadBasicInfomation);
        }
        public async void UploadBasicInfomation()
        {
            await basicInfomationService.UploadAsync();
        }

        #region Basic Infomation

        #region Mainboard
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

        #region HostName
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

        #region HostMac
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

        #region HostAddress
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

        #region Domain
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

        #region Cpu
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

        #region Disk
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

        #region Memory
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

        #endregion

        #region Upload

        #region LastUploadTime
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

        #endregion
    }
}
