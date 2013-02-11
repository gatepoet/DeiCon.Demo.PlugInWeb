using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using RazorGenerator.Mvc;

namespace PlugInWeb.MainWeb.App_Start {
    public static class RazorGeneratorMvcStart {
        public static void Start() {
            var engine = new PrecompiledMvcEngine(typeof(WebApiApplication).Assembly);

            ViewEngines.Engines.Insert(0, engine);

            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }
    }
}
