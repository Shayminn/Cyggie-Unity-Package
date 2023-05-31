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
        /// Get all subclass types that inherits from type <typeparamref name="T"/> in the current domain
        /// </summary>
        /// <typeparam name="T">Super type</typeparam>
        /// <param name="checkAbstract">If true, don't include types that are abstract</param>
        /// <returns>Collection of subclass types</returns>
        public static IEnumerable<Type> GetAllSubclassTypes<T>(bool checkAbstract = true)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes())
                        .Where(t => t.IsSubclassOf(typeof(T)) && (!checkAbstract || !t.IsAbstract));
        }

        /// <summary>
        /// Get all subclass types that inherits from type <typeparamref name="T"/> in the current domain
        /// </summary>
        /// <typeparam name="T">Super type</typeparam>
        /// <param name="checkAbstract">If true, don't include types that are abstract</param>
        /// <param name="pred">Extra conditions that needs to be checked</param>
        /// <returns>Collection of subclass types</returns>
        public static IEnumerable<Type> GetAllSubclassTypes<T>(Predicate<Type> pred, bool checkAbstract = true)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes())
                        .Where(t => t.IsSubclassOf(typeof(T)) && pred.Invoke(t) && (!checkAbstract || !t.IsAbstract));
        }
        
        /// <summary>
        /// Get all subclass types that are assignable from <typeparamref name="T"/> in the current domain
        /// </summary>
        /// <typeparam name="T">Type to check</typeparam>
        /// <param name="checkAbstract">If tr ue, don't include types that are abstract</param>
        /// <returns>Collection of types that are assignable</returns>
        public static IEnumerable<Type> GetAllIsAssignableFrom<T>(bool checkAbstract = true)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes())
                        .Where(t => typeof(T).IsAssignableFrom(t) && (!checkAbstract || !t.IsAbstract));
        }

        /// <summary>
        /// Get all subclass types that are assignable from <typeparamref name="T"/> in the current domain
        /// </summary>
        /// <typeparam name="T">Type to check</typeparam>
        /// <param name="checkAbstract">If tr ue, don't include types that are abstract</param>
        /// <param name="pred">Extra conditions that needs to be checked</param>
        /// <returns>Collection of types that are assignable</returns>
        public static IEnumerable<Type> GetAllIsAssignableFrom<T>(Predicate<Type> pred, bool checkAbstract = true)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes())
                        .Where(t => typeof(T).IsAssignableFrom(t) && pred.Invoke(t) && (!checkAbstract || !t.IsAbstract));
        }
    }
}
