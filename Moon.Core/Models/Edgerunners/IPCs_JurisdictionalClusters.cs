using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("ipcs_jurisdictional-clusters")]
    public class IPCs_JurisdictionalClusters
    {
        public string Mainboard { get; set; }
        public string Category { get; set; }
        public string Cluster { get; set; }
        public string Owner { get; set; }
    }
}
