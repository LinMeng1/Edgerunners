using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("users")]
    public class Users
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string EmployeeId { get; set; }
        public string? ItCode { get; set; }
        public string? Password { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string? Email { get; set; }
        public string? Contact { get; set; }
        public int? Organization { get; set; }
    }
}
