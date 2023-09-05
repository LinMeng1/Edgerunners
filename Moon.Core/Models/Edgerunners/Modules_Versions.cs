using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("modules_versions")]
    public class Modules_Versions
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int ModuleId { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public string Version { get; set; }
        public string ReleaseNotes { get; set; }
        public string Manifest { get; set; }
        public DateTime ReleaseTime { get; set; }
    }
}
