using Prism.DryIoc;
using Prism.Ioc;
using System.Windows;
using ModHub.Views;
using Prism.Modularity;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using NightCity.Core.Services.Prism;
using System;
using System.Reflection;

namespace ModHub
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {
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
        protected override IModuleCatalog CreateModuleCatalog()
        {
            DynamicDirectoryModuleCatalog catalog = new DynamicDirectoryModuleCatalog(null);
            return catalog;
        }
    }
}
