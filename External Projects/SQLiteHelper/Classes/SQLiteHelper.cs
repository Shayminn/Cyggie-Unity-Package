using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Cyggie.SQLite
{
    /// <summary>
    /// Helper class for managing/handling SQLite
    /// </summary>
    public static class SQLiteHelper
    {
        private static string databasePath = ""; // Set this according to your database location for your project

        private static string ConnectionString => $"Data Source={databasePath};Version=3;";

        /// <summary>
        /// Execute non query command
        /// </summary>
        /// <param name="query">Command string</param>
        /// <param name="sqlParams">Parameters to the command (The key must be found within the query string)</param>
        /// <returns>Number of rows affected</returns>
        public static int Execute(string query, params SQLiteParams[] sqlParams)
        {
            using SQLiteConnection conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            using SQLiteCommand command = conn.CreateCommand();
            command.CommandText = query;
            command.AddParameters(false, sqlParams);

            int result = 0;
            try
            {
                result = command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                string paramss = "";
                foreach (SQLiteParams sqlParam in sqlParams)
                {
                    if (!string.IsNullOrEmpty(paramss))
                        paramss += ", ";

                    paramss += $"[{sqlParam.ParameterKey}, {sqlParam.ParameterValue}]";
                }

                WriteLine(MessageCode.Error, $"SQLite exception when executing Query: {query} (Parameters: {paramss})\nException: {ex}");
            }

            conn.Close();
            return result;
        }

        #region Read methods

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
        public static bool Read<T>(out T obj, string suffix = "", params SQLiteParams[] sqlParams)
        {
            obj = default;
            bool success = false;

            if (!HasSubclass(typeof(T)))
                return false;

            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();

                using SQLiteCommand command = conn.CreateCommand();

                string tableName = typeof(T).Name.ToLower();
                command.CommandText = $"SELECT * FROM {tableName}" + (tableName.EndsWith('s') ? "" : "s"); // add an s to the end if not already there
                command.AddParameters(true, sqlParams);
                command.CommandText += $" {suffix}";

                try
                {
                    using SQLiteDataReader reader = command.ExecuteReader();

                    List<object> args = new List<object>();
                    PropertyInfo[] fields = typeof(T).GetProperties().Where(x => Attribute.IsDefined(x, typeof(SQLitePropertyAttribute))).ToArray();

                    if (reader.Read())
                    {
                        foreach (PropertyInfo property in fields)
                        {
                            int index = reader.GetOrdinal(property.Name);
                            if (index == -1)
                            {
                                WriteLine(MessageCode.Warning, $"Property ({property.Name} from {typeof(T)}) not found within table columns. ");
                                continue;
                            }
                            object arg = SQLiteUtils.ConvertValue(reader.GetValue(index), property.PropertyType);
                            args.Add(arg);
                        }

                        // Try to create an instance of object T
                        success = TryCreateInstance(args, out obj);
                    }
                }
                catch (Exception ex)
                {
                    WriteLine(MessageCode.Error, $"Failed to read from the database. Command: {command.CommandText} \n{ex}");
                }

                conn.Close();
            }

            return success;
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
        public static bool Read<T>(string tableName, out T obj, string suffix = "", params SQLiteParams[] sqlParams)
        {
            obj = default;
            bool success = false;

            if (!HasSubclass(typeof(T)))
                return false;

            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();

                using SQLiteCommand command = conn.CreateCommand();
                command.CommandText = $"SELECT * FROM {tableName.ToLower()}";
                command.AddParameters(true, sqlParams);
                command.CommandText += $" {suffix}";

                try
                {
                    using SQLiteDataReader reader = command.ExecuteReader();

                    List<object> args = new List<object>();
                    PropertyInfo[] fields = typeof(T).GetProperties().Where(x => Attribute.IsDefined(x, typeof(SQLitePropertyAttribute))).ToArray();

                    if (reader.Read())
                    {
                        foreach (PropertyInfo property in fields)
                        {
                            int index = reader.GetOrdinal(property.Name);
                            if (index == -1)
                            {
                                WriteLine(MessageCode.Warning, $"Property ({property.Name} from {typeof(T)}) not found within table columns. ");
                                continue;
                            }
                            object arg = SQLiteUtils.ConvertValue(reader.GetValue(index), property.PropertyType);
                            args.Add(arg);
                        }

                        // Try to create an instance of object T
                        success = TryCreateInstance(args, out obj);
                    }
                }
                catch (Exception ex)
                {
                    WriteLine(MessageCode.Error, $"Failed to read from the database. Command: {command.CommandText} \n{ex}");
                }

                conn.Close();
            }

            return success;
        }

        /// <summary>
        /// Execute a read command (for a list of objects)
        /// Assign the SQLiteProperty to each Property of the object that needs to be read from the Database
        /// Make sure there is a constructor that matches the number of Properties and their associated types
        /// </summary>
        /// <typeparam name="T">Object class</typeparam>
        /// <param name="list">Outputs a list of object</param>
        /// <param name="suffix">Suffix to add to command after the WHERE statement (i.e. GROUP BY)</param>
        /// <param name="sqlParams">Parameters to the command</param>
        /// <returns>Success?</returns>
        public static bool ReadAll<T>(out IEnumerable<T> list, string suffix = "", params SQLiteParams[] sqlParams)
        {
            List<T> tempList = new List<T>();
            list = new List<T>();
            bool success = false;

            if (!HasSubclass(typeof(T)))
                return false;

            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                
                using SQLiteCommand command = conn.CreateCommand();

                string tableName = typeof(T).Name.ToLower();
                command.CommandText = $"SELECT * FROM {tableName}" + (tableName.EndsWith('s') ? "" : "s"); // add an s to the end if not already there
                command.AddParameters(true, sqlParams);
                command.CommandText += $" {suffix}";

                try
                {
                    using SQLiteDataReader reader = command.ExecuteReader();

                    List<object> args = new List<object>();
                    PropertyInfo[] fields = typeof(T).GetProperties().Where(x => Attribute.IsDefined(x, typeof(SQLitePropertyAttribute))).ToArray();
                    while (reader.Read())
                    {
                        args = new List<object>();
                        foreach (PropertyInfo property in fields)
                        {
                            int index = reader.GetOrdinal(property.Name);
                            if (index == -1)
                            {
                                WriteLine(MessageCode.Warning, $"Property ({property.Name} from {typeof(T)}) not found within table columns. ");
                                continue;
                            }
                            object arg = SQLiteUtils.ConvertValue(reader.GetValue(index), property.PropertyType);
                            args.Add(arg);
                        }

                        // Try to create an instance of object T
                        if (!TryCreateInstance(args, out T obj))
                            break;

                        tempList.Add(obj);
                    }

                    success = true;
                    list = tempList;
                }
                catch (Exception ex)
                {
                    WriteLine(MessageCode.Error, $"Failed to read from the database. Command: {command.CommandText} \n{ex}");
                }

                conn.Close();
            }

            return success;
        }

        /// <summary>
        /// Execute a read command (for a list of objects)
        /// Assign the SQLiteProperty to each Property of the object that needs to be read from the Database
        /// Make sure there is a constructor that matches the number of Properties and their associated types
        /// </summary>
        /// <typeparam name="T">Object class</typeparam>
        /// <param name="tableName">Table name to read from</param>
        /// <param name="list">Outputs a list of object</param>
        /// <param name="suffix">Suffix to add to command after the WHERE statement (i.e. GROUP BY)</param>
        /// <param name="sqlParams">Parameters to the command</param>
        /// <returns>Success?</returns>
        public static bool ReadAll<T>(string tableName, out IEnumerable<T> list, string suffix = "", params SQLiteParams[] sqlParams)
        {
            List<T> tempList = new List<T>();
            list = new List<T>();
            bool success = false;

            if (!HasSubclass(typeof(T)))
                return false;

            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
               
                using SQLiteCommand command = conn.CreateCommand();
                command.CommandText = $"SELECT * FROM {tableName.ToLower()}";
                command.AddParameters(true, sqlParams);
                command.CommandText += $" {suffix}";

                try
                {
                    using SQLiteDataReader reader = command.ExecuteReader();

                    List<object> args = new List<object>();
                    PropertyInfo[] fields = typeof(T).GetProperties().Where(x => Attribute.IsDefined(x, typeof(SQLitePropertyAttribute))).ToArray();

                    while (reader.Read())
                    {
                        args = new List<object>();
                        foreach (PropertyInfo property in fields)
                        {
                            int index = reader.GetOrdinal(property.Name);
                            if (index == -1)
                            {
                                WriteLine(MessageCode.Warning, $"Property ({property.Name} from {typeof(T)}) not found within table columns. ");
                                continue;
                            }
                            object arg = SQLiteUtils.ConvertValue(reader.GetValue(index), property.PropertyType);
                            args.Add(arg);
                        }

                        // Try to create an instance of object T
                        if (!TryCreateInstance(args, out T obj))
                            break;

                        tempList.Add(obj);
                    }

                    success = true;
                    list = tempList;
                }
                catch (Exception ex)
                {
                    WriteLine(MessageCode.Error, $"Failed to read from the database. Command: {command.CommandText} \n{ex}");
                }

                conn.Close();
            }

            return success;
        }

        /// <summary>
        /// Execute a count of the table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="count"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        public static bool Count(string tableName, out int count, params SQLiteParams[] sqlParams)
        {
            bool success = false;

            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                
                using SQLiteCommand command = conn.CreateCommand();
                command.CommandText = $"SELECT COUNT(*) FROM {tableName.ToLower()}";
                command.AddParameters(true, sqlParams);

                count = (int) SQLiteUtils.ConvertValue(command.ExecuteScalar(), typeof(int));
                success = count > 0;

                conn.Close();
            }

            return success;
        }

        #endregion

        #region Util methods

        /// <summary>
        /// Execute an sql script from path (If the path is a directory, it will execute all the subdirectories and files within it)
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="logs">Show logs?</param>
        /// <returns></returns>
        public static bool ExecuteFromPath(string path, bool logs = true)
        {
            if (logs)
                WriteLine(MessageCode.Message, $"ExecuteFromPath(\"{path}\")");

            try
            {
                // Directory
                if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                {
                    foreach (string p in Directory.EnumerateFileSystemEntries(path))
                    {
                        if (File.GetAttributes(p).HasFlag(FileAttributes.Directory))
                        {
                            ExecuteFromPath(path);
                        }
                        else
                        {
                            if (logs)
                                WriteLine(MessageCode.Message, $"Executing .sql file: {p}");

                            string script = File.ReadAllText(p);
                            Execute(script);
                        }
                    }
                }
                // File
                else
                {
                    if (logs)
                        WriteLine(MessageCode.Message, $"Executing .sql file: {path}");

                    string script = File.ReadAllText(path);
                    Execute(script);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        /// <summary>
        /// Sets the database path for the SQLite database (must be called before any commands gets executed)
        /// </summary>
        /// <param name="path"></param>
        public static void SetDatabasePath(string path)
        {
            databasePath = path;
        }

        /// <summary>
        /// Create a new blueprint to rebuild the whole SQL database. <br/>
        /// Specify <paramref name="path"/> to create a file.
        /// </summary>
        /// <param name="path">Path to create .sql file (specify its name as well)</param>
        /// <returns>Blueprint in string</returns>
        public static string CreateBlueprint(string path = "")
        {
            if (!string.IsNullOrEmpty(path) && !path.EndsWith(".sql"))
            {
                WriteLine(MessageCode.Error, $"Failed at {nameof(CreateBlueprint)}: path must end with \".sql\".");
                return "";
            }

            List<string> tableNames = new List<string>();
            StringBuilder blueprint = new StringBuilder();

            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (SQLiteCommand command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT name, sql FROM sqlite_master WHERE type='table'";

                    // Create table sql statements
                    // Iterate through all the table names and its create sqls
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
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
                        Count(name, out int rowCount);

                        // Read all rows from table
                        command.CommandText = $"SELECT * FROM {name}";

                        using SQLiteDataReader reader = command.ExecuteReader();

                        // Check if there's any row to insert
                        if (!reader.HasRows) continue;

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
                }

                conn.Close();

                // Garbage collect because Close() is not releasing the db file
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            if (!string.IsNullOrEmpty(path))
            {
                // Write the new blueprint to the path
                File.WriteAllText(path, blueprint.ToString());

                WriteLine(MessageCode.Message, $"Blueprint successfully created at: {path}");
            }

            return blueprint.ToString();
        }

        #endregion

        #region Internal methods

        ///// <summary>
        ///// Util method to Console.WriteLine with a color coded message
        ///// </summary>
        ///// <param name="code">Message code (Error/Message/Warning)</param>
        ///// <param name="message">Message content</param>
        //internal static void WriteLine(MessageCode code, string message)
        //{
        //    // Color coded according to message
        //    ConsoleColor temp = Console.ForegroundColor;
        //    switch (code)
        //    {
        //        case MessageCode.Error:
        //            Console.ForegroundColor = ConsoleColor.Red;
        //            break;

        //        case MessageCode.Message:
        //            Console.ForegroundColor = ConsoleColor.Gray;
        //            break;

        //        case MessageCode.Warning:
        //            Console.ForegroundColor = ConsoleColor.Yellow;
        //            break;

        //        default:
        //            WriteLine(MessageCode.Error, $"Failed in {nameof(WriteLine)}, unhandled switch-case type of {typeof(MessageCode)}");
        //            break;
        //    }

        //    Console.Write($"[SQLManager {code}] ");
        //    Console.WriteLine($"{message}");

        //    // Reassign console color back to what it was
        //    Console.ForegroundColor = temp;
        //}

        /// <summary>
        /// Util method to send Debug messages through Unity
        /// </summary>
        /// <param name="code">Message code (Error/Message/Warning)</param>
        /// <param name="message">Message content</param>
        internal static void WriteLine(MessageCode code, string message)
        {
            switch (code)
            {
                case MessageCode.Error:
                    Debug.LogError(message);
                    break;

                case MessageCode.Message:
                    Debug.Log(message);
                    break;

                case MessageCode.Warning:
                    Debug.LogWarning(message);
                    break;

                default:
                    WriteLine(MessageCode.Error, $"Failed in {nameof(WriteLine)}, unhandled switch-case type of {typeof(MessageCode)}");
                    break;
            }
        }

        /// <summary>
        /// Checks if Type is Subclass of SQLObject
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="scalar">ReadScalar?</param>
        /// <returns>Correct</returns>
        private static bool HasSubclass(Type t, bool scalar = false)
        {
            if (scalar == t.IsSubclassOf(typeof(SQLiteObject)))
            {
                string errorMsg = scalar
                              ? $"Trying to do a ReadScalar with a Class object. Use Read() instead."
                              : $"Trying to read from Type {t}, but it does not not inherit from {typeof(SQLiteObject)}.";

                WriteLine(MessageCode.Error, errorMsg);
                return false;
            }

            return true;
        }

        private static bool TryCreateInstance<T>(List<object> args, out T obj)
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

                WriteLine(MessageCode.Error, $"Failed to read from the database. No valid constructor was found for {typeof(T)} with {args.Count} arguments ({argsStr})");
                WriteLine(MessageCode.Error, $"Exception: {ex}");
                return false;
            }
        }

        #endregion
    }
}
