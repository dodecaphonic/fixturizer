using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fixturizer {
        public class Loader
        {
                private readonly string _basePath;

                public Loader(string basePath)
                {
                        _basePath = basePath;
                }

                public IList<T> Load<T>(string fixtureName) where T : new()
                {
                        var filename = fixtureName + ".json";
                        var path = Path.Combine(_basePath, filename);

                        dynamic fixture;
                        var mappedList = new List<T>();
                        
                        using (var reader = new StreamReader(path))
                        {
                                var buffer = reader.ReadToEnd();
                                fixture = JsonConvert.DeserializeObject(buffer);

                                foreach (var obj in fixture)
                                {
                                        var mapped = new T();

                                        foreach (var prop in obj)
                                        {
                                                SetProperty(prop, mapped);
                                        }
                                        
                                        mappedList.Add(mapped);
                                }
                        }

                        return mappedList;
                }

                protected void SetProperty<T>(JProperty source, T dest)
                {
                        var type = typeof(T);
                        var matchingProperty = type.GetProperty(source.Name, BindingFlags.Public | BindingFlags.Instance);

                        if (null != matchingProperty && matchingProperty.CanWrite)
                        {
                                switch (source.Value.Type)
                                {
                                case JTokenType.Integer:
                                        matchingProperty.SetValue(dest, (int)source.Value, null);
                                        break;
                                case JTokenType.String:
                                        matchingProperty.SetValue(dest, (string)source, null);
                                        break;
                                case JTokenType.Float:
                                        matchingProperty.SetValue(dest, (double)source, null);
                                        break;
                                default:
                                        Console.WriteLine("Fodeu");
                                        break;
                                }
                                
                        }
                        else
                        {
                                throw new Exception("WTF?");
                        }
                }
        }
}
