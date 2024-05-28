using Cyggie.Plugins.MySQL.Abstract;
using System;

namespace Cyggie.Plugins.MySQL.Attributes
{
    /// <summary>
    /// Attribute to preload a <see cref="MySQLTableObject"/> when the database is connected <br/>
    /// Preloaded tables are added to the pool, when reading from them, it won't query the database and simply retrieve <br/>
    /// By this nature, they should be read-only and not be written to. <br/>
    /// If the data somehow changes, it won't be reflected in the pool and the pool can't be updated unless the database connection is re-established
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MySQLTableNameAttribute : Attribute
    {
        /// <summary>
        /// MySQL table name <br/>
        /// Defaults to the class name
        /// </summary>
        public string TableName { get; private set; } = string.Empty;

        /// <summary>
        /// Attribute to preload all the table data when the database is connected
        /// </summary>
        /// <param name="tableName"></param>
        public MySQLTableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
