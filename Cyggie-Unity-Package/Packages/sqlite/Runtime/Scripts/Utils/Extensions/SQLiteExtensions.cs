using Cyggie.Main.Runtime.Utils.Extensions;
using Mono.Data.Sqlite;
using System.Data;

namespace Cyggie.Runtime.SQLite.Utils.Extensions
{
    internal static class SQLiteExtensions
    {
        /// <summary>
        /// Extension for SQLiteCommand to add paramaters with SQLParams
        /// </summary>
        /// <param name="command">SQLiteCommand obj</param>
        /// <param name="whereStatement">Add where statement to command text?</param>
        /// <param name="sqlParams">SQLParams</param>
        internal static void AddParameters(this IDbCommand command, bool whereStatement, params SqliteParameter[] sqlParams)
        {
            if (sqlParams.Length > 0)
            {
                if (whereStatement)
                    command.CommandText += " WHERE ";

                foreach (SqliteParameter paramss in sqlParams)
                {
                    paramss.ParameterName = paramss.ParameterName.InsertStartsWith("@");

                    if (whereStatement)
                    {
                        // Add paramss to command text
                        // Substring to skip the @
                        command.CommandText += $"{paramss.ParameterName[1..]}={paramss.Value}";
                    }

                    // Add parameter
                    command.Parameters.Add(paramss);
                }
            }
        }
    }
}
