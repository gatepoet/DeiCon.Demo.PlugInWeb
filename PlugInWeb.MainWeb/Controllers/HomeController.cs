using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlugInWeb.MainWeb.App_Start;
using org.theGecko.Utilities.MefMvc;

namespace PlugInWeb.MainWeb.Controllers
{
    public class HomeController : MefController
    {
        public ActionResult Index()
        {
            return View(Context.Plugins);
        }
        public ActionResult Menu()
        {
            return PartialView(Context.MenuContainer);
        }
    }
}
