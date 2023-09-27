using Microsoft.Win32;
using ModHub.Views;
using NightCity.Core;
using NightCity.Core.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.IO;
using System.Windows.Input;

namespace ModHub.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        IRegionManager manager;
        DynamicDirectoryModuleCatalog moduleCatalog;
        public MainWindowViewModel(IRegionManager manager, IModuleCatalog moduleCatalog, IEventAggregator e)
        {
            this.manager = manager;
            this.moduleCatalog = (DynamicDirectoryModuleCatalog)moduleCatalog;
        }
        public ICommand ImportModuleCommand
        {
            get => new DelegateCommand(ImportModule);
        }
        public void ImportModule()
        {
            var dialog = new OpenFileDialog
            {
                Filter = ".dll|*.dll"
            };
            if (dialog.ShowDialog() == false) return;
            string path = dialog.FileName;
            Template template = new Template(manager, moduleCatalog, path);
            template.Show();
        }

        public ICommand ImportShortcutModuleCommand
        {
            get => new DelegateCommand(ImportShortcutModule);
        }
        public void ImportShortcutModule()
        {
            var dialog = new OpenFileDialog
            {
                Filter = ".dll|*.dll"
            };
            if (dialog.ShowDialog() == false) return;
            string path = dialog.FileName;
            Template template = new Template(manager, moduleCatalog, path, true);
            template.Show();
        }
    }
}
