using System;

namespace Cyggie.SQLite
{
    /// <summary>
    /// Assign attribute to properties in order to use them when reading from the Database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SQLitePropertyAttribute : Attribute
    {

    }
}
