using System;

namespace Cyggie.Plugins.Utils.Extensions
{
    /// <summary>
    /// Extension class to <see cref="Type"/>
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Checks if <paramref name="type"/> is of generic type and is equal to <paramref name="genericType"/> <br/>
        /// This will check the current type and all its base types
        /// </summary>
        /// <param name="type">Current type to compare</param>
        /// <param name="genericType">Generic type to check for equality</param>
        /// <returns>True if this type or any of its basetype is equal to <paramref name="genericType"/></returns>
        public static bool IsSubclassOfGenericType(this Type? type, Type genericType) => type.IsSubclassOfGenericType(genericType, out _);

        /// <summary>
        /// Checks if <paramref name="type"/> is of generic type and is equal to <paramref name="genericType"/> <br/>
        /// This will check the current type and all its base types
        /// </summary>
        /// <param name="type">Current type to compare</param>
        /// <param name="genericType">Generic type to check for equality</param>
        /// <param name="outputType">Output type that is equal to generic type (null if not found)</param>
        /// <returns>True if this type or any of its basetype is equal to <paramref name="genericType"/></returns>
        public static bool IsSubclassOfGenericType(this Type? type, Type genericType, out Type? outputType)
        {
            outputType = null;
            if (genericType == null || genericType == typeof(object)) return false;

            while (type != null)
            {
                Type? current = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (genericType == current)
                {
                    outputType = current;
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }
    }
}
