using Cyggie.Plugins.MySQL.Attributes;
using Cyggie.Plugins.MySQL.Utils.Helpers;

namespace Cyggie.Plugins.MySQL.Abstract
{
    /// <summary>
    /// Class for a MySQL table's object model <br/>
    /// The fields are assigned automatically based on the field/property names and table columns (using JSON conversion with the Newtonsoft Json library)<br/>
    /// <br/>
    /// <code>
    /// Example table
    /// field1 | field2
    ///    1      test
    ///    
    /// Example class fields/properties
    /// public int Field1;
    /// public string Field2 { get; set; }
    /// </code>
    /// </summary>
    public abstract class MySQLTableObject
    {
        /// <summary>
        /// Table name for this type <br/>
        /// Use <see cref="MySQLTableNameAttribute"/> to assign this value
        /// </summary>
        public string TableName => MySQLTableHelper.GetTableName(GetType());

        internal void OnObjectCreated_Internal() => OnObjectCreated();

        /// <summary>
        /// Called when the object is created
        /// </summary>
        protected virtual void OnObjectCreated()
        {

        }
    }
}
