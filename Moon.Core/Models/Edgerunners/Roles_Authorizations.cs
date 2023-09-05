using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("roles_authorizations")]
    public class Roles_Authorizations
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int RoleId { get; set; }

        [SugarColumn(IsPrimaryKey = true)]
        public int AuthorizationEnum { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
