using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("ipc-issue-failure-categories")]
    public class IPCIssueFailureCategories
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
