using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("users_roles")]
    public class Users_Roles
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string UserEmployeeId { get; set; }

        [SugarColumn(IsPrimaryKey = true)]
        public int RoleId { get; set; }
    }
}

