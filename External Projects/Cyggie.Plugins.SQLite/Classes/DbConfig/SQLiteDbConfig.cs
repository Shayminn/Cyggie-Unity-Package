using Cyggie.Plugins.Logs;
using Cyggie.Plugins.SQLite.Utils.Extensions;
using System;

namespace Cyggie.Plugins.SQLite
{
    /// <summary>
    /// The SQLite object that holds information regarding the state of the database and its configurations <br/>
    /// Not inheriting from <see cref="SQLiteObject"/> because this object is read manually created
    /// </summary>
    internal class SQLiteDbConfig
    {
        /// <summary>
        /// The database configuration associated to this object
        /// </summary>
        public DbConfig Config { get; set; } = DbConfig.Encrypted;

        /// <summary>
        /// Value of the configuration
        /// </summary>
        private object Value { get; set; } = "";

        /// <summary>
        /// Constructor used when reading from the database
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="value">Value of the configuration</param>
        internal SQLiteDbConfig(DbConfig config, string value)
        {
            Config = config;
            Value = value;
        }

        /// <summary>
        /// Constructor used when creating a new/missing configuration <br/>
        /// Value is set to its default based on <see cref="DbConfigExtensions.GetDefaultValue"/>
        /// </summary>
        /// <param name="config">Configuration</param>
        internal SQLiteDbConfig(DbConfig config)
        {
            Config = config;
            Value = config.GetDefaultValue();
        }

        /// <summary>
        /// Get the value of the configuration in the right type
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <returns>Value in type <typeparamref name="T"/></returns>
        internal T GetValue<T>()
        {
            try
            {
                object value;

                // This is necessary to convert False into false
                if (typeof(T) == typeof(bool))
                {
                    value = Convert.ToBoolean(Value);
                }
                else
                {
                    value = Value;
                }

                return (T) value;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to convert {Value} to type {typeof(T)}, exception: {ex}", nameof(SQLiteDbConfig));

#pragma warning disable CS8603 // Possible null reference return.
                return default;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }

        /// <summary>
        /// Get the value of the configuration as an <see cref="object"/>
        /// </summary>
        /// <returns>Value as object</returns>
        internal object GetValue() => Value;

        /// <summary>
        /// Set the value of the configuration
        /// </summary>
        /// <param name="value"></param>
        internal void SetValue(object value)
        {
            if (value == null) return;
            Value = value;
        }
    }
}
