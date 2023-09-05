using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using TestEngAuthorization.Views;

namespace TestEngAuthorization
{
    public class TestEngAuthorization : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("TestEngAuthorizationRegion", typeof(Main));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}
