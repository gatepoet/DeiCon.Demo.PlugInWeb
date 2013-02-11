using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;

namespace org.theGecko.Utilities.MefMvc
{
    /// <summary>
    /// Context class used to inject MEF composition information into controllers if needed
    /// </summary>
    public class MefContext
    {
        public CompositionContainer Container { get; private set; }
        public Menu MenuContainer { get; private set; }
        public IEnumerable<IPluginRegistration> Plugins { get; private set; }

        public MefContext(CompositionContainer container, Menu menuContainer, IEnumerable<IPluginRegistration> plugins)
        {
            Container = container;
            MenuContainer = menuContainer;
            Plugins = plugins;
        }
    }
}