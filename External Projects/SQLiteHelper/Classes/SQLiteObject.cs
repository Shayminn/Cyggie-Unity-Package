using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cyggie.SQLite
{
    /// <summary>
    /// 
    /// </summary>
    public class SQLiteObject
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
                    paramss.Add(new SQLiteParams(property.Name, property.GetValue(this)));
                }

                return paramss.ToArray();
            }
        }
    }
}
