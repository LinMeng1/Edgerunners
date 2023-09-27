using NightCity.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using NightCity.Core.Services.Prism;
using NightCity.Core;

namespace NightCity
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {
        private static Mutex _mutex = null;
        protected override Window CreateShell()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
               {
                   return true;
               };
            return Container.Resolve<MainWindow>();
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IPropertyService, PropertyService>();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            #region 防止复数实例
            const string appName = "NightCity";
            _mutex = new Mutex(true, appName, out bool createdNew);
            if (!createdNew)
                Current.Shutdown();
            #endregion
            base.OnStartup(e);
        }
        protected override IModuleCatalog CreateModuleCatalog()
        {
            DynamicDirectoryModuleCatalog catalog = new DynamicDirectoryModuleCatalog(null);
            return catalog;
        }
    }
}
