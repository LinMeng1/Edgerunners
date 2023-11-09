using System;

namespace NightCity.Launcher.Utilities
{
    public class PublishInformation
    {
        public string Project { get; set; }
        public string Version { get; set; }
        public string ReleaseNotes { get; set; }
        public string Manifest { get; set; }
        public string ReleaseAddress { get; set; }
        public DateTime ReleaseTime { get; set; }
    }
}
