using NightCity.Core.Models.Standard;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace NightCity.Converters
{
    internal class ModuleManagerConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string Category = parameter.ToString();
            switch (Category)
            {
                case "IsSelected":
                    {
                        string selected = string.Empty;
                        if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
                            selected = values[0].ToString();
                        string current = string.Empty;
                        if (values[1] != DependencyProperty.UnsetValue && values[1] != null)
                            current = values[1].ToString();
                        if (selected != string.Empty && selected == current)
                            return true;
                        else
                            return false;
                    }
                case "IsInstalled":
                    {
                        string moduleName = string.Empty;
                        if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
                            moduleName = values[0].ToString();
                        else
                            return true;
                        ObservableCollection<ModuleInfo> installedModules = null;
                        if (values[1] != DependencyProperty.UnsetValue && values[1] != null)
                            installedModules = (ObservableCollection<ModuleInfo>)values[1];
                        if (installedModules != null && installedModules.FirstOrDefault(it => it.Name == moduleName) != null)
                            return true;
                        else
                            return false;
                    }
                case "ActionButtonVisibility":
                    {
                        string category = string.Empty;
                        if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
                            category = values[0].ToString();
                        string moduleName = string.Empty;
                        if (values[1] != DependencyProperty.UnsetValue && values[1] != null)
                            moduleName = values[1].ToString();
                        string moduleVersion = string.Empty;
                        if (values[2] != DependencyProperty.UnsetValue && values[2] != null)
                            moduleVersion = values[2].ToString();
                        ObservableCollection<ModuleInfo> installedModules = new ObservableCollection<ModuleInfo>();
                        if (values[3] != DependencyProperty.UnsetValue && values[3] != null)
                            installedModules = (ObservableCollection<ModuleInfo>)values[3];
                        ObservableCollection<ModuleInfo> loadedModules = new ObservableCollection<ModuleInfo>();
                        if (values[4] != DependencyProperty.UnsetValue && values[4] != null)
                            loadedModules = (ObservableCollection<ModuleInfo>)values[4];
                        ModuleInfo installedModule = installedModules.FirstOrDefault(it => it.Name == moduleName);
                        ModuleInfo loadedModule = loadedModules.FirstOrDefault(it => it.Name == moduleName);
                        switch (category)
                        {
                            case "Launch":
                                if (installedModule == null) return Visibility.Collapsed;
                                if (installedModule.Version != moduleVersion) return Visibility.Collapsed;
                                if (loadedModule == null) return Visibility.Visible;
                                if (loadedModule.Version != moduleVersion) return Visibility.Collapsed;
                                return Visibility.Visible;
                            case "Update":
                                if (installedModule == null) return Visibility.Collapsed;
                                if (installedModule.Version == moduleVersion) return Visibility.Collapsed;
                                return Visibility.Visible;
                            case "Install":
                                if (moduleName == string.Empty) return Visibility.Collapsed;
                                if (installedModule != null) return Visibility.Collapsed;
                                return Visibility.Visible;
                            case "Reload":
                                if (installedModule == null) return Visibility.Collapsed;
                                if (installedModule.Version != moduleVersion) return Visibility.Collapsed;
                                if (loadedModule == null) return Visibility.Collapsed;
                                if (loadedModule.Version == moduleVersion) return Visibility.Collapsed;
                                return Visibility.Visible;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
