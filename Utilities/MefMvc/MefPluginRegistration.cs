using System.Collections.Generic;
using System.Reflection;
using System.Web.Routing;

namespace org.theGecko.Utilities.MefMvc
{
    ///<summary>
    ///</summary>
    public abstract class MefPluginRegistration : IPluginRegistration
    {
        protected Assembly PluginAssembly;

        protected MefPluginRegistration()
        {
            // Save a reference to the plugin assembly
            PluginAssembly = Assembly.GetAssembly(GetType());
        }

        #region IPluginRegistration Members

        /// <summary>
        /// Specifies the plugin's assembly name.
        /// </summary>
        public string AssemblyName
        {
            get
            {
                return PluginAssembly.GetName().Name;
            }
        }

        /// <summary>
        /// Specifies the plugin's name.
        /// </summary>
        public virtual string PluginName
        {
            get
            {
                return AssemblyName;
            }
        }

        /// <summary>
        /// Specifies the plugin's priority relative to other plugins.
        /// </summary>
        public abstract int Priority
        {
            get;
        }

        public virtual IList<string> EmbeddedResourcePaths
        {
            get
            {
                return new List<string> {"Content", "Images", "Scripts"};
            }
        }

        public virtual void RegisterRoutes(RouteCollection routes)
        {
            // Routes for embedded resources
            // Based on ideas from http://github.com/loudej/spark/blob/cb60a413bf72dfd0bc7cf7bd6b6a111a8ea2ab63/src/Samples/Modules/Spark.Modules/WebPackageBase.cs
            foreach (string path in EmbeddedResourcePaths)
            {
                routes.Add(
                    new Route(path + "/{assembly}/{*resource}",
                    new RouteValueDictionary(),
                    new RouteValueDictionary(new { assembly = AssemblyName }),
                    new EmbeddedContentRouteHandler(PluginAssembly, AssemblyName + "." + path))
                );

                if (PluginName != AssemblyName)
                {
                    routes.Add(
                        new Route(path + "/{plugin}/{*resource}",
                        new RouteValueDictionary(),
                        new RouteValueDictionary(new {plugin = PluginName}),
                        new EmbeddedContentRouteHandler(PluginAssembly, AssemblyName + "." + path))
                    );
                }
            }

            // Hijack "Areas" functionality
            //routes.MapRoute(
            //    AssemblyName + "_default", // Route name
            //    AssemblyName + "/{controller}/{action}/{id}", // URL with parameters
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            //);
        }

        /// <summary>
        /// Adds this plugin's menu items to the site menu.
        /// </summary>
        /// <param name="menu">The site menu.</param>
        public virtual void RegisterMenus(Menu menu)
        {
        }

        #endregion

        /// <summary>
        /// Returns a System.String that represents the current IPluginRegistration.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", PluginName, Priority);
        }
    }
}