﻿using OnCall.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace OnCall
{
    public class OnCall : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("OnCallRegion", typeof(Main));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}
