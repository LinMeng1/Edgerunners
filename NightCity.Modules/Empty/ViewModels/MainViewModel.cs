using Prism.Mvvm;

namespace Empty.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private string text = "Empty.Main V 1.0.0.1";
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
