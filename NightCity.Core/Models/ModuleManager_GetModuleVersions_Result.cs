using Prism.Mvvm;
using System;

namespace NightCity.Core.Models
{
    public class ModuleManager_GetModuleVersions_Result
    {
        public string Version { get; set; }
        public string ReleaseNotes { get; set; }
        public string Manifest { get; set; }
        public DateTime ReleaseTime { get; set; }
    }
}
