using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("ipc-clusters")]
    public class IPCClusters
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Mainboard { get; set; }
        public string? Category { get; set; }
        public string Cluster { get; set; }
        public DateTime CreateTime { get; set; }
        public string Creator { get; set; }
    }
}
