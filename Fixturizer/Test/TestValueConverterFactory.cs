using System;
using Fixturizer;

namespace Fixturizer.Tests
{
        using NUnit.Framework;

        [TestFixture]
        public class TestValueConverterFactory
        {
                private ValueConverterFactory _factory;
                
                [SetUp]
                public void Init()
                {
                        _factory = new ValueConverterFactory();
                }
                
                [Test]
                public void TestFactoryCanCreateConverter()
                {
                        var converter = _factory.GetConverter(typeof(int));

                        Assert.IsNotNull(converter);
                }

                [Test]
                public void TestFactoryThrowsExceptionWhenTypeIsUnknown()
                {
                        Assert.Throws(typeof(Fixturizer.UnsupportedTypeException), () => _factory.GetConverter(typeof(ValueConverterFactory)));
                }
        }
}
