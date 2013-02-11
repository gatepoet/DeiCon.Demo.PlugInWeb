using org.theGecko.Utilities.MefMvc;

namespace PlugInWeb.PlugIns.DataApi
{
    public class PluginRegistration : MefPluginRegistration
    {
        public override int Priority
        {
            get
            {
                return 200;
            }
        }
    }

}
