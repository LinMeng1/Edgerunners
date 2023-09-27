using NightCity.Core;
using NightCity.Core.Interfaces;
using NightCity.Core.Models.Standard;
using Prism.Regions;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace NightCity.Views
{
    /// <summary>
    /// Template.xaml 的交互逻辑
    /// </summary>
    public partial class Template
    {
        object currentModel;
        ModuleInfo module;
        public Template(IRegionManager manager, DynamicDirectoryModuleCatalog moduleCatalog, ModuleInfo module, ObservableCollection<ModuleInfo> loadedMod, out bool loadResult)
        {

            this.module = module;
            loadResult = false;
            if (module.Category == "Authorization")
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Head.Text = module.Name;
            Official.Visibility = module.IsOfficial ? Visibility.Visible : Visibility.Collapsed;
            Author.Text = module.Author;
            Title = module.Name;
            try
            {
                ApplicationIcon.Kind = module.Category == "Authorization" ? MaterialDesignThemes.Wpf.PackIconKind.Fingerprint : module.Icon;
                if (moduleCatalog.Modules.FirstOrDefault(it => it.ModuleName == module.Name) == null)
                    moduleCatalog.LoadModuleCatalog(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Modules\{module.Name}\{module.Version}\{module.Name}.dll"), true);
                string moduleType = moduleCatalog.Modules.FirstOrDefault(it => it.ModuleName == module.Name).ModuleType;
                string pattern = @"([\s\S.]*?), ([\s\S.]*?), Version=([\s\S.]*?), Culture=([\s\S.]*?), PublicKeyToken=([\s\S.]*?)";
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                Match match = r.Match(moduleType);
                string version = string.Empty;
                if (match.Success)
                    version = match.Groups[3].Value;
                if (loadedMod.FirstOrDefault(it => it.Name == module.Name) == null)
                    loadedMod.Add(module);
                Foot.Text = $"Version  {version}";
                if (version != module.Version)
                    throw new Exception("Module version mismatch");
                loadResult = true;
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"Open Module Fail:{ex.Message}";
            }
            if (loadResult)
            {
                ErrorRegion.Visibility = Visibility.Collapsed;
                TemplateRegion.Visibility = Visibility.Visible;
                manager.Regions.Remove($"{module.Name}Region");
                RegionManager.SetRegionName(TemplateRegion, $"{module.Name}Region");
                RegionManager.SetRegionManager(TemplateRegion, manager);
                currentModel = manager.Regions[$"{module.Name}Region"].ActiveViews.FirstOrDefault();
            }
        }
        private void Minimize_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                WindowState = WindowState.Minimized;
            }
        }
        private void Maximize_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                if (WindowState == WindowState.Maximized)
                {
                    BorderThickness = new Thickness(0);
                    WindowState = WindowState.Normal;
                }
                else
                {
                    BorderThickness = new Thickness(8);
                    WindowState = WindowState.Maximized;
                }

            }
        }
        private void Close_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                if (module.Category == "Monitor" || module.Category == "Authorization")
                    Hide();
                else
                {
                    Dispose();
                    Close();
                }
            }
        }
        private void Window_Move(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }
        public void Dispose()
        {
            try
            {
                UserControl model = currentModel as UserControl;
                IDisposable viewModel = model.DataContext as IDisposable;
                viewModel?.Dispose();
                GC.Collect();
            }
            catch { }
        }
        public void Disauthorize()
        {
            try
            {
                UserControl model = currentModel as UserControl;
                IAuthorizable viewModel = model.DataContext as IAuthorizable;
                viewModel?.Disauthorize();
            }
            catch { }
        }
    }
}
