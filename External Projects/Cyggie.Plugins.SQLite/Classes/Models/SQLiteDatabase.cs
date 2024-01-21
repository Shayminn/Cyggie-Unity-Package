using Cyggie.Plugins.Encryption;
using Cyggie.Plugins.Logs;
using Cyggie.Plugins.SQLite.Utils.Extensions;
using Cyggie.Plugins.SQLite.Utils.Helpers;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Cyggie.Plugins.SQLite
{
    /// <summary>
    /// Helper for managing SQLite databases
    /// </summary>
    public class SQLiteDatabase
    {
        private const string cSQLiteFileExtension = ".sqlite";
        private const string cSQLFileExtension = ".sql";
        private const string cMetaFileExtension = ".meta";

        /// <summary>
        /// The database's file path
        /// </summary>
        public string DatabasePath { get; private set; }

        /// <summary>
        /// The database's name
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Determine whether the database is in Read Only mode
        /// </summary>
        public bool IsReadOnly
        {
            get => _configs != null && _configs.GetConfigValue<bool>(DbConfig.ReadOnly);
            set => _configs?.SetConfigValue(DbConfig.ReadOnly, value);
        }

        /// <summary>
        /// Determine whether the database is Encrypted <br/>
        /// When encrypted, Read Only is automatically enabled
        /// </summary>
        public bool IsEncrypted
        {
            get => _configs != null && _configs.GetConfigValue<bool>(DbConfig.Encrypted);
            set
            {
                if (_configs == null) return;

                if (value)
                {
                    Encrypt();
                }
                else
                {
                    Decrypt();
                }
            }
        }

        private readonly SqliteConnection? _conn = null;
        private readonly SQLiteDbConfigList? _configs = null;

        private SQLiteDatabase(string path, bool newDb)
        {
            DatabasePath = path;
            DatabaseName = Path.GetFileNameWithoutExtension(path);

            CreatePathToDirectory(DatabasePath);

            // File already exists, opening existing database
            if (File.Exists(path))
            {
                // Send log only while the application is running
                if (Application.isPlaying)
                {
                    Log.Debug($"Created database connection to path: {path} ({DatabaseName}).", nameof(SQLiteDatabase));
                }
            }
            // New db file
            else
            {
                Log.Debug($"Created a new database file at path: {path} ({DatabaseName}).", nameof(SQLiteDatabase));
            }

            try
            {
                _conn = new SqliteConnection($"Data Source={DatabasePath};");

                // Test connection
                _conn.Open();
                _conn.Close();

                if (newDb)
                {
                    _configs = SQLiteDbConfigList.Create(this);
                }
                else
                {
                    _configs = SQLiteDbConfigList.Read(this);
                }
            }
            catch (SqliteException ex)
            {
                Log.Error($"Failed to create/open database, exception: {ex}.", nameof(SQLiteDatabase));
            }
        }

        #region Database Control methods

        /// <summary>
        /// Encrypt the database, encrypting every single field on all tables
        /// </summary>
        public void Encrypt()
        {
            if (_configs == null) return;
            if (_configs.GetConfigValue<bool>(DbConfig.Encrypted))
            {
                Log.Warning($"Unable to encrypt database ({DatabaseName}), already encrypted!", nameof(SQLiteDatabase));
                return;
            }

            try
            {
                // Get all table names
                if (!ReadAllTableNames(applyDecryption: false, out string[] tableNames)) return;

                foreach (string tableName in tableNames)
                {
                    // Skip the cyggie_db_config table
                    if (tableName == SQLiteDbConfigList.cDatabaseTableName) continue;

                    // Get all rows from the table
                    if (!ReadAll(tableName, out object[][] rows)) continue;

                    // Get all column names from the table
                    if (!ReadAllTableColumnNames(tableName, applyEncryption: false, applyDecryption: false, out string[] columnNames)) continue;

                    // Remove everything from the table
                    Execute($"DELETE FROM '{tableName}'", true);

                    // Encrypt all rows and add them to the clean table
                    for (int row = 0; row < rows.Length; row++)
                    {
                        StringBuilder insert = new StringBuilder();
                        for (int col = 0; col < rows[row].Length; col++)
                        {
                            // Encrypt the data
                            rows[row][col] = AESEncryptor.Encrypt(rows[row][col].ToString());
                            if (!string.IsNullOrEmpty(insert.ToString()))
                            {
                                insert.Append(", ");
                            }

                            // Update the insert command
                            insert.Append($"'{rows[row][col]}'");
                        }

                        // Add the encrypted row to the table
                        Execute($"INSERT INTO '{tableName}' VALUES({insert})", true);
                    }

                    // Encrypt table column names
                    foreach (string colName in columnNames)
                    {
                        Execute($"ALTER TABLE '{tableName}' RENAME COLUMN '{colName}' TO '{AESEncryptor.Encrypt(colName)}'", true);
                    }

                    // Encrypt table name
                    Execute($"ALTER TABLE '{tableName}' RENAME TO '{AESEncryptor.Encrypt(tableName)}'", true);
                }

                _configs.SetConfigValue(DbConfig.Encrypted, true);
                _configs.SetConfigValue(DbConfig.ReadOnly, true);
                Log.Debug($"Database ({DatabaseName}) has been encrypted.", nameof(SQLiteDatabase));
            }
            catch (Exception exception)
            {
                Log.Error($"Unable to encrypt database, exception: {exception}", nameof(SQLiteDatabase));
            }
        }

        /// <summary>
        /// Decrypt the database from an encrypted state
        /// </summary>
        public void Decrypt()
        {
            if (_configs == null) return;
            if (!_configs.GetConfigValue<bool>(DbConfig.Encrypted))
            {
                Log.Warning($"Unable to decrypt database ({DatabaseName}), it is not encrypted!", nameof(SQLiteDatabase));
                return;
            }

            try
            {
                // Get all table names
                if (!ReadAllTableNames(applyDecryption: false, out string[] tableNames)) return;

                foreach (string tableName in tableNames)
                {
                    // Skip the cyggie_db_config table
                    if (tableName == SQLiteDbConfigList.cDatabaseTableName) continue;

                    // Get all table values as string (since they were encrypted)
                    if (!ReadAllAsString(tableName, out List<List<string>> rows)) continue;

                    // Get all column names from the table
                    if (!ReadAllTableColumnNames(tableName, applyEncryption: false, applyDecryption: false, out string[] columnNames)) continue;

                    // Remove everything from the table
                    Execute($"DELETE FROM '{tableName}'", true);

                    // Encrypt all rows and add them to the clean table
                    for (int row = 0; row < rows.Count; row++)
                    {
                        StringBuilder insert = new StringBuilder();
                        for (int col = 0; col < rows[row].Count; col++)
                        {
                            // Decrypt the data
                            rows[row][col] = AESEncryptor.Decrypt(rows[row][col].ToString());
                            if (!string.IsNullOrEmpty(insert.ToString()))
                            {
                                insert.Append(", ");
                            }

                            // Update the insert command
                            insert.Append($"'{rows[row][col]}'");
                        }

                        // Add the encrypted row to the table
                        Execute($"INSERT INTO '{tableName}' VALUES({insert})", true);
                    }

                    // Decrypt table column names
                    foreach (string colName in columnNames)
                    {
                        Execute($"ALTER TABLE '{tableName}' RENAME COLUMN '{colName}' TO '{AESEncryptor.Decrypt(colName)}'", true);
                    }

                    // Decrypt table name
                    Execute($"ALTER TABLE '{tableName}' RENAME TO '{AESEncryptor.Decrypt(tableName)}'", true);
                }

                _configs.SetConfigValue(DbConfig.Encrypted, false);
                Log.Debug($"Database ({DatabaseName}) has been decrypted.", nameof(SQLiteDatabase));
            }
            catch (Exception exception)
            {
                Log.Error($"Unable to decrypt database, exception: {exception}", nameof(SQLiteDatabase));
            }
        }

        /// <summary>
        /// Change the name of the database <br/>
        /// This will automatically rename the file
        /// </summary>
        /// <param name="newName">New name assigned to the database</param>
        /// <returns>Successful?</returns>
        public bool ChangeDatabaseName(string newName)
        {
            if (newName == DatabaseName) return false;

            try
            {
                string oldDbPath = DatabasePath;
                string newDbPath = GetUniquePath($"{Path.GetDirectoryName(DatabasePath)}/{newName}{cSQLiteFileExtension}");

                // Move the meta file
                File.Move($"{oldDbPath}{cMetaFileExtension}", $"{newDbPath}{cMetaFileExtension}");

                // Move the sqlite file
                File.Move(oldDbPath, newDbPath);

                DatabasePath = newDbPath;
                DatabaseName = newName;
                Log.Debug($"Renamed database from \"{DatabaseName}\" to \"{newName}\".", nameof(SQLiteDatabase));
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Unable to change database name: old name: {DatabaseName}, new name: {newName}, exception: {ex}", nameof(SQLiteDatabase));
                return false;
            }
        }

        /// <summary>
        /// Delete this SQLite database
        /// </summary>
        /// <returns>Successful?</returns>
        public bool Delete()
        {
            try
            {
                // Delete the meta file aswell
                File.Delete($"{DatabasePath}.meta");

                // Delete the sqlite file
                File.Delete(DatabasePath);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Unable to delete database: {DatabaseName}, exception: {ex}", nameof(SQLiteDatabase));
                return false;
            }
        }

        /// <summary>
        /// Create a new blueprint to rebuild the whole SQL database. <br/>
        /// Specify <paramref name="path"/> to create a file.
        /// </summary>
        /// <param name="path">Path to create .sql file (specify its name as well)</param>
        /// <returns>Blueprint in string</returns>
        public string CreateBlueprint(string path)
        {
            if (_conn == null)
            {
                Log.Error("Failed to create blueprint, connection is null.", nameof(SQLiteDatabase));
                return "";
            }

            if (string.IsNullOrEmpty(path))
            {
                Log.Error("Failed to create blueprint, path is null or empty.", nameof(SQLiteDatabase));
                return "";
            }

            // Make sure path ends with .sql
            if (!path.EndsWith(cSQLFileExtension))
            {
                path += cSQLFileExtension;
            }

            CreatePathToDirectory(path);
            _conn.Open();

            List<string> tableNames = new List<string>();
            StringBuilder blueprint = new StringBuilder();

            using SqliteCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT name, sql FROM sqlite_master WHERE type='table'";

            // Create table sql statements
            // Iterate through all the table names and its create sqls
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = reader.GetString(0);
                    tableNames.Add(name);

                    // Drop table if exists command
                    blueprint.Append($"DROP TABLE IF EXISTS '{name}';\n");

                    // Create table command
                    blueprint.Append($"{reader.GetString(1)};\n\n");
                }
            }

            _conn.Close();

            // Insert sql statements
            // Iterate through all the tables
            foreach (string name in tableNames)
            {
                // Get row count (this will open/close the connection)
                Count(name, out int rowCount);

                _conn.Open();

                // Read all rows from table
                command.CommandText = $"SELECT * FROM '{name}'";

                using SqliteDataReader reader = command.ExecuteReader();

                // Check if there's any row to insert
                if (!reader.HasRows) continue;

                // Insert SQL statement
                StringBuilder insertSQL = new StringBuilder($"INSERT INTO '{name}' VALUES");
                int currentRow = 1;

                while (reader.Read())
                {
                    insertSQL.Append("(");

                    // Add all columns to insert values
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        insertSQL.Append($"'{reader.GetValue(i).ToString().Replace("'", "''")}'");

                        // Add , if there's more values or ; if it's the last
                        if (i < reader.FieldCount - 1)
                        {
                            insertSQL.Append(",");
                        }
                    }

                    insertSQL.Append(")");

                    // Add , if there's more values or ; if it's the last
                    insertSQL.Append(currentRow < rowCount ? "," : ";\n");
                    ++currentRow;
                }

                blueprint.Append(insertSQL);
                blueprint.Append("\n");

                _conn.Close();
            }

            // Garbage collect because Close() is not releasing the db file
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Write the new blueprint to the path
            File.WriteAllText(path, blueprint.ToString());

            Log.Debug($"Blueprint successfully created at: {path}", nameof(SQLiteDatabase));
            return blueprint.ToString();
        }

        #endregion

        #region Query methods

        /// <summary>
        /// Execute a non-query on the database <br/>
        /// For queries, use <see cref="Read"/> or <see cref="ReadAll"/>
        /// </summary>
        /// <param name="query">Non-Query command to execute</param>
        /// <param name="sqlParams">Parameters to add to command</param>
        /// <returns>Number of rows affected</returns>
        public int Execute(string query, params SQLiteParams[] sqlParams) => Execute(query, false, sqlParams);

        /// <summary>
        /// Execute a non-query on the database <br/>
        /// For queries, use <see cref="Read"/> or <see cref="ReadAll"/>
        /// </summary>
        /// <param name="query">Non-Query command to execute</param>
        /// <param name="bypassReadOnly">Whether this call should ignore whether the database is in read-only or not</param>
        /// <param name="sqlParams">Parameters to add to command</param>
        /// <returns>Number of rows affected</returns>
        internal int Execute(string query, bool bypassReadOnly, params SQLiteParams[] sqlParams)
        {
            if (_conn == null) return 0;
            if (IsReadOnly && !bypassReadOnly)
            {
                Log.Warning("Unable to perform Execute, database is in read-only mode. Only Read methods are allowed.", nameof(SQLiteDatabase));
                return 0;
            }

            _conn.Open();

            using SqliteCommand command = _conn.CreateCommand();
            command.CommandText = query;
            command.AddParameters(sqlParams);

            int result = 0;
            try
            {
                result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to execute command: \"{command.CommandText}\" from the database ({DatabaseName}), exception: {ex}", nameof(SQLiteDatabase));
            }

            _conn.Close();
            return result;
        }

        /// <summary>
        /// Execute a read command for the first row found <br/>
        /// This reads every single column defined in the table (SELECT *) <br/>
        /// Make sure there is a constructor that matches the number of Properties and their associated types
        /// </summary>
        /// <typeparam name="T">Object class</typeparam>
        /// <param name="tableName">Table name to read from</param>
        /// <param name="obj">Outputted object</param>
        /// <param name="suffix">Suffix to add to command after "SELECT * FROM <paramref name="tableName"/>"</param>
        /// <param name="sqlParams">Parameters to the command</param>
        /// <returns>Success? (returns false if no row was found)</returns>
        public bool Read<T>(string tableName, out T? obj, string suffix = "", params SQLiteParams[] sqlParams) where T : SQLiteObject
        {
            obj = default;
            if (_conn == null) return false;
            ApplyEncryption(ref tableName, sqlParams);

            _conn.Open();
            bool success = false;

            using SqliteCommand command = _conn.CreateCommand();
            command.CommandText = $"SELECT * FROM '{tableName}'";
            command.AddParameters(sqlParams);
            command.CommandText += $" {suffix}";

            try
            {
                using SqliteDataReader reader = command.ExecuteReader();

                IEnumerable<PropertyInfo> fields = SQLiteObject.GetFields<T>();
                if (reader.Read())
                {
                    IEnumerable<object> args = SQLiteObject.GetConstructorArguments<T>(fields, reader, IsEncrypted);

                    // Try to create an instance of object T
                    success = TryCreateInstance(args, out obj);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read from the database ({DatabaseName}). Command: {command} \nException: {ex}", nameof(SQLiteDatabase));
            }

            _conn.Close();
            return success;
        }

        /// <summary>
        /// Execute a read command for a 2D array of each individual field (as <see cref="object"/> without a specified type)
        /// </summary>
        /// <param name="tableName">Table name to read from</param>
        /// <param name="objs">Outputted 2D array objects</param>
        /// <param name="suffix">Suffix to add to command after "SELECT * FROM <paramref name="tableName"/>"</param>
        /// <param name="sqlParams">Parameters to the command</param>
        /// <returns>Success?</returns>
        public bool ReadAll(string tableName, out object[][] objs, string suffix = "", params SQLiteParams[] sqlParams)
        {
            objs = new object[][] { };
            if (_conn == null) return false;
            ApplyEncryption(ref tableName, sqlParams);

            _conn.Open();
            bool success = false;
            List<object[]> temp = new List<object[]>();
            using SqliteCommand command = _conn.CreateCommand();

            try
            {
                command.CommandText = $"SELECT * FROM '{tableName}'";
                command.AddParameters(sqlParams);
                command.CommandText += $" {suffix}";

                using SqliteDataReader reader = command.ExecuteReader();

                for (int row = 0; reader.Read(); row++)
                {
                    object[] arr = new object[reader.VisibleFieldCount];
                    reader.GetValues(arr);

                    if (IsEncrypted)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            arr[i] = AESEncryptor.Encrypt(arr[i].ToString());
                        }
                    }

                    temp.Add(arr);
                }

                objs = temp.ToArray();
                success = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read all from the database ({DatabaseName}). Command: {command} \nException: {ex}", nameof(SQLiteDatabase));
            }

            _conn.Close();
            return success;
        }

        /// <summary>
        /// Execute a read command for rows found within the table<br/>
        /// This reads every single column defined in the table (SELECT *) <br/>
        /// Make sure there is a constructor that matches the number of Properties and their associated types
        /// </summary>
        /// <typeparam name="T">Object class</typeparam>
        /// <param name="tableName">Table name to read from</param>
        /// <param name="list">Outputted IEnumerable objects</param>
        /// <param name="suffix">Suffix to add to command after "SELECT * FROM <paramref name="tableName"/>"</param>
        /// <param name="sqlParams">Parameters to the command</param>
        /// <returns>Success? (true even if no rows are found)</returns>
        public bool ReadAll<T>(string tableName, out IEnumerable<T> list, string suffix = "", params SQLiteParams[] sqlParams) where T : SQLiteObject
        {
            list = new List<T>();
            if (_conn == null) return false;
            ApplyEncryption(ref tableName, sqlParams);

            _conn.Open();

            List<T> tempList = new List<T>();
            bool success = false;
            using SqliteCommand command = _conn.CreateCommand();

            try
            {
                command.CommandText = $"SELECT * FROM '{tableName}'";
                command.AddParameters(sqlParams);
                command.CommandText += $" {suffix}";

                using SqliteDataReader reader = command.ExecuteReader();

                IEnumerable<PropertyInfo> fields = SQLiteObject.GetFields<T>();
                while (reader.Read())
                {
                    IEnumerable<object> args = SQLiteObject.GetConstructorArguments<T>(fields, reader, IsEncrypted);

                    if (!TryCreateInstance(args, out T? obj) || obj == null) continue;

                    tempList.Add(obj);
                }

                list = tempList;
                success = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read from the database ({DatabaseName}). Command: {command.CommandText} \nException: {ex}", nameof(SQLiteDatabase));
            }

            _conn.Close();
            return success;
        }

        /// <summary>
        /// Read all table names from the database
        /// </summary>
        /// <param name="tableNames">Outputted array of table names</param>
        /// <returns>Success?</returns>
        public bool ReadAllTableNames(out string[] tableNames) => ReadAllTableNames(true, out tableNames);

        private bool ReadAllTableNames(bool applyDecryption, out string[] tableNames)
        {
            tableNames = new string[] { };
            if (_conn == null) return false;

            _conn.Open();
            bool success = false;
            List<string> temp = new List<string>();

            try
            {
                using SqliteCommand command = _conn.CreateCommand();
                command.CommandText = "SELECT name FROM sqlite_schema WHERE type='table' AND name NOT LIKE 'sqlite_%';";

                using SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string tableName = applyDecryption && IsEncrypted ?
                                       AESEncryptor.Decrypt(reader.GetString(0)) :
                                       reader.GetString(0);

                    temp.Add(tableName);
                }

                tableNames = temp.ToArray();
                success = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read all table names from the database ({DatabaseName}), exception: {ex}", nameof(SQLiteDatabase));
            }

            _conn.Close();
            return success;
        }

        /// <summary>
        /// Read all column names from the specified table
        /// </summary>
        /// <param name="tableName">Table name to read from</param>
        /// <param name="columnNames">Outputted array of column names</param>
        /// <returns></returns>
        public bool ReadAllTableColumnNames(string tableName, out string[] columnNames) => ReadAllTableColumnNames(tableName, true, true, out columnNames);

        private bool ReadAllTableColumnNames(string tableName, bool applyEncryption, bool applyDecryption, out string[] columnNames)
        {
            columnNames = new string[] { };
            if (_conn == null) return false;
            if (applyEncryption)
            {
                ApplyEncryption(ref tableName);
            }

            _conn.Open();
            bool success = false;
            List<string> temp = new List<string>();

            try
            {
                // Get all column names
                using SqliteCommand columnCommand = _conn.CreateCommand();
                columnCommand.CommandText = $"SELECT name FROM pragma_table_info('{tableName}')";

                using SqliteDataReader columnReader = columnCommand.ExecuteReader();
                while (columnReader.Read())
                {
                    string columnName = applyDecryption && IsEncrypted ?
                                        AESEncryptor.Decrypt(columnReader.GetString(0)) :
                                        columnReader.GetString(0);

                    temp.Add(columnName);
                }

                columnNames = temp.ToArray();
                success = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read all column names from table ({tableName}) in the database ({DatabaseName}), exception: {ex}", nameof(SQLiteDatabase));
            }

            _conn.Close();
            return success;
        }

        /// <summary>
        /// Execute a count of the table
        /// </summary>
        /// <param name="tableName">Table name to read from</param>
        /// <param name="count">Number of rows found</param>
        /// <param name="suffix">Suffix to add to command after the "SELECT COUNT(*) FROM <paramref name="tableName"/>"</param>
        /// <param name="sqlParams">SQL Params</param>
        /// <returns>Success?</returns>
        public bool Count(string tableName, out int count, string suffix = "", params SQLiteParams[] sqlParams)
        {
            count = 0;
            if (_conn == null) return false;

            _conn.Open();
            bool success = false;

            using SqliteCommand command = _conn.CreateCommand();
            try
            {
                command.CommandText = $"SELECT COUNT(*) FROM '{tableName}'";
                command.AddParameters(sqlParams);

                count = (int) SQLiteHelper.ConvertValue(command.ExecuteScalar(), typeof(int));
                success = count > 0;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get count from the database ({DatabaseName}). exception: {ex}", nameof(SQLiteDatabase));
            }

            _conn.Close();
            return success;
        }

        /// <summary>
        /// Same as <see cref="ReadAll(string, out object[][], string, SQLiteParams[])"/> but as string regardless of the field's datatype
        /// </summary>
        /// <param name="tableName">The table name to read from</param>
        /// <param name="rows">2D list of rows/columns as string</param>
        /// <returns>Success?</returns>
        internal bool ReadAllAsString(string tableName, out List<List<string>> rows)
        {
            rows = new List<List<string>>();
            if (_conn == null) return false;
            if (!ReadAllTableColumnNames(tableName, applyEncryption: false, applyDecryption: false, out string[] columnNames)) return false;

            _conn.Open();
            bool success = false;

            try
            {
                foreach (string colName in columnNames)
                {
                    // Get all rows by column name
                    SqliteCommand rowCommand = _conn.CreateCommand();
                    rowCommand.CommandText = $"SELECT CAST('{tableName}'.'{colName}' AS VARCHAR(255)) FROM '{tableName}'";

                    SqliteDataReader rowReader = rowCommand.ExecuteReader();
                    for (int row = 0; rowReader.Read(); row++)
                    {
                        if (row >= rows.Count)
                        {
                            rows.Add(new List<string>());
                        }

                        rows[row].Add(rowReader.GetString(0));
                    }
                }

                success = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read all as string from the database ({DatabaseName}), exception: {ex}", nameof(SQLiteDatabase));
            }

            _conn.Close();
            return success;
        }

        #endregion

        #region Util methods

        private bool TryCreateInstance<T>(IEnumerable<object> args, out T? obj) where T : SQLiteObject
        {
            obj = default;

            try
            {
                obj = (T) Activator.CreateInstance(typeof(T), args.ToArray());
                return true;
            }
            catch (MissingMethodException ex)
            {
                string argsStr = "";
                foreach (object o in args)
                {
                    if (!string.IsNullOrEmpty(argsStr))
                        argsStr += ", ";

                    argsStr += o.GetType();
                }

                Log.Error($"Unable to create object, no valid constructor was found for {typeof(T)} with {args.Count()} arguments ({argsStr}), exception {ex}", nameof(SQLiteDatabase));
                return false;
            }
        }

        private void CreatePathToDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
        }

        private void ApplyEncryption(ref string tableName, params SQLiteParams[] sqlParams)
        {
            if (!IsEncrypted) return;

            tableName = AESEncryptor.Encrypt(tableName);
            foreach (SQLiteParams sqlParam in sqlParams)
            {
                sqlParam.Encrypt();
            }
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Create a new SQLite database at the specified <paramref name="databasePath"/>
        /// </summary>
        /// <param name="databasePath">The full path where the database will be created</param>
        /// <param name="db">The created SQLite database (null if exception)</param>
        /// <returns>Successful?</returns>
        public static bool TryCreate(string databasePath, out SQLiteDatabase? db)
        {
            if (!databasePath.EndsWith(cSQLiteFileExtension))
            {
                databasePath += cSQLiteFileExtension;
            }

            databasePath = GetUniquePath(databasePath);
            db = null;

            try
            {
                db = new SQLiteDatabase(databasePath, true);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to create new SQLite database, exception: {ex}", nameof(SQLiteDatabase));
                return false;
            }
        }

        /// <summary>
        /// Open a new SQLite database at the specified <paramref name="databasePath"/> <br/>
        /// Automatically creates one if not found
        /// </summary>
        /// <param name="databasePath">The full path where the database is</param>
        /// <param name="db">The created SQLite database (null if exception)</param>
        /// <returns>Successful? (may return true even if creating a new database)</returns>
        public static bool TryOpen(string databasePath, out SQLiteDatabase? db)
        {
            if (!databasePath.EndsWith(cSQLiteFileExtension))
            {
                databasePath = $"{databasePath}{cSQLiteFileExtension}";
            }

            db = null;

            try
            {
                if (File.Exists(databasePath))
                {
                    db = new SQLiteDatabase(databasePath, false);
                    return true;
                }

                return TryCreate(databasePath, out db);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to open new SQLite database, exception: {ex}", nameof(SQLiteDatabase));
                return false;
            }
        }

        private static string GetUniquePath(string originalPath)
        {
            int index = 1;
            string uniqueDbPath = originalPath;
            while (File.Exists(uniqueDbPath))
            {
                uniqueDbPath = $"{originalPath[..^cSQLiteFileExtension.Length]} ({index}){cSQLiteFileExtension}";
                ++index;
            }

            return uniqueDbPath;
        }

        #endregion
    }
}
