using Cyggie.Plugins.Encryption;

namespace Cyggie.Plugins.SQLite
{
    /// <summary>
    /// SQL Params used for SQLManager methods to replace Parameters
    /// </summary>
    public class SQLiteParams
    {
        /// <summary>
        /// The database's column name
        /// </summary>
        public string ParameterKey { get; private set; }

        /// <summary>
        /// The assigned value to the <see cref="ParameterKey"/>
        /// </summary>
        public object ParameterValue { get; private set; }

        /// <summary>
        /// Constructor with key/value pair
        /// </summary>
        /// <param name="key">The database's column name</param>
        /// <param name="value">The assigned value to the <see cref="ParameterKey"/></param>
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
        /// Encrypt the parameter key and value
        /// </summary>
        internal void Encrypt()
        {
            ParameterKey = AESEncryptor.Encrypt(ParameterKey);
            ParameterValue = AESEncryptor.Encrypt(ParameterValue);
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
