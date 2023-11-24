using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProgramLauncher.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private string text = "TestProgramLauncher.Main";
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
