using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("product-process")]
    public class ProductProcess
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
