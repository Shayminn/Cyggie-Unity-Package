using System;
using System.Collections.Generic;
using System.Text;

namespace Cyggie.Plugins.SQLite
{
    /// <summary>
    /// Assign attribute to properties in order to use them when reading from the Database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SQLPropertyAttribute : Attribute
    {

    }
}
