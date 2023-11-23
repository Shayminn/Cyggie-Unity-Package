using Cyggie.Plugins.Encryption;
using Cyggie.Plugins.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyggie.Plugins.SQLite
{
    /// <summary>
    /// Class that holds a list of all database configurations
    /// </summary>
    internal class SQLiteDbConfigList
    {
        internal const string cDatabaseTableName = "cyggie_db_config";
        private const string cConfigColumnName = "config";
        private const string cValueColumnName = "value";

        private readonly List<SQLiteDbConfig> _configurations = new List<SQLiteDbConfig>();
        private readonly SQLiteDatabase? _db = null;

        private SQLiteDbConfigList(SQLiteDatabase db)
        {
            _db = db;
        }

        /// <summary>
        /// Get the value of the configuration <paramref name="config"/> <br/>
        /// Converting it to type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of the configuration value</typeparam>
        /// <param name="config">Database configuration</param>
        /// <returns>Configuration value</returns>
        public T GetConfigValue<T>(DbConfig config) => GetDbConfig(config).GetValue<T>();

        /// <summary>
        /// Get the value of the configuration <paramref name="config"/>
        /// </summary>
        /// <param name="config">Database configuration</param>
        /// <returns>Configuration value</returns>
        public object GetConfigValue(DbConfig config) => GetDbConfig(config).GetValue();

        /// <summary>
        /// Set the value of the configuration <paramref name="config"/> <br/>
        /// </summary>
        /// <param name="config">Database configuration</param>
        /// <param name="value">Configuration value to set</param>
        public void SetConfigValue(DbConfig config, object value)
        {
            SQLiteDbConfig dbConfig = GetDbConfig(config);

            object configValue = dbConfig.GetValue();
            if (configValue == null || configValue.Equals(value)) return;

            dbConfig.SetValue(value);
            Update(dbConfig);
        }

        private bool Add(SQLiteDbConfig dbConfig)
        {
            if (_configurations.Any(x => x.Config == dbConfig.Config))
            {
                Log.Error($"Trying to add an existing database configuration: {dbConfig.Config}.", nameof(SQLiteDbConfigList));
                return false;
            }

            _configurations.Add(dbConfig);
            return true;
        }

        private void Update(SQLiteDbConfig dbConfig)
        {
            if (_db == null) return;

            _db.Execute(
                query: $"UPDATE '{cDatabaseTableName}' " +
                       $"SET '{AESEncryptor.Encrypt(cValueColumnName)}'='{AESEncryptor.Encrypt(dbConfig.GetValue().ToString())}' " +
                       $"WHERE '{cDatabaseTableName}'.'{AESEncryptor.Encrypt(cConfigColumnName)}'='{AESEncryptor.Encrypt(dbConfig.Config.ToString())}'",
                bypassReadOnly: true);
        }

        private void Insert(SQLiteDbConfig dbConfig)
        {
            if (_db == null) return;

            if (Add(dbConfig))
            {
                _db.Execute(
                    query: $"INSERT INTO {cDatabaseTableName} VALUES('{AESEncryptor.Encrypt(dbConfig.Config.ToString())}', '{AESEncryptor.Encrypt(dbConfig.GetValue().ToString())}');",
                    bypassReadOnly: true
                );
            }
        }

        private SQLiteDbConfig GetDbConfig(DbConfig config)
        {
            SQLiteDbConfig dbConfig = _configurations.FirstOrDefault(x => x.Config == config);
            if (dbConfig == null)
            {
                Log.Error($"Unable to find database configuration for config {config}, adding it to database.", nameof(SQLiteDbConfigList));

                dbConfig = new SQLiteDbConfig(config);
                Insert(dbConfig);
            }

            return dbConfig;
        }

        /// <summary>
        /// Create the list of database configurations <br/>
        /// Create the table in the database and insert default values
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>The list of database configurations</returns>
        public static SQLiteDbConfigList Create(SQLiteDatabase db)
        {
            SQLiteDbConfigList configs = new SQLiteDbConfigList(db);

            StringBuilder insertValues = new StringBuilder();
            foreach (DbConfig config in Enum.GetValues(typeof(DbConfig)))
            {
                if (!string.IsNullOrEmpty(insertValues.ToString()))
                {
                    insertValues.Append(",");
                }

                SQLiteDbConfig dbConfig = new SQLiteDbConfig(config);
                configs.Add(dbConfig);

                // Insert values: ('value1','value2')
                insertValues.Append("(");
                insertValues.Append($"'{AESEncryptor.Encrypt(config.ToString())}'");
                insertValues.Append(",");
                insertValues.Append($"'{AESEncryptor.Encrypt(dbConfig.GetValue().ToString())}'");
                insertValues.Append(")");
            }

            // Create table
            db.Execute($"DROP TABLE IF EXISTS '{cDatabaseTableName}';");
            db.Execute($"CREATE TABLE '{cDatabaseTableName}' (\n" +
                       $"   '{AESEncryptor.Encrypt(cConfigColumnName)}'  TEXT,\n" +
                       $"   '{AESEncryptor.Encrypt(cValueColumnName)}'   TEXT\n" +
                       $");");

            // Insert into table
            db.Execute($"INSERT INTO {cDatabaseTableName} VALUES{insertValues};");

            return configs;
        }

        /// <summary>
        /// Read an existing list of database configurations <br/>
        /// Calls <see cref="Create(SQLiteDatabase)"/> if not found, automatically creating a new one and adding it to the database
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>The list of database configurations</returns>
        public static SQLiteDbConfigList Read(SQLiteDatabase db)
        {
            SQLiteDbConfigList configs = new SQLiteDbConfigList(db);

            if (db.ReadAll(cDatabaseTableName, out object[][] objs))
            {
                foreach (object[] row in objs)
                {
                    string enumValue = AESEncryptor.Decrypt(row[0].ToString());
                    string value = AESEncryptor.Decrypt(row[1].ToString());

                    if (!Enum.TryParse(enumValue, out DbConfig config))
                    {
                        Log.Error($"Unable to parse {enumValue} to type {typeof(DbConfig)}.", nameof(SQLiteDbConfigList));
                        continue;
                    }

                    configs.Add(new SQLiteDbConfig(config, value));
                }

                return configs;
            }
            else
            {
                return Create(db);
            }
        }
    }
}
