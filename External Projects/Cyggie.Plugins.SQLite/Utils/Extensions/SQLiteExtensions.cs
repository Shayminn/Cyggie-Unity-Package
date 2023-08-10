using Mono.Data.Sqlite;

namespace Cyggie.Plugins.SQLite.Utils.Extensions
{
    /// <summary>
    /// Extension class for SQLite related classes
    /// </summary>
    internal static class SQLiteExtensions
    {
        /// <summary>
        /// Extension for <see cref="SqliteCommand"/> to add paramaters with SQLParams
        /// </summary>
        /// <param name="command">Command obj</param>
        /// <param name="sqlParams">SQLParams associated to the where statement</param>
        internal static void AddParameters(this SqliteCommand command, params SQLiteParams[] sqlParams)
        {
            if (sqlParams.Length > 0)
            {
                foreach (SQLiteParams paramss in sqlParams)
                {
                    // Add parameter
                    command.Parameters.AddWithValue(paramss.ParameterKey, paramss.ParameterValue);
                }
            }
        }
    }
}
