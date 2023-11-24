using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("product-process_solutions")]
    public class ProductProcess_Solutions
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ProductProcess { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public string Solution { get; set; }
    }
}
