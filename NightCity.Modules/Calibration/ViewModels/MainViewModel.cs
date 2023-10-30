using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibration.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private string text = "Calibration.Main";
        public string Text
        {
            get => text;
            set
            {
                SetProperty(ref text, value);
            }
        }
    }
}
