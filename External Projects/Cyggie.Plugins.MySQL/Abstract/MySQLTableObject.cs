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
        /// Table name that this class model represents <br/>
        /// Defaults to the type's name
        /// </summary>
        public virtual string TableName => GetType().Name;
    }
}
