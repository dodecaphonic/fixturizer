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
                private IDictionary<Type, bool> _basicTypes = new Dictionary<Type, bool> {
                        { typeof(string), true },
                        { typeof(int), true },
                        { typeof(uint), true },
                        { typeof(short), true },
                        { typeof(char), true },
                        { typeof(DateTime), true },
                        { typeof(double), true },
                        { typeof(float), true },
                        { typeof(byte), true },
                        { typeof(byte[]), true }
                };

                public Loader(string basePath)
                {
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

                        foreach (var prop in raw.Properties())
                        {
                                if (HasBasicType(prop, mapped))
                                {
                                        SetProperty(prop, mapped);                
                                }
                                else
                                {
                                        var type = GetMatchingProperty(prop, mapped).PropertyType;
                                        var child = LoadObject((JObject)(prop.Value), type);
                                        SetProperty(prop, mapped, child);
                                }
                        }

                        return mapped;
                }

                protected bool HasBasicType<T>(JProperty prop, T obj)
                {
                        var matchingProperty = GetMatchingProperty(prop, obj);
                        return _basicTypes.ContainsKey(matchingProperty.PropertyType);
                }

                protected void SetProperty<T>(JProperty source, T dest, object value=null)
                {
                        var matchingProperty = GetMatchingProperty(source, dest);

                        if (null != matchingProperty && matchingProperty.CanWrite)
                        {
                                // Clean this up.
                                if (null != value)
                                {
                                        matchingProperty.SetValue(dest, value, null);
                                        return;
                                }
                                
                                switch (source.Value.Type)
                                {
                                case JTokenType.Integer:
                                        matchingProperty.SetValue(dest, (int)source, null);
                                        break;
                                case JTokenType.Comment:
                                case JTokenType.Raw:
                                case JTokenType.String:
                                        matchingProperty.SetValue(dest, (string)source, null);
                                        break;
                                case JTokenType.Float:
                                        matchingProperty.SetValue(dest, (double)source, null);
                                        break;
                                case JTokenType.Date:
                                        matchingProperty.SetValue(dest, (DateTime)source, null);
                                        break;
                                case JTokenType.Bytes:
                                        matchingProperty.SetValue(dest, (byte[])source, null);
                                        break;                                        
                                default:
                                        Console.WriteLine("Fodeu");
                                        break;
                                }
                        }
                        else
                        {
                                throw new Exception(String.Format("Couldn't convert property {0}", source.Name));
                        }
                }

                protected PropertyInfo GetMatchingProperty<T>(JProperty prop, T obj)
                {
                        var type = obj.GetType();
                        return type.GetProperty(prop.Name, BindingFlags.Public | BindingFlags.Instance);
                }
        }
}
