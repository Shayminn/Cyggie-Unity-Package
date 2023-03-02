using System;
using System.Collections.Generic;
using System.Text;

namespace Cyggie.SQLite
{
    /// <summary>
    /// SQL Params used for SQLManager methods to replace Parameters
    /// </summary>
    public class SQLiteParams
    {
        internal string ParameterKey { get; private set; }

        internal object ParameterValue { get; private set; }
       
        /// <summary>
        /// Constructor with key and value
        /// </summary>
        /// <param name="key">Parameter key</param>
        /// <param name="value">Parameter value</param>
        public SQLiteParams(string key, object value)
        {
            if (!key.StartsWith('@'))
            {
                key = key.Insert(0, "@");
            }

            ParameterKey = key.ToLower();
            ParameterValue = value;
        }

        /// <summary>
        /// Override default ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Key: {ParameterKey} | Value: {ParameterValue}";
        }
    }
}
