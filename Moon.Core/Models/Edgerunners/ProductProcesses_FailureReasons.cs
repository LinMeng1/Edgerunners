using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("product-processes_failure-reasons")]
    public class ProductProcesses_FailureReasons
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ProductProcess { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public string FailureReason { get; set; }
    }
}
