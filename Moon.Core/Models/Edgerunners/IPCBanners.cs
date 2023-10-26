using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("ipc-banners")]
    public class IPCBanners
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        public string Mainboard { get; set; }
        public string Urgency { get; set; }
        public int Priority { get; set; }
        public string? Category { get; set; }
        public string Content { get; set; }
        public string? LinkCommand { get; set; }
        public string? LinkInformation { get; set; }
        public bool Extensible { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
