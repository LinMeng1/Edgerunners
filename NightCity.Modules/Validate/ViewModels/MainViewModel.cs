using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Validate.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel()
        {

            Thread.Sleep(10000);
        }

        private string text = "Validate.Main";
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
