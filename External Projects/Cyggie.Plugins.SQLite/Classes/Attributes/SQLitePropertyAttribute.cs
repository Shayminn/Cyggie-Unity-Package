using System;

namespace Cyggie.Plugins.SQLite
{
    /// <summary>
    /// Assign attribute to properties in order to use them when reading from the Database
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SQLitePropertyAttribute : Attribute
    {
        /// <summary>
        /// Custom defined column name <br/>
        /// If null/empty, it'll use the Property's name instead
        /// </summary>
        public string ColumnName { get; set; } = "";
        
        /// <summary>
        /// Checks if <see cref="ColumnName"/> is defined
        /// </summary>
        internal bool HasDefinedName => !string.IsNullOrEmpty(ColumnName);

        /// <summary>
        /// Optional custom column name <br/>
        /// If column name is null/empty, it'll use the Property's name instead
        /// </summary>
        /// <param name="columnName"></param>
        public SQLitePropertyAttribute(string columnName = "")
        {
            ColumnName = columnName;
        }
    }
}
    