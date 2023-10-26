using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("users_office-pcs")]
    public class Users_OfficePCs
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string EmployeeId { get; set; }
        public string Mainboard { get; set; }
        public DateTime LinkTime { get; set; }
    }
}
