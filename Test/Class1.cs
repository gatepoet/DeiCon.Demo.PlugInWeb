using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using NUnit.Framework;
using org.theGecko.Utilities.MefMvc;

namespace Test
{
    [TestFixture]
    public class Class1
    {
        private CompositionContainer _container;

        [SetUp]
        public void Setup()
        {
            var rb = new RegistrationBuilder();
            rb.ForTypesDerivedFrom<IController>()
              .Export<IController>(b => b.AddMetadata("ControllerName", t => t.Name.Substring(0, t.Name.Length - "Controller".Length)))
              .SetCreationPolicy(CreationPolicy.NonShared);
            rb.ForTypesDerivedFrom<IPluginRegistration>().Export<IPluginRegistration>();
            DirectoryCatalog catalog = new DirectoryCatalog(@"..\..\..\PlugInWeb.MainWeb\Plugins", "*.dll", rb);
            _container = new CompositionContainer(catalog);
        }

        [Test]
        public void Must_have_controller()
        {
            var exports = _container.GetExports<IController>();

            Assert.That(exports, Is.Not.Empty);
        }

        [Test]
        public void Must_have_metadata()
        {
            var metadata = _container.GetExports<IController, IDictionary<String, object>>().First().Metadata;
            foreach (var o in metadata)
            {
                Console.WriteLine(o.Key);
            }
            Assert.That(metadata.ContainsKey("ControllerName"));
        }

        [Test]
        public void Must_have_strong_typed_metadata_export()
        {
            var export = _container.GetExports<IController, IControllerMetaData>();

            Assert.That(export, Is.Not.Empty);
        }

        [Test]
        public void Must_have_strong_typed_metadata()
        {
            var export = _container.GetExports<IController, IControllerMetaData>();

            Assert.That(export.First().Metadata.ControllerName, Is.EqualTo("Test"));
        }

        [Test]
        public void Must_have_plugins()
        {
            var exports = _container.GetExports<IPluginRegistration>();

            Assert.That(exports, Is.Not.Empty);
        }
    }
}
