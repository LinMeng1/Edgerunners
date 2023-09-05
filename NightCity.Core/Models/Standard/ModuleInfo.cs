using MaterialDesignThemes.Wpf;
using Prism.Mvvm;

namespace NightCity.Core.Models.Standard
{
    public class ModuleInfo : BindableBase
    {
        public string Name { get; set; }
        public string Category { get; set; }

        private PackIconKind icon;
        public PackIconKind Icon
        {
            get => icon;
            set => SetProperty(ref icon, value);
        }
        public string Author { get; set; }
        public string AuthorItCode { get; set; }
        public bool IsOfficial { get; set; }
        public string Description { get; set; }
        public string Manifest { get; set; }
        public string Version { get; set; }
    }
}
