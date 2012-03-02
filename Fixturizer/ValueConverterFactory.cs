using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Fixturizer
{
        /// <summary>
        ///   Creates IValueConverters and allows users to get the appropriate
        ///   one for a given type.
        /// </summary>
        public class ValueConverterFactory
        {
                private readonly IDictionary<Type, IValueConverter> _converters;
                
                public ValueConverterFactory()
                {
                        _converters = LoadConverters();
                }

                /// <summary>
                ///   Finds a converter for the given type.
                /// </summary>
                /// <param name="t">The Type for which a Converter is needed</param>
                /// <exception cref="Fixturizer.UnsupportedTypeException">
                ///   Thrown if there's no suitable converter for the type.
                /// </exception>
                public IValueConverter GetConverter(Type t)
                {
                        try
                        {
                                return _converters[t];
                        }
                        catch (Exception)
                        {
                                throw new UnsupportedTypeException();
                        }
                        
                }

                protected IDictionary<Type, IValueConverter> LoadConverters()
                {
                        var ivcType = typeof(IValueConverter);
                        var assembly = ivcType.Assembly;
                        var available = from t in assembly.GetTypes()
                                where t != ivcType && ivcType.IsAssignableFrom(t)
                                select t;

                        var typeMap = new Dictionary<Type, IValueConverter>();

                        foreach (var converterType in available)
                        {
                                var converter = (IValueConverter)Activator.CreateInstance(converterType);

                                foreach (var supportedType in converter.SupportedTypes)
                                {
                                        typeMap[supportedType] = converter;
                                }
                        }

                        return typeMap;
                }
        }
}

