using System;
using System.Reflection;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Iesi.Collections;
using System.Collections.Generic;

namespace Fixturizer
{
    public class Loader
    {
        private readonly string _basePath;
        private readonly ValueConverterFactory _factory;

        public Loader(string basePath)
        {
            _factory = new ValueConverterFactory();
            _basePath = basePath;
        }

        public IList<T> Load<T>(Stream jsonStream) where T : new()
        {
            using (var reader = new StreamReader(jsonStream))
            {
                return DesserializeStream<T>(reader);
            }
        }

        public IList<T> Load<T>(string fixtureName) where T : new()
        {
            var filename = fixtureName + ".json";
            var path = Path.Combine(_basePath, filename);
            using (var reader = new StreamReader(path))
            {
                return DesserializeStream<T>(reader);
            }
        }

        protected IList<T> DesserializeStream<T>(StreamReader reader) where T : new()
        {
            List<T> mappedList = new List<T>();
            var buffer = reader.ReadToEnd();
            dynamic fixtures = JsonConvert.DeserializeObject(buffer);
            foreach (var obj in fixtures)
            {
                var typed = LoadObject<T>(obj);
                mappedList.Add(typed);
            }

            return mappedList;
        }

        protected T LoadObject<T>(JObject raw)
        {
            return (T)LoadObject(raw, typeof(T));
        }

        protected object LoadObject(JObject raw, Type t)
        {
            var mapped = Activator.CreateInstance(t);
            object value = null;

            foreach (var prop in raw.Properties())
            {
                var matchingProperty = GetMatchingProperty(prop, mapped);
                var propType = matchingProperty.PropertyType;

                try
                {
                    if (!IsGenericCollection(matchingProperty))
                    {
                        var converter = _factory.GetConverter(propType);
                        value = converter.Convert(prop.Value as JValue);
                    }
                    else
                    {
                        var childType = GetGenericType(matchingProperty);
                        var collection = CreateGenericCollection(matchingProperty, childType);

                        foreach (var obj in prop.Value)
                        {
                            AppendToGenericCollection(collection, LoadObject(obj as JObject, childType));
                        }

                        value = collection;
                    }
                }
                catch (UnsupportedTypeException)
                {
                    value = LoadObject((JObject)(prop.Value), propType);
                }

                matchingProperty.SetValue(mapped, value, null);
            }

            return mapped;
        }

        protected PropertyInfo GetMatchingProperty<T>(JProperty prop, T obj)
        {
            var type = obj.GetType();
            return type.GetProperty(prop.Name, BindingFlags.Public | BindingFlags.Instance);
        }

        protected bool IsGenericCollection(PropertyInfo prop)
        {
            if (!prop.PropertyType.IsGenericType)
            {
                return false;
            }

            var type = prop.PropertyType.GetGenericTypeDefinition();
            return IsAssignableToGenericType(type, typeof(IEnumerable<>));
        }

        /// <summary>
        ///   Determines if a generic type can be assigned to another.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///    Gotten from: http://stackoverflow.com/questions/5461295/using-isassignablefrom-with-generics 
        ///   </para>
        /// </remarks>
        private bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
                if (it.IsGenericType)
                    if (it.GetGenericTypeDefinition() == genericType) return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return baseType.IsGenericType &&
                    baseType.GetGenericTypeDefinition() == genericType ||
                    IsAssignableToGenericType(baseType, genericType);
        }

        protected Type GetGenericType(PropertyInfo prop)
        {
            var type = prop.PropertyType;

            return type.GetGenericArguments()[0];
        }

        protected Type GetCollectionGenericType(Type collectionType)
        {
            return collectionType.GetGenericTypeDefinition();
        }

        // THIS IS REALLY DUMB AND AD-HOC. REALLY. REALLY, REALLY.
        // PLEASE MAKE IT SMARTER, DUMBASS.
        protected object CreateGenericCollection(PropertyInfo prop, Type specializedType)
        {
            Type genericType, concreteType, finalType;
            genericType = GetCollectionGenericType(prop.PropertyType);
            concreteType = genericType;

            if (genericType == typeof(IList<>))
            {
                concreteType = typeof(List<>);
            }
            else if (genericType == typeof(ISet<>))
            {
                concreteType = typeof(SortedSet<>);
            }

            finalType = concreteType.MakeGenericType(new Type[] { specializedType });
            return Activator.CreateInstance(finalType);
        }

        protected void AppendToGenericCollection(object collection, object value)
        {
            var type = collection.GetType();
            type.GetMethod("Add").Invoke(collection, new[] { value });
        }
    }
}
