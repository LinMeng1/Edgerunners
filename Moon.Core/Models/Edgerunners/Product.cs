using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("product")]
    public class Product
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string InternalName { get; set; }
        public string ExternalName { get; set; }
        public string Engineer { get; set; }
    }
}
