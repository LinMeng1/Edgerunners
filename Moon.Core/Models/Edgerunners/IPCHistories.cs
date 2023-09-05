using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("ipc-histories")]
    public class IPCHistories
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Mainboard { get; set; }
        public string? HostName { get; set; }
        public string? HostMac { get; set; }
        public string? HostAddress { get; set; }
        public string? Domain { get; set; }
        public string? Cpu { get; set; }
        public string? Disk { get; set; }
        public string? Memory { get; set; }
        public DateTime UploadTime { get; set; }
    }
}
