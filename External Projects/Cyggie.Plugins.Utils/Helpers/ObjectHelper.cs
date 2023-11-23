using System;

namespace Cyggie.Plugins.Utils.Helpers
{
    /// <summary>
    /// Helper class for <see cref="UnityEngine.Object"/>
    /// </summary>
    public static class ObjectHelper
    {
        /// <summary>
        /// Assign value if <paramref name="obj"/> is null, returning <paramref name="obj"/> or new value from <paramref name="func"/>. <br/>
        /// The equivalent of the "??=" operator, but <see cref="UnityEngine.Object"/> null checks are different from <see cref="System.Object"/> so this step is necessary to achieve the same result.
        /// </summary>
        /// <typeparam name="T">Must derive from <see cref="UnityEngine.Object"/></typeparam>
        /// <param name="obj">Object to check for null</param>
        /// <param name="func">Func to create an object from</param>
        /// <returns><paramref name="obj"/> or new value from <paramref name="func"/></returns>
        public static T AssignIfNull<T>(ref T obj, Func<T> func) where T : UnityEngine.Object
        {
            if (obj == null)
            {
                obj = func.Invoke();
            }

            return obj;
        }

        /// <summary>
        /// Assign value if <paramref name="obj"/> is null, returning <paramref name="obj"/> or <paramref name="newValue"/>. <br/>
        /// The equivalent of the "??=" operator, but <see cref="UnityEngine.Object"/> null checks are different from <see cref="System.Object"/> so this step is necessary to achieve the same result.
        /// </summary>
        /// <typeparam name="T">Must derive from <see cref="UnityEngine.Object"/></typeparam>
        /// <param name="obj">Object to check for null</param>
        /// <param name="newValue">New value to assign</param>
        /// <returns><paramref name="obj"/> or <paramref name="newValue"/></returns>
        public static T AssignIfNull<T>(ref T obj, T newValue) where T : UnityEngine.Object
        {
            if (obj == null)
            {
                obj = newValue;
            }

            return obj;
        }
    }
}
