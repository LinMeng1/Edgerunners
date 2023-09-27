using NightCity.Core;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace ModHub.Views
{
    /// <summary>
    /// Template.xaml 的交互逻辑
    /// </summary>
    public partial class Template : Window
    {
        public Template(IRegionManager manager, DynamicDirectoryModuleCatalog moduleCatalog, string path, bool shortcut = false)
        {
            bool loadResult = false;
            InitializeComponent();
            string name = string.Empty;
            try
            {
                name = Path.GetFileNameWithoutExtension(path);
                if (moduleCatalog.Modules.FirstOrDefault(it => it.ModuleName == name) == null)
                    moduleCatalog.LoadModuleCatalog(path, true);
                string moduleType = moduleCatalog.Modules.FirstOrDefault(it => it.ModuleName == name).ModuleType;
                string pattern = @"([\s\S.]*?), ([\s\S.]*?), Version=([\s\S.]*?), Culture=([\s\S.]*?), PublicKeyToken=([\s\S.]*?)";
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                Match match = r.Match(moduleType);
                if (match.Success)
                    Title = $"{name} v{match.Groups[3].Value}";
                loadResult = true;
            }
            catch (Exception ex)
            {
                TemplateRegion.Content = $"Open Module Fail:{ex.Message}";
            }
            if (loadResult)
            {
                if (shortcut)
                {
                    manager.Regions.Remove($"{name}RegionShortcut");
                    RegionManager.SetRegionName(TemplateRegion, $"{name}RegionShortcut");
                    RegionManager.SetRegionManager(TemplateRegion, manager);
                }
                else
                {
                    manager.Regions.Remove($"{name}Region");
                    RegionManager.SetRegionName(TemplateRegion, $"{name}Region");
                    RegionManager.SetRegionManager(TemplateRegion, manager);
                }              
            }
        }
    }
}
