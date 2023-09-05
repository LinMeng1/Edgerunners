using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("organizations")]
    public class Organizations
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
    }
}
