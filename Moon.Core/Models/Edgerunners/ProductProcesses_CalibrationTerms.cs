using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("product-processes_calibration-terms")]
    public class ProductProcesses_CalibrationTerms
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ProductProcess { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public string CalibrationTerm { get; set; }
        public string FileDirectory { get; set; }
        public string FileName { get; set; }
        public int ValidityPeriod { get; set; }
        public bool Optional { get; set; }
    }
}
