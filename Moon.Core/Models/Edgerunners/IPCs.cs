using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("ipcs")]
    public class IPCs
    {
        public string Mainboard { get; set; }
        public string HostName { get; set; }
        public string HostMac { get; set; }
        public string HostAddress { get; set; }
        public string Domain { get; set; }
        public string Cpu { get; set; }
        public string Disk { get; set; }
        public string Memory { get; set; }
        public DateTime UploadTime { get; set; }
        public string Version { get; set; }
    }
}
