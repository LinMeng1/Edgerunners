using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("publications")]
    public class Publications
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Project { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public string Version { get; set; }
        public string ReleaseNotes { get; set; }
        public string Manifest { get; set; }
        public string ReleaseAddress { get; set; }
        public DateTime ReleaseTime { get; set; }
    }
}
