using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("product-processes")]
    public class ProductProcesses
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
