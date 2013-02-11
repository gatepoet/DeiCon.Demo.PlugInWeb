using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mef;
using Autofac.Integration.Mvc;
using org.theGecko.Utilities;
using org.theGecko.Utilities.MefMvc;

namespace PlugInWeb.MainWeb.App_Start
{
    public class IoCConfig
    {
        public static void RegisterContainer()
        {
            RegisterAutoFacWithMef();
        }

        private static void RegisterAutoFacWithMef()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(WebApiApplication).Assembly);

            string pluginPath = SettingsUtil.Instance.GetSetting("PluginPath", @"Plugins");
            var pluginProvider = new MefMvcPluginProvider(pluginPath);
            pluginProvider.Initialize(builder);

            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

        }
    }
}