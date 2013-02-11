using org.theGecko.Utilities.MefMvc;

namespace PlugInWeb.PlugIns.Test
{
    public class PluginRegistration : MefPluginRegistration
    {
        public override int Priority
        {
            get
            {
                return 300;
            }
        }

        public override void RegisterMenus(org.theGecko.Utilities.MefMvc.Menu menu)
        {
            var item = menu.AddLink("TestPlugin", 1, null, null);

            item.AddLink("Test", 1, "Test");
            item.AddLink("Json", 2, "Test", "Json");
            item.AddLink("Election", 3, "Election");
        }
    }

}
