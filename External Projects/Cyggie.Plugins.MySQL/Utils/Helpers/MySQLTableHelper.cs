using Cyggie.Plugins.Logs;
using Cyggie.Plugins.MySQL.Abstract;
using System;
using System.Collections.Generic;

namespace Cyggie.Plugins.MySQL.Utils.Helpers
{
    /// <summary>
    /// Helper class to manage MySQLTable
    /// </summary>
    public static class MySQLTableHelper
    {
        private static Dictionary<Type, string> _map = new Dictionary<Type, string>();

        /// <summary>
        /// Get the table name for type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type to get the table name for</typeparam>
        /// <returns>Table name (type name if not found)</returns>
        public static string GetTableName<T>() where T : MySQLTableObject
        {
            return GetTableName(typeof(T));
        }

        /// <summary>
        /// Get the table name for <paramref name="type"/>
        /// </summary>
        /// <param name="type">Type to get the table name for</param>
        /// <returns>Table name (type name if not found)</returns>
        public static string GetTableName(Type type)
        {
            if (_map.ContainsKey(type))
            {
                return _map[type];
            }

            return type.Name;
        }

        internal static void AddMapping(Type type, string name)
        {
            if (_map.ContainsKey(type))
            {
                Log.Error($"Failed to add mapping, map already contains table name ({_map[type]}) for {type}.", nameof(MySQLTableHelper));
                return;
            }

            _map.Add(type, name);
        }
    }
}
