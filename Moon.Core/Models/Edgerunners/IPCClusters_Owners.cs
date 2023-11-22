using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("ipc-clusters_owners")]
    public class IPCClusters_Owners
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Cluster { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public string Category { get; set; }
        public string Owner { get; set; }
        public string Creator { get; set; }
    }
}
