using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("ipcs_modules")]
    public class IPCs_Modules
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string IPCMainboard { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public int ModuleId { get; set; }
        public string ModuleVersion { get; set; }
    }
}
