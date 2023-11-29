using Prism.Mvvm;

namespace Calibration.Models.Standard
{
    public class CalibrationTerm : BindableBase
    {
        public string Name { get; set; }
        public string FileDirectory { get; set; }
        public string FileName { get; set; }
        public string ValidityPeriod { get; set; }
        public bool Optional { get; set; }

        private bool result;
        public bool Result
        {
            get => result;
            set => SetProperty(ref result, value);
        }
    }
}
