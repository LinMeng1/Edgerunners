using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("roles")]
    public class Roles
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
