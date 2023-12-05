using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("connection-histories")]
    public class ConnectionHistories
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Client { get; set; }
        public bool IsConnected { get; set; }
        public DateTime Time { get; set; }
    }
}
