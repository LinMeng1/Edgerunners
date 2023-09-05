using Lock.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Lock
{
    public class Lock : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("LockRegion", typeof(Main));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
