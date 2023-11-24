using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProgramLauncher.Views;

namespace TestProgramLauncher
{
    public class TestProgramLauncher : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("TestProgramLauncherRegion", typeof(Main));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
