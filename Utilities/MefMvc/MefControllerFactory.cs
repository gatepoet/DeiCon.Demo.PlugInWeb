using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace org.theGecko.Utilities.MefMvc
{

    /// <summary>
    /// Based on ideas from http://blog.maartenballiauw.be/post/2009/04/21/ASPNET-MVC-and-the-Managed-Extensibility-Framework-%28MEF%29.aspx
    /// </summary>
    public class MefControllerFactory : IHttpControllerSelector, IControllerFactory, IHttpControllerActivator
    {

        private readonly MefContext _context;
        private readonly DefaultControllerFactory _defaultFactory;
        private readonly DefaultHttpControllerSelector _defaultHttpControllerSelector;

        public MefControllerFactory(MefContext context)
        {
            _context = context;
            _defaultFactory = new DefaultControllerFactory();
            _defaultHttpControllerSelector = new DefaultHttpControllerSelector(GlobalConfiguration.Configuration);
        }

        #region IControllerFactory Members

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            IController controller = SelectController(requestContext, controllerName);

            // Inject extra context
            if (controller is MefController)
            {
                (controller as MefController).Context = _context;
            }

            return controller;
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.Default;
        }

        private IController SelectController(RequestContext requestContext, string controllerName)
        {
            if (controllerName != null)
            {
                // Select all controllers with the right name across all loaded plugins
                IEnumerable<Lazy<IController>> exports = _context.Container.GetExports<IController, IControllerMetaData>()
                    .Where(c =>
                           c.Metadata.ControllerName.ToLowerInvariant() == controllerName.ToLowerInvariant()
                    );

                // Iterate around our plugins in order to select the highest priority controller
                if (exports.Count() > 0)
                {
                    foreach (IPluginRegistration plugin in _context.Plugins)
                    {
                        foreach (Lazy<IController> export in exports)
                        {
                            if (Assembly.GetAssembly(export.GetType()).GetName().Name == plugin.AssemblyName)
                            {
                                return export.Value;
                            }
                        }
                    }

                    // Shouldn't really be reached
                    return exports.First().Value;
                }
            }

            // Controller not found, it must be in the main project
            return _defaultFactory.CreateController(requestContext, controllerName);
        }

        public void ReleaseController(IController controller)
        {
            IDisposable disposable = controller as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor,
                                      Type controllerType)
        {
            var export = _context.Container.GetExports<ApiController, IControllerMetaData>()
                                 .Single(c => c.Metadata.ControllerType == controllerType);
            return export.Value;
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var controllerName = _defaultHttpControllerSelector.GetControllerName(request);
            if (controllerName != null)
            {
                // Select all controllers with the right name across all loaded plugins
                var exports = _context.Container.GetExports<ApiController, IControllerMetaData>()
                    .Where(c =>
                           c.Metadata.ControllerName.ToLowerInvariant() == controllerName.ToLowerInvariant()
                    );

                // Iterate around our plugins in order to select the highest priority controller
                if (exports.Any())
                {
                    foreach (IPluginRegistration plugin in _context.Plugins)
                    {
                        foreach (var export in exports)
                        {
                            if (Assembly.GetAssembly(export.Metadata.ControllerType).GetName().Name == plugin.AssemblyName)
                            {
                                return new HttpControllerDescriptor(GlobalConfiguration.Configuration, controllerName, export.Metadata.ControllerType);
                            }
                        }
                    }

                    // Shouldn't really be reached
                    return new HttpControllerDescriptor(GlobalConfiguration.Configuration, controllerName, exports.First().Metadata.ControllerType);
                }
            }

            return null;
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return null;
        }
    }
}