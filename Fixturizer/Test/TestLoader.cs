using System;
using System.IO;
using System.Linq;
using Fixturizer;
using System.Collections.Generic;

namespace Fixturizer.Test
{
    using NUnit.Framework;
    using System.Reflection;

    [TestFixture]
    public class TestLoader
    {
        private Loader _loader;

        [SetUp]
        public void Init()
        {
            var fullPath = System.Reflection.Assembly.GetAssembly(typeof(Loader)).Location;
            var path = Path.GetDirectoryName(fullPath);
            _loader = new Loader(Path.Combine(path, "Fixtures"));
        }

        [Test]
        public void TestLoadFlatObject()
        {
            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream("Test.Fixtures.element.json");
            var elements = _loader.Load<Element>(stream);
            var element = elements.First();
            Assert.AreEqual(element.Id, 1);
            Assert.AreEqual(element.Description, "teste");
            Assert.AreEqual(element.Length, 10.543231234);
            Assert.AreEqual(element.Creation, new DateTime(2012, 02, 23, 21, 05, 09));
        }

        [Test]
        public void TestLoadObjectGraph()
        {
            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream("Test.Fixtures.elementlayer.json");
            var elements = _loader.Load<ElementLayer>(stream);
            var element = elements.First();
            Assert.AreEqual(element.Id, 1);
            Assert.AreEqual(element.Description, "teste");
            Assert.AreEqual(element.Length, 10.543231234);
            Assert.IsNotNull(element.Layer);
            Assert.AreEqual(element.Layer.Id, 1);
        }

        [Test]
        public void TestLoadCollectionObjects()
        {
            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream("Test.Fixtures.elementcollection.json");
            var elements = _loader.Load<ElementWithCollection>(stream);
            var element = elements.First();
            Assert.AreEqual(2, element.Layers.Count());

            var layer = element.Layers[1];
            Assert.AreEqual("Your fiery foe Trader Joe", layer.Name);
        }
    }

    public class Element
    {
        public int Id
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public double Length
        {
            get;
            set;
        }

        public DateTime Creation
        {
            get;
            set;
        }
    }

    public class Layer
    {
        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }

    public class ElementLayer : Element
    {
        public Layer Layer
        {
            get;
            set;
        }
    }

    public class ElementWithCollection : Element
    {
        public IList<Layer> Layers
        {
            get;
            set;
        }
    }
}
