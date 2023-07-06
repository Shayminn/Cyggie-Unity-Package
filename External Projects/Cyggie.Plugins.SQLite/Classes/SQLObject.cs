using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cyggie.Plugins.SQLite
{
    /// <summary>
    /// 
    /// </summary>
    public class SQLObject
    {
        /// <summary>
        /// Returns array of SQLParams for SQLManager methods
        /// Parameter keys in Query must match the property name (case-insensitive)
        /// Any parameters that are not found within the Query string will be ignored
        /// </summary>
        public SQLParams[] SQLParams
        {
            get
            {
                PropertyInfo[] fields = this.GetType().GetProperties().Where(x => Attribute.IsDefined(x, typeof(SQLPropertyAttribute))).ToArray();
                List<SQLParams> paramss = new List<SQLParams>();

                // Iterate through all Properties that has attribute SQLProperty
                foreach (PropertyInfo property in fields)
                {
                    paramss.Add(new SQLParams(property.Name, property.GetValue(this)));
                }

                return paramss.ToArray();
            }
        }
    }
}
