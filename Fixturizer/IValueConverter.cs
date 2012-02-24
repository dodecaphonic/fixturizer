using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fixturizer
{
        public class UnsupportedTypeException : Exception
        {
        }

        /// <summary>
        ///   Defines the behavior for converters of JProperty instances into
        ///   values/objects relevant to the user.
        /// </summary>
        public interface IValueConverter
        {
                /// <summary>
                ///   Lists all the types this converter supports.
                /// </summary>
                /// <remarks>
                ///   <para>
                ///     Ideally, implementers should return an immutable
                ///     collection.
                ///   </para>
                /// </remarks>
                IList<Type> SupportedTypes
                {
                        get;
                }

                /// <summary>
                ///   Converts a JValue into a known type.
                /// </summary>
                /// <remarks>
                ///   <para>
                ///     This takes into account the list of known types in
                ///     SupportedTypes.
                ///   </para>
                /// </remarks>
                /// <example>
                ///   
                /// </example>
                /// <returns>An instance/a value of type T.</returns>
                /// <exception cref="Fixturizer.UnsupportedTypeException">
                ///   Thrown when T is not one of the ConvertibleTypes.
                /// </exception>
                object Convert(JValue value);
        }
}
