using Empty.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Empty
{
    public class Empty : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("EmptyRegion", typeof(Main));
        }
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}
