using Calibration.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibration
{
    public class Calibration : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("CalibrationRegion", typeof(Main));
            regionManager.RegisterViewWithRegion("CalibrationRegionShortcut", typeof(Shortcut));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
