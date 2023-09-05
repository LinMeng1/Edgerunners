using Prism.Mvvm;
using System;

namespace NightCity.Core.Models.Standard
{
    public class ModuleVersionInfo : BindableBase
    {
        public string Version { get; set; }

        private string releaseNotes;
        public string ReleaseNotes
        {
            get => releaseNotes;
            set
            {
                SetProperty(ref releaseNotes, value);
            }
        }

        private string manifest;
        public string Manifest
        {
            get => manifest;
            set
            {
                SetProperty(ref manifest, value);
            }
        }
        public DateTime ReleaseTime { get; set; }
    }
}
