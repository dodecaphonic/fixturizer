using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GeoAPI.Geometries;
using Fixturizer;

namespace Fixturizer.Tests
{
        using NUnit.Framework;

        [TestFixture]
        public class TestGeospatialValueConverter
        {
                private IValueConverter _converter;
                
                [SetUp]
                public void Init()
                {
                        _converter = new GeospatialValueConverter();
                }
                
                [Test]
                public void TestConvertPoint()
                {
                        var value = new JValue("POINT(1 1)");
                        var converted = (IPoint)_converter.Convert(value);
                        
                        Assert.IsNotNull(converted);
                        Assert.AreEqual(1, converted.X);
                        Assert.AreEqual(1, converted.Y);
                }

                [Test]
                public void TestConvertLineString()
                {
                        var value = new JValue("LINESTRING(1 1, 1 0, 0 1, 0 2)");
                        var converted = (ILineString)_converter.Convert(value);
                        
                        Assert.IsNotNull(converted);
                        Assert.AreEqual(4, converted.NumPoints);
                        Assert.IsFalse(converted.IsRing);
                        Assert.IsFalse(converted.IsClosed);
                }                
        }
}
