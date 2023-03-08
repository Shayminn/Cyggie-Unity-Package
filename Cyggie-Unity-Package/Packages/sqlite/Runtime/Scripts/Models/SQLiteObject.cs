using Cyggie.Runtime.SQLite.Utils.Attributes;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cyggie.Runtime.SQLite.Models
{
    /// <summary>
    /// Model class for any SQLite objects
    /// </summary>
    public abstract class SQLiteObject
    {
        /// <summary>
        /// Returns array of SQLParams for SQLManager methods
        /// Parameter keys in Query must match the property name (case-insensitive)
        /// Any parameters that are not found within the Query string will be ignored
        /// </summary>
        public SqliteParameter[] SQLParams
        {
            get
            {
                PropertyInfo[] fields = GetType().GetProperties().Where(x => Attribute.IsDefined(x, typeof(SQLitePropertyAttribute))).ToArray();
                List<SqliteParameter> paramss = new List<SqliteParameter>();

                // Iterate through all Properties that has attribute SQLProperty
                foreach (PropertyInfo property in fields)
                {
                    paramss.Add(new SqliteParameter(property.Name, property.GetValue(this)));
                }

                return paramss.ToArray();
            }
        }
    }
}
