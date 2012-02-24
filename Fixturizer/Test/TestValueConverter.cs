using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Fixturizer;

namespace Fixturizer.Test
{
        using NUnit.Framework;

        [TestFixture]
        public class TestValueConverters
        {
                [Test]
                public void TestFrameworkDefaultsValueConverter()
                {
                        var converter = new FrameworkDefaultsValueConverter();
                        var intType = converter.SupportedTypes.Select(t => t == typeof(int)).First();
                        var intValue = new JValue(1);
                        var doubleValue = new JValue(10.0);
                        var stringValue = new JValue("This is cool...?");
                        var now = DateTime.Now;
                        var dateValue = new JValue(now);
                        var bytes = new byte[2];
                        var byteValue = new JValue(bytes);
                        
                        Assert.IsNotNull(intType);
                        Assert.AreEqual(1, converter.Convert(intValue));
                        Assert.AreEqual(10.0, converter.Convert(doubleValue));
                        Assert.AreEqual("This is cool...?", converter.Convert(stringValue));
                        Assert.AreEqual(now, converter.Convert(dateValue));
                        Assert.AreEqual(bytes, converter.Convert(byteValue));
                }
        }
}
