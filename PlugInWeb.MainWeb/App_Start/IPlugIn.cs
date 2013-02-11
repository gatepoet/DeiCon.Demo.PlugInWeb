using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace PlugInWeb.MainWeb.App_Start
{
    public interface IPlugIn
    {
        string Name { get; }
        MenuItem[] MenuItems { get; }
    }
}