﻿namespace NightCity.Core.Models
{
    public class ModuleManager_GetModules_Result
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Version { get; set; }
        public string Manifest { get; set; }
        public string Icon { get; set; }
        public string Author { get; set; }
        public string AuthorItCode { get; set; }
        public bool IsOfficial { get; set; }
        public string Description { get; set; }
    }
}
