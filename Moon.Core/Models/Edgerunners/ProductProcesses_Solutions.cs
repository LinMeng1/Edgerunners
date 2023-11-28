using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("product-processes_solutions")]
    public class ProductProcesses_Solutions
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ProductProcess { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public string Solution { get; set; }
    }
}
