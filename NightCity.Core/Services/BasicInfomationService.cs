using Newtonsoft.Json;
using NightCity.Core.Models;
using NightCity.Core.Models.Standard;
using System;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace NightCity.Core.Services
{
    public class BasicInfomationService
    {
        private string mainboard;
        private string hostName;
        private string hostMac;
        private string hostAddress;
        private string domain;
        private string cpu;
        private string disk;
        private string memory;
        private int scanIntervalSeconds;
        private DateTime lastUploadTime;
        private int uploadIntervalSeconds;
        private HttpService httpService;
        public BasicInfomationService(int scanIntervalSeconds, int uploadIntervalSeconds)
        {
            this.scanIntervalSeconds = scanIntervalSeconds;
            this.uploadIntervalSeconds = uploadIntervalSeconds;
            httpService = new HttpService();
            MonitorTask();
            UploadTask();
        }
        private void MonitorTask()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Global.Log($"[BasicInfomationService]:[MonitorTask] scanning basic infomation");
                    GetMainboard();
                    GetHostName();
                    GetHostMacAndAddress();
                    GetDomain();
                    GetCpu();
                    GetDisk();
                    GetMemory();
                    Thread.Sleep(scanIntervalSeconds * 1000);
                }
            });
        }
        private void UploadTask()
        {
            Task.Run(async () =>
            {
                while (mainboard == null || hostName == null || hostMac == null || hostAddress == null || domain == null || cpu == null || memory == null) { }
                while (true)
                {
                    await UploadAsync();
                    Thread.Sleep(uploadIntervalSeconds * 1000);
                }
            });
        }
        public async Task UploadAsync()
        {
            Global.Log($"[BasicInfomationService]:[UploadAsync]:uploading basic infomation");
            ControllersResult result = new ControllersResult();
            try
            {
                result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/performance/UploadBasicInformation", new Performance_UploadBasicInformation_Parameter()
                {
                    Mainboard = mainboard,
                    HostName = hostName,
                    HostMac = hostMac,
                    HostAddress = hostAddress,
                    Domain = domain,
                    Cpu = cpu,
                    Disk = disk,
                    Memory = memory,
                }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                Global.Log($"[BasicInfomationService]:[UploadAsync]:upload success");
            }
            catch (Exception e)
            {
                Global.Log($"[BasicInfomationService]:[UploadAsync]:exception:{e.Message}", true);
            }
            if (result.Result)
            {
                lastUploadTime = DateTime.Now;
                LastUploadTimeChanged?.Invoke(lastUploadTime);
            }
        }
        public delegate void MainboardChangedDelegate(string mainboard);
        public event MainboardChangedDelegate MainboardChanged;
        private void GetMainboard()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * from Win32_BIOS");
                foreach (ManagementObject mo in searcher.Get().Cast<ManagementObject>())
                {
                    mainboard = mo.GetPropertyValue("SerialNumber").ToString();
                    Global.Log($"[BasicInfomationService]:[GetMainboard]:{mainboard}");
                    MainboardChanged?.Invoke(mainboard);
                    break;
                }
            }
            catch (Exception e)
            {
                Global.Log($"[BasicInfomationService]:[GetMainboard] exception:{e.Message}", true);
            }
        }
        public delegate void HostNameChangedDelegate(string hostName);
        public event HostNameChangedDelegate HostNameChanged;
        private void GetHostName()
        {
            try
            {
                hostName = Dns.GetHostName();
                Global.Log($"[BasicInfomationService]:[GetHostName]:{hostName}");
                HostNameChanged?.Invoke(hostName);
            }
            catch (Exception e)
            {
                Global.Log($"[BasicInfomationService]:[GetHostName] exception:{e.Message}", true);
            }
        }
        public delegate void HostMacChangedDelegate(string hostMac);
        public event HostMacChangedDelegate HostMacChanged;
        public delegate void HostAddressChangedDelegate(string hostAddress);
        public event HostAddressChangedDelegate HostAddressChanged;
        private void GetHostMacAndAddress()
        {
            try
            {
                string Mac = string.Empty;
                string Ip = string.Empty;
                string[] effectiveNetworkSegment = { "10.114", "10.124" };
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    IPInterfaceProperties p = nic.GetIPProperties();
                    foreach (UnicastIPAddressInformation ip in p.UnicastAddresses)
                    {
                        string hostAddress = ip.Address.ToString();
                        string hostMac = nic.GetPhysicalAddress().ToString();
                        bool effective = false;
                        for (int i = 0; i < effectiveNetworkSegment.Length; i++)
                        {
                            if (hostAddress.IndexOf(effectiveNetworkSegment[i]) == 0)
                            {
                                effective = true;
                                break;
                            }
                        }
                        if (effective)
                        {
                            Mac = hostMac;
                            Ip = hostAddress;
                        }
                    }
                }
                hostMac = Mac;
                Global.Log($"[BasicInfomationService]:[GetHostMac]:{hostMac}");
                HostMacChanged?.Invoke(hostMac);
                hostAddress = Ip;
                Global.Log($"[BasicInfomationService]:[GetHostAddress]:{hostAddress}");
                HostAddressChanged?.Invoke(hostAddress);
            }
            catch (Exception e)
            {
                Global.Log($"[BasicInfomationService]:[GetHostMacAndAddress] exception:{e.Message}", true);
            }
        }
        public delegate void DomainChangedDelegate(string domain);
        public event DomainChangedDelegate DomainChanged;
        private void GetDomain()
        {
            try
            {
                domain = IPGlobalProperties.GetIPGlobalProperties().DomainName;
                Global.Log($"[BasicInfomationService]:[GetDomain]:{domain}");
                DomainChanged?.Invoke(domain);
            }
            catch (Exception e)
            {
                Global.Log($"[BasicInfomationService]:[GetDomain] exception:{e.Message}", true);
            }
        }
        public delegate void CpuChangedDelegate(string cpu);
        public event CpuChangedDelegate CpuChanged;
        private void GetCpu()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * from Win32_Processor");
                foreach (ManagementObject mo in searcher.Get())
                {
                    cpu = mo.GetPropertyValue("Name").ToString();
                    Global.Log($"[BasicInfomationService]:[GetCpu]:{cpu}");
                    CpuChanged?.Invoke(cpu);
                    break;
                }
            }
            catch (Exception e)
            {
                Global.Log($"[BasicInfomationService]:[GetCpu] exception:{e.Message}", true);
            }
        }
        public delegate void DiskChangedDelegate(string disk);
        public event DiskChangedDelegate DiskChanged;
        private void GetDisk()
        {
            try
            {
                string physicalDisk = string.Empty;
                ManagementScope scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM MSFT_PhysicalDisk");
                string type = string.Empty;
                scope.Connect();
                searcher.Scope = scope;
                int index = 0;
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    switch (Convert.ToInt16(queryObj["MediaType"]))
                    {
                        case 1:
                            type = "Unspecified";
                            break;

                        case 3:
                            type = "HDD";
                            break;

                        case 4:
                            type = "SSD";
                            break;

                        case 5:
                            type = "SCM";
                            break;

                        default:
                            type = "Unspecified";
                            break;
                    }
                    int size = (int)Math.Ceiling(Convert.ToDouble(queryObj["Size"]) / (1024 * 1024 * 1024));
                    if (index > 0)
                        physicalDisk += "+";
                    physicalDisk += $"{type} {size}GB";
                    index++;
                }
                searcher.Dispose();
                disk = physicalDisk;
                Global.Log($"[BasicInfomationService]:[GetDisk]:{disk}");
                DiskChanged?.Invoke(disk);
            }
            catch (Exception e)
            {
                Global.Log($"[BasicInfomationService]:[GetDisk] exception:{e.Message}", true);
            }
        }
        public delegate void MemoryChangedDelegate(string memory);
        public event MemoryChangedDelegate MemoryChanged;
        private void GetMemory()
        {
            try
            {
                string physicalMemory = string.Empty;
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * from Win32_PhysicalMemory");
                int index = 0;
                foreach (ManagementObject mo in searcher.Get())
                {
                    long size = Convert.ToInt64(mo.GetPropertyValue("Capacity").ToString());
                    int sizeGB = Convert.ToInt32(size / (1024 * 1024 * 1024));
                    if (index > 0)
                        physicalMemory += "+";
                    physicalMemory += $"{sizeGB}GB";
                    index++;
                }
                memory = physicalMemory;
                Global.Log($"[BasicInfomationService]:[GetMemory]:{memory}");
                MemoryChanged?.Invoke(memory);
            }
            catch (Exception e)
            {
                Global.Log($"[BasicInfomationService]:[GetMemory] exception:{e.Message}", true);
            }
        }
        public delegate void LastUploadTimeChangedDelegate(DateTime lastUploadTime);
        public event LastUploadTimeChangedDelegate LastUploadTimeChanged;
    }
}
