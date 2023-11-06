using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("publish")]
    public class Publish
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
