using Prism.Mvvm;
using System.Windows.Media;

namespace NightCity.Launcher.Utilities
{
    public class LocalInstallInformation : BindableBase
    {
        public string DisplayIcon { get; set; }

        private ImageSource iconImage;
        public ImageSource IconImage
        {
            get => iconImage;
            set
            {
                SetProperty(ref iconImage, value);
            }
        }

        public string DisplayName { get; set; }

        private string displayVersion;
        public string DisplayVersion
        {
            get => displayVersion;
            set
            {
                SetProperty(ref displayVersion, value);
            }
        }

        private string latestVersion;
        public string LatestVersion
        {
            get => latestVersion;
            set
            {
                SetProperty(ref latestVersion, value);
            }
        }
        public string Publisher { get; set; }
        public string UninstallString { get; set; }

        private bool isInstalled;
        public bool IsInstalled
        {
            get => isInstalled;
            set
            {
                SetProperty(ref isInstalled, value);
            }
        }
    }
}
