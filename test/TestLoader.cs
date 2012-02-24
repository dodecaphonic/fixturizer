using System;
using System.Linq;
using Fixturizer;
using System.Collections.Generic;

namespace Fixturizer.Test
{
        using NUnit.Framework;

        [TestFixture]
        public class TestLoader
        {
                private Loader _loader;
        
                [SetUp]
                public void Init()
                {
                        _loader = new Loader("/Users/dodecaphonic/Projects/Prodec/fixturizer/fixtures");
                }

                [Test]
                public void TestLoadFlatObject()
                {
                        var elements = _loader.Load<Element>("element");
                        var element = elements.First();
                        Assert.AreEqual(element.Id, 1);
                        Assert.AreEqual(element.Description, "teste");
                        Assert.AreEqual(element.Length, 10.543231234);
                        Assert.AreEqual(element.Creation, new DateTime(2012, 02, 23, 21, 05, 09));
                }

                [Test]
                public void TestLoadObjectGraph()
                {
                        var elements = _loader.Load<ElementLayer>("elementlayer");
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
                        var elements = _loader.Load<ElementWithCollection>("elementcollection");
                        var element = elements.First();
                        Assert.AreEqual(2, element.Layers.Count());

                        var layer = element.Layers[1];
                        Assert.AreEqual("Your fiery foe Trader Joe", layer.Name);
                }
        }

        class Element
        {
                public int Id
                {
                        get; set;
                }

                public string Description
                {
                        get; set;
                }

                public double Length
                {
                        get; set;
                }

                public DateTime Creation
                {
                        get; set;
                }
        }

        class Layer
        {
                public int Id
                {
                        get; set;
                }

                public string Name
                {
                        get; set;
                }                
        }
        
        class ElementLayer : Element
        {
                public Layer Layer
                {
                        get; set;
                }
        }

        class ElementWithCollection : Element
        {
                public IList<Layer> Layers
                {
                        get; set;
                }
        }
}
