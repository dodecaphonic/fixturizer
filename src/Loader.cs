using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fixturizer {
        public class Loader
        {
                private readonly string _basePath;
                private readonly ValueConverterFactory _factory;

                public Loader(string basePath)
                {
                        _factory = new ValueConverterFactory();
                        _basePath = basePath;
                }

                public IList<T> Load<T>(string fixtureName) where T : new()
                {
                        var filename = fixtureName + ".json";
                        var path = Path.Combine(_basePath, filename);

                        IList<T> mappedList = new List<T>();
                        using (var reader = new StreamReader(path))
                        {
                                var buffer = reader.ReadToEnd();
                                dynamic fixtures = JsonConvert.DeserializeObject(buffer);
                                foreach (var obj in fixtures)
                                {
                                        mappedList.Add(LoadObject<T>(obj));
                                }
                        }

                        return mappedList;
                }

                protected T LoadObject<T>(JObject raw) where T : new()
                {
                        return (T)LoadObject(raw, typeof(T));
                }

                protected object LoadObject(JObject raw, Type t)
                {
                        var mapped = Activator.CreateInstance(t);
                        object value;
                        
                        foreach (var prop in raw.Properties())
                        {
                                var matchingProperty = GetMatchingProperty(prop, mapped);
                                var propType = matchingProperty.PropertyType;

                                try
                                {
                                        var converter = _factory.GetConverter(propType);
                                        value = converter.Convert(prop.Value as JValue);
                                }
                                catch (UnsupportedTypeException e)
                                {
                                        value = LoadObject((JObject)(prop.Value), propType);
                                }

                                matchingProperty.SetValue(mapped, value , null);
                        }
                        
                        return mapped;
                }

                protected PropertyInfo GetMatchingProperty<T>(JProperty prop, T obj)
                {
                        var type = obj.GetType();
                        return type.GetProperty(prop.Name, BindingFlags.Public | BindingFlags.Instance);
                }
        }
}
