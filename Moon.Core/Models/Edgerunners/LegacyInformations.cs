using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("legacy-informations")]
    public class LegacyInformations
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        public string Channel { get; set; }
        public string Destination { get; set; }
        public string Information { get; set; }
        public string FeedBack { get; set; }
        public string? Addition { get; set; }
        public DateTime Time { get; set; }
    }
}
