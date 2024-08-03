using Cyggie.Plugins.Logs;
using Cyggie.Plugins.MySQL.Abstract;
using Cyggie.Plugins.MySQL.Attributes;
using Cyggie.Plugins.MySQL.Utils.Helpers;
using Cyggie.Plugins.Services.Interfaces;
using Cyggie.Plugins.Services.Models;
using Cyggie.Plugins.Utils.Helpers;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cyggie.Plugins.MySQL.Services
{
    /// <summary>
    /// Service for anything related to MySQL
    /// </summary>
    public class MySQLService : Service
    {
        /// <summary>
        /// Pool created from <see cref="MySQLTablePreloadAttribute"/>s only
        /// </summary>
        private readonly Dictionary<Type, IEnumerable<MySQLTableObject>> _pool = new Dictionary<Type, IEnumerable<MySQLTableObject>>();

        private MySqlConnection? _conn = null;

        /// <summary>
        /// Action called when a databse connection is established
        /// </summary>
        public Action? OnConnected;

        /// <summary>
        /// Whether a connection is established and ready for queries
        /// </summary>
        public bool IsReady => _conn != null && _conn.State == ConnectionState.Open;

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            IEnumerable<Type> sqlObjectTypes = TypeHelper.GetAllIsAssignableFrom<MySQLTableObject>()
                                                     .Where(x => x.GetCustomAttribute<MySQLTableNameAttribute>() != null);

            foreach (Type type in sqlObjectTypes)
            {
                MySQLTableNameAttribute tableNameAttr = type.GetCustomAttribute<MySQLTableNameAttribute>();
                MySQLTableHelper.AddMapping(type, tableNameAttr.TableName);
            }
        }

        #region Public API

        /// <summary>
        /// Connect to the database constructing the connection string using the provided arguments
        /// </summary>
        /// <param name="server">Server address</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="database">Database to connect to</param>
        public void Connect(string server, string username, string password, string database)
        {
            Connect(new MySqlConnectionStringBuilder()
            {
                Server = server,
                UserID = username,
                Password = password,
                Database = database
            });
        }

        /// <summary>
        /// Connect to the database using the provided string builder <paramref name="builder"/>
        /// </summary>
        /// <param name="builder">Connection string builder</param>
        public void Connect(MySqlConnectionStringBuilder builder)
        {
            _conn = new MySqlConnection(builder.ConnectionString);
            _conn.StateChange += OnConnectionStatChange;

            try
            {
                _conn.Open();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to open MySQL connection to {builder.ConnectionString}, exception: {ex}.", nameof(MySQLService));
            }
        }

        /// <summary>
        /// Disconnect from the currently connected database
        /// </summary>
        public void Disconnect()
        {
            if (_conn == null)
            {
                Log.Error($"Failed to disconnect, connection is null.", nameof(MySQLService));
                return;
            }

            _conn.Close();
            _conn = null;
        }

        /// <summary>
        /// Create a MySQL Command with <paramref name="parameters"/>
        /// </summary>
        /// <param name="commandText">The command's query text</param>
        /// <param name="parameters">Parameters to be added to the command</param>
        /// <returns>MySqlCommand object (null if connection is not established)</returns>
        public MySqlCommand? CreateCommand(string commandText = "", params MySqlParameter[] parameters)
        {
            if (!ValidateConnection() || _conn == null) return null;

            MySqlCommand command = _conn.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(parameters);

            return command;
        }

        /// <summary>
        /// Execute an sql query (such as UPDATE, INSERT, DELETE, etc.)
        /// </summary>
        /// <param name="commandText">The command's query text</param>
        /// <param name="parameters">Parameters to be added to the command</param>
        /// <returns>Number of rows affected</returns>
        public int Execute(string commandText, params MySqlParameter[] parameters)
        {
            MySqlCommand? command = CreateCommand(commandText, parameters);
            if (command == null) return 0;

            return Execute(command);
        }

        /// <summary>
        /// Execute a <paramref name="command"/> (such as UPDATE, INSERT, DELETE, etc.)
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns>Number of rows affected</returns>
        public int Execute(MySqlCommand command)
        {
            if (!ValidateConnection()) return 0;

            int rowsAffected = 0;
            try
            {
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to execute command: {command.CommandText}, exception: {ex}.", nameof(MySQLService));
            }
            finally
            {
                command.DisposeAsync();
            }

            return rowsAffected;
        }

        #region Read

        /// <summary>
        /// Read the first element from the objects found in the pool
        /// </summary>
        /// <typeparam name="T">Type of objects to retrieve</typeparam>
        /// <param name="predicate">Predicate applied to the read</param>
        /// <returns>First element of query</returns>
        public T ReadFirst<T>(Func<T, bool>? predicate = null) where T : MySQLTableObject
            => predicate == null ? Read<T>().FirstOrDefault() : Read<T>().FirstOrDefault(predicate);

        /// <summary>
        /// Read the first element from the objects found
        /// </summary>
        /// <typeparam name="T">Type of objects to retrieve</typeparam>
        /// <param name="commandText">The command's query text</param>
        /// <param name="predicate">Predicate applied to the read</param>
        /// <param name="parameters">Parameters to be added to the command</param>
        /// <returns>First element of query</returns>
        public T ReadFirst<T>(string commandText, Func<T, bool>? predicate = null, params MySqlParameter[] parameters) where T : MySQLTableObject
            => predicate == null ? Read<T>(commandText, parameters).FirstOrDefault() : Read<T>(commandText, parameters).FirstOrDefault(predicate);

        /// <summary>
        /// Read the first element from the objects found
        /// </summary>
        /// <typeparam name="T">Type of objects to retrieve</typeparam>
        /// <param name="command">Command to execute a read (SELECT)</param>
        /// <param name="predicate">Predicate applied to the read</param>
        /// <returns>First element of query</returns>
        public T? ReadFirst<T>(MySqlCommand command, Func<T, bool>? predicate = null) where T : MySQLTableObject
            => predicate == null ? Read<T>(command).FirstOrDefault() : Read<T>(command).FirstOrDefault(predicate);

        /// <summary>
        /// Read from the pool all objects of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of objects to retrieve</typeparam>
        /// <returns>IEnumerable of objects</returns>
        public IEnumerable<T> Read<T>() where T : MySQLTableObject
        {
            return Read(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Read a table with an sql query 
        /// </summary>
        /// <typeparam name="T">Object type to read</typeparam>
        /// <param name="commandText">The command's query text</param>
        /// <param name="parameters">Parameters to be added to the command</param>
        /// <returns>IEnumerable of objects</returns>
        public IEnumerable<T> Read<T>(string commandText, params MySqlParameter[] parameters) where T : MySQLTableObject
        {
            MySqlCommand? command = CreateCommand(commandText, parameters);
            if (command == null) return Array.Empty<T>();

            return Read<T>(command);
        }

        /// <summary>
        /// Read a table with <paramref name="command"/>
        /// </summary>
        /// <typeparam name="T">Object type to read</typeparam>
        /// <param name="command">Command to execute a read (SELECT)</param>
        /// <returns>IEnumerable of object of type <typeparamref name="T"/></returns>
        public IEnumerable<T> Read<T>(MySqlCommand command) where T : MySQLTableObject
        {
            return Read(typeof(T), command).Cast<T>();
        }

        /// <summary>
        /// Read the first element from the objects found in the pool
        /// </summary>
        /// <param name="type">Type of objects to retrieve</param>
        /// <returns>First element of query</returns>
        public MySQLTableObject? ReadFirst(Type type) 
            => Read(type).FirstOrDefault();

        /// <summary>
        /// Read the first element from the objects found
        /// </summary>
        /// <param name="type">Type of objects to retrieve</param>
        /// <param name="commandText">The command's query text</param>
        /// <param name="parameters">Parameters to be added to the command</param>
        /// <returns>First element of query</returns>
        public MySQLTableObject? ReadFirst(Type type, string commandText, params MySqlParameter[] parameters)
            => Read(type, commandText, parameters).FirstOrDefault();

        /// <summary>
        /// Read the first element from the objects found
        /// </summary>
        /// <param name="type">Type of objects to retrieve</param>
        /// <param name="command">Command to execute a read (SELECT)</param>
        /// <returns>First element of query</returns>
        public MySQLTableObject? ReadFirst(Type type, MySqlCommand command)
            => Read(type, command).FirstOrDefault();

        /// <summary>
        /// Read from the pool all objects of type <paramref name="type"/>
        /// </summary>
        /// <param name="type">Type of objects to retrieve</param>
        /// <returns>IEnumerable of objects</returns>
        public IEnumerable<MySQLTableObject> Read(Type type)
        {
            // Check if pool contains type already (preloaded types)
            if (_pool.ContainsKey(type))
            {
                return _pool[type];
            }

            Log.Error($"Failed to read, type {type} is not pooled. Use the [MySQLTablePreload] attribute or add a command text to the Read method.", nameof(MySQLService));
            return Array.Empty<MySQLTableObject>();
        }

        /// <summary>
        /// Read a table with an sql query 
        /// </summary>
        /// <param name="type">Object type to read (must inherit from <see cref="MySQLTableObject"/> and not be abstract</param>
        /// <param name="commandText">The command's query text</param>
        /// <param name="parameters">Parameters to be added to the command</param>
        /// <returns>IEnumerable of objects</returns>
        public IEnumerable<MySQLTableObject> Read(Type type, string commandText, params MySqlParameter[] parameters)
        {
            MySqlCommand? command = CreateCommand(commandText, parameters);
            if (command == null) return Array.Empty<MySQLTableObject>();

            return Read(type, command);
        }

        /// <summary>
        /// Read a table with <paramref name="command"/>
        /// </summary>
        /// <param name="type">Object type to read (must inherit from <see cref="MySQLTableObject"/> and not be abstract</param>
        /// <param name="command">Command to execute a read (SELECT)</param>
        /// <returns>IEnumerable of object of <paramref name="type"/></returns>
        public IEnumerable<MySQLTableObject> Read(Type type, MySqlCommand command)
        {
            if (!ValidateConnection()) return Array.Empty<MySQLTableObject>();

            if (!typeof(MySQLTableObject).IsAssignableFrom(type))
            {
                Log.Error($"Failed to read from type {type}, {typeof(MySQLTableObject)} is not assignable from {type}.", nameof(MySQLService));
                return Array.Empty<MySQLTableObject>();
            }

            if (type.IsAbstract)
            {
                Log.Error($"Failed to read from type {type}, it is abstract.", nameof(MySQLService));
                return Array.Empty<MySQLTableObject>();
            }

            List<MySQLTableObject> objects = new List<MySQLTableObject>();
            MySqlDataReader? reader = null;

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (!TryCreateTableObject(reader, type, out object? tableObj) || tableObj == null) continue;

                    objects.Add((MySQLTableObject) tableObj);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to execute read command. Exception: {ex}", nameof(MySQLService));
            }
            finally
            {
                reader?.DisposeAsync();
            }

            return objects;
        }

        #endregion

        #endregion

        #region Public Async API

        /// <summary>
        /// Connect asynchronously to the database constructing the connection string using the provided arguments
        /// </summary>
        /// <param name="server">Server address</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="database">Database to connect to</param>
        public async Task ConnectAsync(string server, string username, string password, string database)
        {
            await ConnectAsync(new MySqlConnectionStringBuilder()
            {
                Server = server,
                UserID = username,
                Password = password,
                Database = database
            });
        }

        /// <summary>
        /// Connect asynchronously to the database using the provided string builder <paramref name="builder"/>
        /// </summary>
        /// <param name="builder">Connection string builder</param>
        public async Task ConnectAsync(MySqlConnectionStringBuilder builder)
        {
            _conn = new MySqlConnection(builder.ConnectionString);
            _conn.StateChange += OnConnectionStatChange;
            await _conn.OpenAsync();
        }

        /// <summary>
        /// Disconnect asynchronously from the currently connected database
        /// </summary>
        public void DisconnectAsync()
        {
            if (_conn == null)
            {
                Log.Error($"Failed to disconnect, connection is null.", nameof(MySQLService));
                return;
            }

            _pool.Clear();
            _conn.CloseAsync();
            _conn = null;
        }

        /// <summary>
        /// Execute asynchronously an sql query (such as UPDATE, INSERT, DELETE, etc.)
        /// </summary>
        /// <param name="commandText">The command's query text</param>
        /// <param name="parameters">Parameters to be added to the command</param>
        /// <returns>Number of rows affected</returns>
        public async Task<int> ExecuteAsync(string commandText, params MySqlParameter[] parameters)
        {
            MySqlCommand? command = CreateCommand(commandText, parameters);
            if (command == null) return 0;

            return await ExecuteAsync(command);
        }

        /// <summary>
        /// Execute asynchronously a <paramref name="command"/> (such as UPDATE, INSERT, DELETE, etc.)
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns>Number of rows affected</returns>
        public async Task<int> ExecuteAsync(MySqlCommand command)
        {
            if (!ValidateConnection()) return 0;

            int rowsAffected = 0;
            try
            {
                rowsAffected = await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to execute command: {command.CommandText}, exception: {ex}.", nameof(MySQLService));
            }
            finally
            {
                await command.DisposeAsync();
            }

            return rowsAffected;
        }

        #region Read

        /// <summary>
        /// Read the first element from the objects found
        /// </summary>
        /// <typeparam name="T">Type of objects to retrieve</typeparam>
        /// <param name="commandText">The command's query text</param>
        /// <param name="predicate">Predicate applied to the read</param>
        /// <param name="parameters">Parameters to be added to the command</param>
        /// <returns>First element of query</returns>
        public async Task<T> ReadFirstAsync<T>(string commandText, Func<T, bool>? predicate = null, params MySqlParameter[] parameters) where T : MySQLTableObject
            => predicate == null ? (await ReadAsync<T>(commandText, parameters)).FirstOrDefault() : (await ReadAsync<T>(commandText, parameters)).FirstOrDefault(predicate);

        /// <summary>
        /// Read the first element from the objects found
        /// </summary>
        /// <typeparam name="T">Type of objects to retrieve</typeparam>
        /// <param name="command">Command to execute a read (SELECT)</param>
        /// <param name="predicate">Predicate applied to the read</param>
        /// <returns>First element of query</returns>
        public async Task<T?> ReadFirstAsync<T>(MySqlCommand command, Func<T, bool>? predicate = null) where T : MySQLTableObject
            => predicate == null ? (await ReadAsync<T>(command)).FirstOrDefault() : (await ReadAsync<T>(command)).FirstOrDefault(predicate);

        /// <summary>
        /// Read asynchronously a table with an sql query 
        /// </summary>
        /// <typeparam name="T">Object type to read</typeparam>
        /// <param name="commandText">The command's query text</param>
        /// <param name="parameters">Parameters to be added to the command</param>
        /// <returns>IEnumerable of objects</returns>
        public async Task<IEnumerable<T>> ReadAsync<T>(string commandText, params MySqlParameter[] parameters) where T : MySQLTableObject
        {
            MySqlCommand? command = CreateCommand(commandText, parameters);
            if (command == null) return Array.Empty<T>();

            return await ReadAsync<T>(command);
        }

        /// <summary>
        /// Read asynchronously a table with <paramref name="command"/>
        /// </summary>
        /// <typeparam name="T">Object type to read</typeparam>
        /// <param name="command">Command to execute a read (SELECT)</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ReadAsync<T>(MySqlCommand command) where T : MySQLTableObject
        {
            IEnumerable<MySQLTableObject> tableObjects = await ReadAsync(typeof(T), command);
            return tableObjects.Cast<T>();
        }

        /// <summary>
        /// Read the first element from the objects found
        /// </summary>
        /// <param name="type">Type of objects to retrieve</param>
        /// <param name="commandText">The command's query text</param>
        /// <param name="parameters">Parameters to be added to the command</param>
        /// <returns>First element of query</returns>
        public async Task<MySQLTableObject?> ReadFirstAsync(Type type, string commandText, params MySqlParameter[] parameters)
            => (await ReadAsync(type, commandText, parameters)).FirstOrDefault();

        /// <summary>
        /// Read the first element from the objects found
        /// </summary>
        /// <param name="type">Type of objects to retrieve</param>
        /// <param name="command">Command to execute a read (SELECT)</param>
        /// <returns>First element of query</returns>
        public async Task<MySQLTableObject?> ReadFirstAsync(Type type, MySqlCommand command)
            => (await ReadAsync(type, command)).FirstOrDefault();

        /// <summary>
        /// Read asynchronously a table with an sql query 
        /// </summary>
        /// <param name="type">Object type to read (must inherit from <see cref="MySQLTableObject"/> and not be abstract</param>
        /// <param name="commandText">The command's query text</param>
        /// <param name="parameters">Parameters to be added to the command</param>
        /// <returns>IEnumerable of objects</returns>
        public async Task<IEnumerable<MySQLTableObject>> ReadAsync(Type type, string commandText, params MySqlParameter[] parameters)
        {
            MySqlCommand? command = CreateCommand(commandText, parameters);
            if (command == null) return Array.Empty<MySQLTableObject>();

            return await ReadAsync(type, command);
        }

        /// <summary>
        /// Read asynchronously a table with <paramref name="command"/>
        /// </summary>
        /// <param name="type">Object type to read (must inherit from <see cref="MySQLTableObject"/> and not be abstract</param>
        /// <param name="command">Command to execute a read (SELECT)</param>
        /// <returns>IEnumerable of object of <paramref name="type"/></returns>
        public async Task<IEnumerable<MySQLTableObject>> ReadAsync(Type type, MySqlCommand command)
        {
            if (!ValidateConnection()) return Array.Empty<MySQLTableObject>();

            if (!typeof(MySQLTableObject).IsAssignableFrom(type))
            {
                Log.Error($"Failed to read from type {type}, {typeof(MySQLTableObject)} is not assignable from {type}.", nameof(MySQLService));
                return Array.Empty<MySQLTableObject>();
            }

            if (type.IsAbstract)
            {
                Log.Error($"Failed to read from type {type}, it is abstract.", nameof(MySQLService));
                return Array.Empty<MySQLTableObject>();
            }

            List<MySQLTableObject> objects = new List<MySQLTableObject>();
            MySqlDataReader? reader = null;

            try
            {
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (!TryCreateTableObject(reader, type, out object? tableObj) || tableObj == null) continue;

                    objects.Add((MySQLTableObject) tableObj);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to execute read command. Exception: {ex}", nameof(MySQLService));
            }
            finally
            {
                if (reader != null)
                {
                    await reader.DisposeAsync();
                }
            }

            return objects;
        }

        #endregion

        #endregion

        #region Utils

        private bool ValidateConnection()
        {
            if (!IsReady)
            {
                Log.Error($"Failed to validate connection, database connection is not ready.", nameof(MySQLService));
                return false;
            }

            return true;
        }

        private bool TryCreateTableObject(MySqlDataReader reader, Type type, out object? tableObj)
        {
            int count = reader.FieldCount;
            Dictionary<string, object> values = new Dictionary<string, object>();
            for (int i = 0; i < count; i++)
            {
                values.Add(reader.GetName(i), reader.GetValue(i));
            }

            StringBuilder builder = new StringBuilder();

            // Construct JSON
            builder.Append("{");
            foreach (KeyValuePair<string, object> kv in values)
            {
                string value = kv.Value is string ? $"\"{((string) kv.Value).Replace("\"", "\\\"")}\"" : $"{kv.Value}";
                builder.Append($"\"{kv.Key}\": {value},");
            }

            // Remove the last comma
            builder.Remove(builder.Length - 1, 1);
            builder.Append("}");

            tableObj = JsonConvert.DeserializeObject(builder.ToString(), type);
            if (tableObj == null)
            {
                Log.Error($"Unable to deserialize json into object of type {type}. JSON: {builder}", nameof(MySQLService));
                return false;
            }

            ((MySQLTableObject) tableObj).OnObjectCreated_Internal();
            return true;
        }

        #endregion

        /// <inheritdoc/>
        public override void Dispose()
        {
            if (_conn != null)
            {
                _conn.StateChange += OnConnectionStatChange;
                _conn = null;
            }
        }

        private void OnConnectionStatChange(object sender, StateChangeEventArgs args)
        {
            if (_conn == null) return;

            if (args.CurrentState == ConnectionState.Open)
            {
                string databaseName = _conn.Database;

                List<Type> preloadSQLObjectTypes = TypeHelper.GetAllIsAssignableFrom<MySQLTableObject>()
                                                             .Where(x =>
                                                                {
                                                                    MySQLTablePreloadAttribute preloadAttr = x.GetCustomAttribute<MySQLTablePreloadAttribute>();
                                                                    if (preloadAttr == null) return false;

                                                                    bool rightDb = string.IsNullOrEmpty(preloadAttr.DatabaseName) ||
                                                                                   preloadAttr.DatabaseName.Equals(databaseName, StringComparison.OrdinalIgnoreCase);
                                                                    if (!rightDb) return false;

                                                                    return true;
                                                                })
                                                             .OrderByDescending(x =>
                                                                {
                                                                    MySQLTablePreloadAttribute preloadAttr = x.GetCustomAttribute<MySQLTablePreloadAttribute>();
                                                                    return preloadAttr.Priority;
                                                                })
                                                             .ToList();

                foreach (Type type in preloadSQLObjectTypes)
                {
                    IEnumerable<MySQLTableObject> objects = Read(type, $"SELECT * FROM {MySQLTableHelper.GetTableName(type)}");

                    _pool.Add(type, objects);
                }

                Log.Debug($"Preloaded {_pool.Count} MySQL table(s)", nameof(MySQLService));
                OnConnected?.Invoke();
            }
        }
    }
}
