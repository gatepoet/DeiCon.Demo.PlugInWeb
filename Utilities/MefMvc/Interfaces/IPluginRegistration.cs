using System.Collections.Generic;
using System.Web.Routing;

namespace org.theGecko.Utilities.MefMvc
{
    public interface IPluginRegistration
    {
        ///<summary>
        /// Assembly name of the plugin
        ///</summary>
        string AssemblyName { get; }

        /// <summary>
        /// Name of the plugin
        /// </summary>
        string PluginName { get; }

        /// <summary>
        /// Priority of the plugin (determines order to search for views and controllers)
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Paths to use to find embedded resources
        /// </summary>
        IList<string> EmbeddedResourcePaths { get; }

        /// <summary>
        /// Method to register routes for the plugin
        /// </summary>
        /// <param name="routes"></param>
        void RegisterRoutes(RouteCollection routes);

        /// <summary>
        /// Method to register menus for the plugin
        /// </summary>
        /// <param name="menu"></param>
        void RegisterMenus(Menu menu);
    }
}
