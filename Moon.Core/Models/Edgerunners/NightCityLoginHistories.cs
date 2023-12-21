using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("night-city-login-histories")]
    public class NightCityLoginHistories
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string User { get; set; }
        public string Client { get; set; }
        public DateTime Time { get; set; }
    }
}
