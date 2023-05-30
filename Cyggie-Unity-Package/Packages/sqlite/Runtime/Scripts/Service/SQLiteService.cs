using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Main.Runtime.Utils.Extensions;
using Cyggie.Runtime.SQLite.Models;
using Cyggie.Runtime.SQLite.Utils.Attributes;
using Cyggie.Runtime.SQLite.Utils.Extensions;
using Cyggie.Runtime.SQLite.Utils.Helpers;
using Cyggie.SQLite.Runtime.Utils.Constants;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Cyggie.SQLite.Runtime.ServicesNS
{
    /// <summary>
    /// Service for managing an SQLite database <br/>
    /// Execute SQLite queries <br/>
    /// Uses model class <see cref="SQLiteObject"/>
    /// </summary>
    public class SQLiteService : Service
    {
        private IDbConnection _connection = null;
        private bool _readOnly = false;
        private bool _readAllOnStart = false;
        private bool _addTrailingS = false;
        private readonly List<SQLiteObject> _sqliteObjects = new List<SQLiteObject>();

        private bool IsInitialized => _connection != null;

        /// <inheritdoc/>
        protected override void OnInitialized(ServiceConfigurationSO configuration)
        {
            base.OnInitialized(configuration);

            // Get the service configuration
            if (configuration == null || configuration is not SQLiteSettings sqliteSettings)
            {
                Debug.LogError($"Failed in {nameof(OnInitialized)}, configuration is null or is not type of {typeof(SQLiteSettings)}.");
                return;
            }

            _readOnly = sqliteSettings.ReadOnly;
            _readAllOnStart = sqliteSettings.ReadAllOnStart;
            _addTrailingS = sqliteSettings.AddSToTableName;

            // Make sure the database path exists
            string databasePath = Application.streamingAssetsPath + sqliteSettings.DatabasePath;
            if (!Directory.Exists(databasePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(databasePath));
            }

            _connection = new SqliteConnection($"{Constants.cDatabaseURI}{databasePath}");

            // Test database connection
            try
            {
                _connection.Open();
                _connection.Close();
            }
            catch (Exception ex)
            {
                _connection = null; // this signals that IsInitialized = false
                Debug.LogError($"Failed on {nameof(OnInitialized)}, test connection failed, connection string: {_connection.ConnectionString}\n, exception: {ex}.");
                return;
            }

            // Read all objects in database and add it to 
            if (sqliteSettings.ReadAllOnStart)
            {
                List<Type> objectTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(SQLiteObject)) && !t.IsAbstract).ToList();

                foreach (Type type in objectTypes)
                {
                    if (ReadAll(type, out IEnumerable<SQLiteObject> sqliteObjects))
                    {
                        _sqliteObjects.AddRange(sqliteObjects);
                    }
                }
            }
        }

        #region Public API

        /// <summary>
        /// Execute a non query command
        /// </summary>
        /// <param name="query">Command string</param>
        /// <param name="sqlParams">Parameters to the command (The key must be found within the query string)</param>
        /// <returns>Number of rows affected (0 if failed)</returns>
        public int Execute(string query, params SqliteParameter[] sqlParams)
        {
            if (!IsInitialized)
            {
                Debug.LogError($"Failed in {nameof(Execute)}, SQLite Service is not initialized.");
                return 0;
            }

            if (_readOnly)
            {
                Debug.LogError($"Failed in {nameof(Execute)}, SQLite Service is in read-only mode, use {nameof(Read)} instead.");
                return 0;
            }

            int result = 0;

            try
            {
                OpenConnection();

                // Create query command
                using IDbCommand command = _connection.CreateCommand();
                command.CommandText = query;
                command.AddParameters(false, sqlParams);

                result = command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                string paramss = "";
                foreach (SqliteParameter sqlParam in sqlParams)
                {
                    if (!string.IsNullOrEmpty(paramss))
                        paramss += ", ";

                    paramss += $"[{sqlParam.ParameterName}, {sqlParam.Value}]";
                }

                Debug.LogError($"SQLite exception when executing Query:\n{query}\n (Parameters: {paramss})\nException: {ex}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Cyggie.SQLite] Error when executing SQL Statement: {ex}\n{ex.StackTrace}");
            }
            finally
            {
                CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// Get all sqlite objects of type <typeparamref name="T"/> <br/>
        /// This does not open a connection to the SQLite database but simply retrieves it from a local list, initialized on start when <see cref="SQLiteSettings.ReadAllOnStart"/> is enabled
        /// </summary>
        /// <typeparam name="T">Type of SQLiteObject</typeparam>
        /// <returns>Collection of <typeparamref name="T"/> objects (empty list if failed)</returns>
        public IEnumerable<T> Get<T>()
        {
            if (!_readAllOnStart)
            {
                Debug.LogError($"Failed in {nameof(Get)}, list of objects is empty since {nameof(SQLiteSettings.ReadAllOnStart)} is disabled. Enable it in Package Configurations or use {nameof(Read)} instead.");
                return new List<T>();
            }

            return _sqliteObjects.Where(x => x.GetType() == typeof(T)).Cast<T>();
        }

        #region Read methods

        #region Read

        /// <summary>
        /// Execute a read command (for a single object)
        /// Assign the SQLiteProperty to each Property of the object that needs to be read from the Database
        /// Make sure there is a constructor that matches the number of Properties and their associated types
        /// Name of table is defined as typeof(T)s
        /// </summary>
        /// <typeparam name="T">Object class</typeparam>
        /// <param name="obj">Outputs the object</param>
        /// <param name="suffix">Suffix to add to command after the WHERE statement (i.e. GROUP BY)</param>
        /// <param name="sqlParams">Parameters to the command</param>
        /// <returns>Success? (returns false if no row was found)</returns>
        public bool Read<T>(out T obj, string suffix = "", params SqliteParameter[] sqlParams) where T : SQLiteObject
        {
            return Read(typeof(T).Name.ToLower(), out obj, suffix, sqlParams);
        }

        /// <summary>
        /// Execute a read command (for a single object)
        /// Assign the SQLiteProperty to each Property of the object that needs to be read from the Database
        /// Make sure there is a constructor that matches the number of Properties and their associated types
        /// </summary>
        /// <typeparam name="T">Object class</typeparam>
        /// <param name="tableName">Table name to read from</param>
        /// <param name="obj">Outputs the object</param>
        /// <param name="suffix">Suffix to add to command after the WHERE statement (i.e. GROUP BY)</param>
        /// <param name="sqlParams">Parameters to the command</param>
        /// <returns>Success? (returns false if no row was found)</returns>
        public bool Read<T>(string tableName, out T obj, string suffix = "", params SqliteParameter[] sqlParams) where T : SQLiteObject
        {
            bool success = Read(tableName, typeof(T), out SQLiteObject sqliteObject, suffix, sqlParams);
            obj = (T) sqliteObject;

            return success;
        }

        /// <summary>
        /// Execute a read command (for a single object)
        /// Assign the SQLiteProperty to each Property of the object that needs to be read from the Database
        /// Make sure there is a constructor that matches the number of Properties and their associated types
        /// </summary>
        /// <param name="type">Type of SQLiteObject</param>
        /// <param name="obj">Outputs the object</param>
        /// <param name="suffix">Suffix to add to command after the WHERE statement (i.e. GROUP BY)</param>
        /// <param name="sqlParams">Parameters to the command</param>
        /// <returns>Success? (returns false if no row was found)</returns>
        public bool Read(Type type, out SQLiteObject obj, string suffix = "", params SqliteParameter[] sqlParams)
        {
            return Read(type.Name.ToLower(), type, out obj, suffix, sqlParams);
        }

        /// <summary>
        /// Execute a read command (for a single object)
        /// Assign the SQLiteProperty to each Property of the object that needs to be read from the Database
        /// Make sure there is a constructor that matches the number of Properties and their associated types
        /// </summary>
        /// <typeparam name="T">Object class</typeparam>
        /// <param name="tableName">Table name to read from</param>
        /// <param name="obj">Outputs the object</param>
        /// <param name="suffix">Suffix to add to command after the WHERE statement (i.e. GROUP BY)</param>
        /// <param name="sqlParams">Parameters to the command</param>
        /// <returns>Success? (returns false if no row was found)</returns>
        public bool Read(string tableName, Type type, out SQLiteObject obj, string suffix = "", params SqliteParameter[] sqlParams)
        {
            obj = default;
            bool success = false;

            IDbCommand command = null;
            try
            {
                OpenConnection();

                // Create query command
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM " + ((_addTrailingS) ? tableName.InsertEndsWith("s") : tableName).ToLower();
                command.AddParameters(true, sqlParams);
                command.CommandText += $" {suffix}";

                Debug.Log(command.CommandText);
                using IDataReader reader = command.ExecuteReader();

                List<object> args = new List<object>();
                PropertyInfo[] fields = type.GetProperties().Where(x => Attribute.IsDefined(x, typeof(SQLitePropertyAttribute))).ToArray();

                if (reader.Read())
                {
                    foreach (PropertyInfo property in fields)
                    {
                        int index = reader.GetOrdinal(property.Name);
                        if (index == -1)
                        {
                            Debug.LogWarning($"Property ({property.Name} from {type}) not found within table columns. ");
                            continue;
                        }
                        object arg = SQLiteHelper.ConvertValue(reader.GetValue(index), property.PropertyType);
                        args.Add(arg);
                    }

                    // Try to create an instance of object T
                    success = TryCreateInstance(type, args, out obj);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to read from the database. Command: " + (command != null ? command.CommandText : "") + $", Exception: \n{ex}");
            }
            finally
            {
                CloseConnection();
            }

            return success;
        }

        #endregion

        #region Read All

        /// <summary>
        /// Execute a read command (for a list of objects)
        /// Assign the SQLiteProperty to each Property of the object that needs to be read from the Database
        /// Make sure there is a constructor that matches the number of Properties and their associated types
        /// </summary>
        /// <typeparam name="T">Object class</typeparam>
        /// <param name="collection">Outputs a list of object</param>
        /// <param name="suffix">Suffix to add to command after the WHERE statement (i.e. GROUP BY)</param>
        /// <param name="sqlParams">Parameters to the command</param>
        /// <returns>Success?</returns>
        public bool ReadAll<T>(out IEnumerable<T> collection, string suffix = "", params SqliteParameter[] sqlParams) where T : SQLiteObject
        {
            return ReadAll(typeof(T).Name.ToLower(), out collection, suffix, sqlParams);
        }

        /// <summary>
        /// Execute a read command (for a list of objects)
        /// Assign the SQLiteProperty to each Property of the object that needs to be read from the Database
        /// Make sure there is a constructor that matches the number of Properties and their associated types
        /// </summary>
        /// <typeparam name="T">Object class</typeparam>
        /// <param name="tableName">Table name to read from</param>
        /// <param name="collection">Outputs a list of object</param>
        /// <param name="suffix">Suffix to add to command after the WHERE statement (i.e. GROUP BY)</param>
        /// <param name="sqlParams">Parameters to the command</param>
        /// <returns>Success?</returns>
        public bool ReadAll<T>(string tableName, out IEnumerable<T> collection, string suffix = "", params SqliteParameter[] sqlParams) where T : SQLiteObject
        {
            bool success = ReadAll(tableName, typeof(T), out IEnumerable<SQLiteObject> objCollection, suffix, sqlParams);
            collection = objCollection.Cast<T>();

            return success;
        }

        /// <summary>
        /// Execute a read command (for a list of objects)
        /// Assign the SQLiteProperty to each Property of the object that needs to be read from the Database
        /// Make sure there is a constructor that matches the number of Properties and their associated types
        /// </summary>
        /// <param name="type">Type of object to read</param>
        /// <param name="list">Outputs a list of object</param>
        /// <param name="suffix">Suffix to add to command after the WHERE statement (i.e. GROUP BY)</param>
        /// <param name="sqlParams">Parameters to the command</param>
        /// <returns>Success?</returns>
        public bool ReadAll(Type type, out IEnumerable<SQLiteObject> list, string suffix = "", params SqliteParameter[] sqlParams)
        {
            return ReadAll(type.Name.ToLower(), type, out list, suffix, sqlParams);
        }

        /// <summary>
        /// Execute a read command (for a list of objects)
        /// Assign the SQLiteProperty to each Property of the object that needs to be read from the Database
        /// Make sure there is a constructor that matches the number of Properties and their associated types
        /// </summary>
        /// <param name="tableName">Table name to read from</param>
        /// <param name="type">Type of object to read</param>
        /// <param name="list">Outputs a list of object</param>
        /// <param name="suffix">Suffix to add to command after the WHERE statement (i.e. GROUP BY)</param>
        /// <param name="sqlParams">Parameters to the command</param>
        /// <returns>Success?</returns>
        public bool ReadAll(string tableName, Type type, out IEnumerable<SQLiteObject> list, string suffix = "", params SqliteParameter[] sqlParams)
        {
            List<SQLiteObject> tempList = new List<SQLiteObject>();
            list = new List<SQLiteObject>();
            bool success = false;

            IDbCommand command = null;
            try
            {
                OpenConnection();

                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM " + ((_addTrailingS) ? tableName.InsertEndsWith("s") : tableName).ToLower();
                command.AddParameters(true, sqlParams);
                command.CommandText += $" {suffix}";

                using IDataReader reader = command.ExecuteReader();

                List<object> args = new List<object>();
                PropertyInfo[] fields = type.GetProperties().Where(x => Attribute.IsDefined(x, typeof(SQLitePropertyAttribute))).ToArray();

                while (reader.Read())
                {
                    args = new List<object>();
                    foreach (PropertyInfo property in fields)
                    {
                        int index = reader.GetOrdinal(property.Name);
                        if (index == -1)
                        {
                            Debug.LogError($"Property ({property.Name} from {type}) not found within table columns. ");
                            continue;
                        }
                        object arg = SQLiteHelper.ConvertValue(reader.GetValue(index), property.PropertyType);
                        args.Add(arg);
                    }

                    // Try to create an instance of object T
                    if (!TryCreateInstance(type, args, out SQLiteObject obj))
                        continue;

                    tempList.Add(obj);
                }

                success = true;
                list = tempList;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to read from the database. Command: {command.CommandText} \n{ex}");
            }
            finally
            {
                CloseConnection();
            }

            return success;
        }

        #endregion

        /// <summary>
        /// Execute a count of the table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="sqlParams">Sqlite Parameters</param>
        /// <returns>Total row count in table</returns>
        public int Count(string tableName, params SqliteParameter[] sqlParams)
        {
            int count = 0;
            try
            {
                OpenConnection();

                IDbCommand command = _connection.CreateCommand();
                command.CommandText = $"SELECT COUNT(*) FROM {tableName.ToLower()}";
                command.AddParameters(true, sqlParams);

                count = (int) SQLiteHelper.ConvertValue(command.ExecuteScalar(), typeof(int));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed in {nameof(Count)}, unknown error occured: {ex}.");
            }
            finally
            {
                CloseConnection();
            }

            return count;
        }

        #endregion

        #endregion

#if UNITY_EDITOR

        #region SQLite Tools methods

        /// <summary>
        /// Create a new blueprint to rebuild the whole SQL database. <br/>
        /// Specify <paramref name="path"/> to create a file.
        /// </summary>
        /// <param name="path">Path to create .sql file (specify its name as well)</param>
        /// <param name="settings">SQLite settings scriptable object</param>
        internal void CreateBlueprint(string path, SQLiteSettings settings)
        {
            if (Application.isPlaying) return;

            // Initialize service 
            if (_connection == null)
            {
                OnInitialized(settings);
            }

            //if (!string.IsNullOrEmpty(path) && !path.EndsWith(".sql"))
            //{
            //    Debug.LogError($"Failed at {nameof(CreateBlueprint)}, path is empty or does not end with \".sql\".");
            //    return;
            //}

            List<string> tableNames = new List<string>();
            StringBuilder blueprint = new StringBuilder();
            bool success = false;

            try
            {
                OpenConnection();

                IDbCommand command = _connection.CreateCommand();
                command.CommandText = "SELECT name, sql FROM sqlite_master WHERE type='table'";

                // Create table sql statements
                // Iterate through all the table names and its create sqls
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);

                        // This is an sqlite generated table and shouldn't be added to the blueprint
                        if (name.StartsWith("sqlite_")) continue;

                        tableNames.Add(name);

                        // Drop table if exists command
                        blueprint.Append($"DROP TABLE IF EXISTS {name};\n");

                        // Create table command
                        blueprint.Append($"{reader.GetString(1)};\n\n");
                    }
                }

                // Insert sql statements
                // Iterate through all the tables
                foreach (string name in tableNames)
                {
                    // Get row count
                    int rowCount = Count(name, hasConnection: true);

                    // Check if there's any row to insert
                    if (rowCount == 0) continue;

                    // Read all rows from table
                    command.CommandText = $"SELECT * FROM {name}";
                    using IDataReader reader = command.ExecuteReader();

                    // Insert SQL statement
                    StringBuilder insertSQL = new StringBuilder($"INSERT INTO {name} VALUES");
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
                }

                success = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed in {nameof(CreateBlueprint)}, unknown error occured, exception: {ex}.");
            }
            finally
            {
                CloseConnection();

                // Garbage collect because Close() is not releasing the db file
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            if (success)
            {
                // Write the new blueprint to the path
                File.WriteAllText(path, blueprint.ToString());

                Debug.Log($"Blueprint successfully created at: {path}");
            }
        }

        /// <summary>
        /// Execute an sql script from path (If the path is a directory, it will execute all the subdirectories and files within it)
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="includeSubfolders">Whether it should include subfolders</param>
        /// <returns></returns>
        internal void ExecuteFromPath(string path, SQLiteSettings settings, bool includeSubfolders = true)
        {
            // Initialize service 
            if (_connection == null)
            {
                OnInitialized(settings);
            }

            Debug.Log($"Executing files from folder path: {path}");

            // Directory
            if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                foreach (string p in Directory.EnumerateFileSystemEntries(path, $"*{FileExtensionConstants.cSQLite}"))
                {
                    if (File.GetAttributes(p).HasFlag(FileAttributes.Directory))
                    {
                        if (!includeSubfolders) continue;

                        ExecuteFromPath(path, settings, includeSubfolders);
                    }
                    else
                    {
                        Debug.Log($"Executing .sql file: {p}");

                        string script = File.ReadAllText(p);
                        Execute(script);
                    }
                }
            }
            // File
            else
            {
                Debug.Log($"Executing .sql file: {path}");

                string script = File.ReadAllText(path);
                Execute(script);
            }
        }

        /// <summary>
        /// Execute a count of the table (Unity Editor only)
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="hasConnection">Whether <see cref="_connection"/> is already opened (if false then it will open and close the connection)</param>
        /// <param name="sqlParams">Sqlite paramters</param>
        /// <returns>Total row count in table</returns>
        internal int Count(string tableName, bool hasConnection, params SqliteParameter[] sqlParams)
        {
            int count = 0;
            try
            {
                if (!hasConnection)
                {
                    OpenConnection();
                }

                IDbCommand command = _connection.CreateCommand();
                command.CommandText = $"SELECT COUNT(*) FROM {tableName.ToLower()}";
                command.AddParameters(true, sqlParams);

                count = (int) SQLiteHelper.ConvertValue(command.ExecuteScalar(), typeof(int));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed in {nameof(Count)}, unknown error occured: {ex}.");
            }
            finally
            {
                if (!hasConnection)
                {
                    CloseConnection();
                }
            }

            return count;
        }

        #endregion

#endif

        #region Util methods

        /// <summary>
        /// Try to create an instance of object type <paramref name="type"/>
        /// </summary>
        /// <param name="args">List of object arguments associated to constructor</param>
        /// <param name="type">Type of object to create that derives from SQLiteObject</param>
        /// <param name="obj">Output object</param>
        /// <returns>Success?</returns>
        private bool TryCreateInstance(Type type, List<object> args, out SQLiteObject obj)
        {
            obj = default;
            if (!typeof(SQLiteObject).IsAssignableFrom(type) || type == typeof(SQLiteObject))
            {
                Debug.LogError($"Failed in {nameof(TryCreateInstance)}, type is {typeof(SQLiteObject)} or does not derive from it.");
                return false;
            }

            try
            {
                obj = (SQLiteObject) Activator.CreateInstance(type, args.ToArray());
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

                Debug.LogError($"Failed to read from the database. No valid constructor was found for {type} with {args.Count} arguments ({argsStr})");
                Debug.LogError($"Exception: {ex}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed in {nameof(TryCreateInstance)}, unexpected error occurred: {ex}");
            }

            return false;
        }

        /// <summary>
        /// Open a new SQLite connection if it's not already opened
        /// </summary>
        private void OpenConnection()
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed in {nameof(OpenConnection)}, unknown error occured, exception: {ex}.");
            }
        }

        /// <summary>
        /// Close existing SQLite connection if it's opened
        /// </summary>
        private void CloseConnection()
        {
            try
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed in {nameof(CloseConnection)}, unknown error occured, exception: {ex}.");
            }
        }

        #endregion
    }
}
