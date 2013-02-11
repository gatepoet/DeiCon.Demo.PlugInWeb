using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;

using System.Web.Routing;
using System.Web.WebPages;
using Autofac;
using Autofac.Integration.Mef;


namespace org.theGecko.Utilities.MefMvc
{
    /// <summary>
    /// Main plugin registration code
    /// </summary>
    public class MefMvcPluginProvider
    {
        private readonly string _pluginPath;
        private readonly string _fullPluginPath;
        private readonly string _defaultMaster;

        public MefMvcPluginProvider(string pluginPath)
            : this(pluginPath, "_Layout")
        {
        }

        public MefMvcPluginProvider(string pluginPath, string defaultMaster)
        {
            _pluginPath = pluginPath;
            _fullPluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pluginPath);
            _defaultMaster = defaultMaster;
        }

        #region IMvcPluginProvider Members

        public void Initialize(ContainerBuilder builder)
        {
            // Add assembly handler for strongly-typed view models
            AppDomain.CurrentDomain.AssemblyResolve += PluginAssemblyResolve;

            // Load types in from assemblies
            var rb = new RegistrationBuilder();
            rb.ForTypesDerivedFrom<IController>()
              .Export<IController>(AddControllerMetadata)
              .SetCreationPolicy(CreationPolicy.NonShared);

            rb.ForTypesDerivedFrom<ApiController>()
              .Export<ApiController>(AddControllerMetadata)
              .SetCreationPolicy(CreationPolicy.NonShared);

            rb.ForTypesDerivedFrom<IPluginRegistration>()
                .Export<IPluginRegistration>()
                .SetCreationPolicy(CreationPolicy.Shared);

            DirectoryCatalog catalog = new DirectoryCatalog(_fullPluginPath, "*.dll", rb);
            CompositionContainer container = new CompositionContainer(catalog);

            // Retrieve plugins in priority order
            ISet<IPluginRegistration> plugins = new SortedSet<IPluginRegistration>(new PluginComparer());
            foreach (IPluginRegistration import in container.GetExportedValues<IPluginRegistration>())
            {
                plugins.Add(import);
            }

            // Register plugins
            Menu menu = new Menu();
            var engine = new MyPrecompiledMvcEngine();
            engine.AddViews(Assembly.GetCallingAssembly());
            foreach (IPluginRegistration plugin in plugins)
            {
                plugin.RegisterRoutes(RouteTable.Routes);
                plugin.RegisterMenus(menu);
                engine.AddViews(plugin.GetType().Assembly);
            }
            // Set the controller factory
            MefContext context = new MefContext(container, menu, plugins);
            var factory = new MefControllerFactory(context);
            ControllerBuilder.Current.SetControllerFactory(factory);

            // Set view engine
            ViewEngines.Engines.Add(engine);
            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);

            builder.RegisterComposablePartCatalog(catalog);
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), factory);
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector), factory);
        }

        private void AddControllerMetadata(ExportBuilder b)
        {
            b.AddMetadata("ControllerName", t => t.Name.Substring(0, t.Name.Length - "Controller".Length));
            b.AddMetadata("ControllerType", t => t);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Based on ideas from http://blog.codinglight.com/2009/07/assembly-resolution-with.html
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="resolveArgs"></param>
        /// <returns></returns>
        private Assembly PluginAssemblyResolve(object sender, ResolveEventArgs resolveArgs)
        {
            Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Check we don't already have the assembly loaded
            foreach (Assembly assembly in currentAssemblies)
            {
                if (assembly.FullName == resolveArgs.Name || assembly.GetName().Name == resolveArgs.Name)
                {
                    return assembly;
                }
            }

            // Load from directory
            return LoadAssemblyFromPath(resolveArgs.Name, _fullPluginPath);
        }

        private static Assembly LoadAssemblyFromPath(string assemblyName, string directoryPath)
        {
            foreach (string file in Directory.GetFiles(directoryPath))
            {
                Assembly assembly;

                if (TryLoadAssemblyFromFile(file, assemblyName, out assembly))
                {
                    return assembly;
                }
            }

            return null;
        }

        private static bool TryLoadAssemblyFromFile(string file, string assemblyName, out Assembly assembly)
        {
            try
            {
                // Convert the filename into an absolute file name for 
                // use with LoadFile. 
                file = new FileInfo(file).FullName;

                if (AssemblyName.GetAssemblyName(file).Name == assemblyName)
                {
                    assembly = Assembly.LoadFile(file);
                    return true;
                }
            }
            catch
            {
            }

            assembly = null;
            return false;
        }

        #endregion

        private class PluginComparer : IComparer<IPluginRegistration>
        {
            public int Compare(IPluginRegistration x, IPluginRegistration y)
            {
                if (x.PluginName == y.PluginName)
                {
                    throw new Exception(string.Format("Multiple Plugins have both been registered with plugin name '{0}'.", x.PluginName));
                } 
                
                int result = x.Priority.CompareTo(y.Priority);

                if (result == 0)
                {
                    throw new Exception(string.Format("Plugins '{0}' and '{1}' have both been registered with priority '{2}'.", x.PluginName, y.PluginName, x.Priority));
                }

                return result;
            }
        }
    }
}
