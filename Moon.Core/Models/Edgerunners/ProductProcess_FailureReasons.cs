using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("product-process_failure-reasons")]
    public class ProductProcess_FailureReasons
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ProductProcess { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public string FailureReason { get; set; }
    }
}
