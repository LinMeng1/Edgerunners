using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("products")]
    public class Products
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string InternalName { get; set; }
        public string ExternalName { get; set; }
        public string Engineer { get; set; }
    }
}
