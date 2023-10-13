using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validate.Views;

namespace Validate
{
    public class Validate : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ValidateRegion", typeof(Main));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
