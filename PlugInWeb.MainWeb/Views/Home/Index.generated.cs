﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PlugInWeb.MainWeb.Views.Home
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.5.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Home/Index.cshtml")]
    public class Index : System.Web.Mvc.WebViewPage<IEnumerable<org.theGecko.Utilities.MefMvc.IPluginRegistration>>
    {
        public Index()
        {
        }
        public override void Execute()
        {
WriteLiteral("<p>PlugIns currently loaded:</p>    \r\n<table>\r\n");

            
            #line 4 "..\..\Views\Home\Index.cshtml"
    
            
            #line default
            #line hidden
            
            #line 4 "..\..\Views\Home\Index.cshtml"
     foreach (var plugin in Model)
    {

            
            #line default
            #line hidden
WriteLiteral("        <tr><td>");

            
            #line 6 "..\..\Views\Home\Index.cshtml"
           Write(plugin.PluginName);

            
            #line default
            #line hidden
WriteLiteral("</td></tr>\r\n");

            
            #line 7 "..\..\Views\Home\Index.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("</table>\r\n\r\n\r\n");

        }
    }
}
#pragma warning restore 1591
