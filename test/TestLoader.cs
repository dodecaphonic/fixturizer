using System;
using System.Linq;
using Fixturizer;

namespace Fixturizer.Test
{
        using NUnit.Framework;

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
        }
}
