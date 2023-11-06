using Newtonsoft.Json;
using NightCity.Core;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NightCity.Launcher.Utilities
{
    public static class ConfigHelper
    {
        public static string path = @".\Config.json";
        public static Config GetConfig(Config last)
        {
            Config config = last;
            try
            {
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                    string jsonStr = JsonConvert.SerializeObject(last);
                    File.WriteAllText(path, jsonStr);
                }
                else
                {
                    string jsonStr = File.ReadAllText(path);
                    config = JsonConvert.DeserializeObject<Config>(jsonStr);
                }
            }
            catch { }
            return config;
        }
        public static void SetConfig(Config current)
        {
            try
            {
                if (!File.Exists(path))
                    File.Create(path).Close();
                string jsonStr = JsonConvert.SerializeObject(current);
                File.WriteAllText(path, jsonStr);
            }
            catch { }
        }
    }
    public class Config : BindableBase
    {
        private string dataSource;
        public string DataSource
        {
            get => dataSource;
            set
            {
                SetProperty(ref dataSource, value);
                ConfigChanged?.Invoke();
            }
        }

        public delegate void ConfigChangedDelegate();
        public event ConfigChangedDelegate ConfigChanged;
    }
}
