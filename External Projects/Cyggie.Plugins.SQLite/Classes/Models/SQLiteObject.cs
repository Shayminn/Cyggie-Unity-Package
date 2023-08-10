using Cyggie.Plugins.Encryption;
using Cyggie.Plugins.Logs;
using Cyggie.Plugins.SQLite.Utils.Helpers;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cyggie.Plugins.SQLite
{
    /// <summary>
    /// Model for an SQLite database object <br/>
    /// Each instance of this object represents a row of a database table
    /// </summary>
    public abstract class SQLiteObject
    {
        /// <summary>
        /// Returns array of SQLParams for SQLManager methods
        /// Parameter keys in Query must match the property name (case-insensitive)
        /// Any parameters that are not found within the Query string will be ignored
        /// </summary>
        public SQLiteParams[] SQLParams
        {
            get
            {
                PropertyInfo[] fields = this.GetType().GetProperties().Where(x => Attribute.IsDefined(x, typeof(SQLitePropertyAttribute))).ToArray();
                List<SQLiteParams> paramss = new List<SQLiteParams>();

                // Iterate through all Properties that has attribute SQLProperty
                foreach (PropertyInfo property in fields)
                {
                    SQLitePropertyAttribute sqliteProperty = property.GetCustomAttribute<SQLitePropertyAttribute>();
                    if (sqliteProperty == null) continue;

                    string columnName = sqliteProperty.HasDefinedName ? sqliteProperty.ColumnName : property.Name;
                    paramss.Add(new SQLiteParams(columnName, property.GetValue(this)));
                }

                return paramss.ToArray();
            }
        }

        /// <summary>
        /// Get all SQLiteProperty Attribute fields
        /// </summary>
        /// <typeparam name="T">This object's type</typeparam>
        /// <returns>Enumerable of properties with SQLitePropertyAttribute</returns>
        internal static IEnumerable<PropertyInfo> GetFields<T>() where T : SQLiteObject
        {
            return typeof(T).GetProperties().Where(x => Attribute.IsDefined(x, typeof(SQLitePropertyAttribute)));
        }

        /// <summary>
        /// Get all parameter arguments to its constructor based on <see cref="GetFields{T}"/>
        /// </summary>
        /// <typeparam name="T">This object's type</typeparam>
        /// <param name="fields">Enumerable of fields that has the SQLiteProperty attribute</param>
        /// <param name="reader">Reader to get the current table's row</param>
        /// <param name="encrypted">Whether the database is encrypted</param>
        /// <returns>Enumerable of arguments</returns>
        internal static IEnumerable<object> GetConstructorArguments<T>(IEnumerable<PropertyInfo> fields, SqliteDataReader reader, bool encrypted) where T : SQLiteObject
        {
            List<object> args = new List<object>();
            foreach (PropertyInfo property in fields)
            {
                string propertyName = encrypted ?
                                      AESEncryptor.Encrypt(property.Name.ToLower()) :
                                      property.Name;

                int index = reader.GetOrdinal(propertyName);
                if (index == -1)
                {
                    Log.Warning($"Property ({propertyName} from {typeof(T)}) not found within table columns. ", nameof(SQLiteObject));
                    continue;
                }

                object value = encrypted ?
                               AESEncryptor.Decrypt(reader.GetString(index).ToString()) :
                               reader.GetValue(index);

                object arg = SQLiteHelper.ConvertValue(value, property.PropertyType);
                args.Add(arg);
            }

            return args;
        }
    }
}
