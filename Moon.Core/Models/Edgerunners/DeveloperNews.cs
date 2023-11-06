using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("developer-news")]
    public class DeveloperNews
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
    }
}
