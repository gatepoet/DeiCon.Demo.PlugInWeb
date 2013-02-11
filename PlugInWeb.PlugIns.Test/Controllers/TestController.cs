using System.Web.Mvc;
using org.theGecko.Utilities.MefMvc;

namespace PlugInWeb.PlugIns.Test.Controllers
{
    public class TestController : MefController
    {
        public ActionResult Index()
        {
            return View(Context);
        }
        public ActionResult Json()
        {
            return Json(new {Text="Heisann"}, JsonRequestBehavior.AllowGet);
        }
    }
    public class ElectionController : MefController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}