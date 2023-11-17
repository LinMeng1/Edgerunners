using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("night-city-launch-histories")]
    public class NightCityLaunchHistories
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Mainboard { get; set; }
        public string Version { get; set; }
        public DateTime Time { get; set; }
    }
}
