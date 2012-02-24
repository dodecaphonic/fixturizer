using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fixturizer
{
        public class FrameworkDefaultsValueConverter : IValueConverter
        {
                private readonly List<Type> _supportedTypes = new List<Type> {
                        typeof(string), typeof(int), typeof(uint), typeof(uint),
                        typeof(short), typeof(char), typeof(DateTime),
                        typeof(double), typeof(float), typeof(byte[])
                };

                public IList<Type> SupportedTypes
                {
                        get { return _supportedTypes.AsReadOnly(); }
                }

                public object Convert(JValue value)
                {
                        switch (value.Type)
                        {
                        case JTokenType.Integer:
                                return (int)value;
                        case JTokenType.Comment:
                        case JTokenType.Raw:
                        case JTokenType.String:
                                return (string)value;
                        case JTokenType.Float:
                                return (double)value;
                        case JTokenType.Date:
                                return (DateTime)value;
                        case JTokenType.Bytes:
                                return (byte[])value;
                        default:
                                throw new UnsupportedTypeException();
                        }
                }
        }
}
