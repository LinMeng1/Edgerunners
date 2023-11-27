using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("tcn-test")]
    public class TcnTest
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        public string CRNumber { get; set; }
        public string Title { get; set; }
        public string Product { get; set; }
        public string CommentMessage { get; set; }
        public string WHPFactoryUser { get; set; }
        public DateTime StartTime { get; set; }
        public string Creator { get; set; }
        public string? Attachments { get; set; }
    }
}
