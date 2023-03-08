using System;
using System.Collections.Generic;
using System.Linq;

namespace Cyggie.Main.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper class that includes methods related to Types
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Get all subclass types that inherits from type <typeparamref name="T"/> in the current domain<br/>
        /// </summary>
        /// <typeparam name="T">Super type</typeparam>
        /// <param name="checkAbstract">If true, don't include types that are abstract</param>
        /// <returns>Collection of subclass types</returns>
        public static IEnumerable<Type> GetAllSubclassTypes<T>(bool checkAbstract)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes())
                        .Where(t => t.IsSubclassOf(typeof(T)) && (!checkAbstract || !t.IsAbstract));
        }
    }
}
