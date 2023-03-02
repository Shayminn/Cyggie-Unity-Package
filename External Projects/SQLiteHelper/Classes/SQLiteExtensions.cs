using System;
using System.Data.SQLite;

namespace Cyggie.SQLite
{
    internal static class SQLiteExtensions
    {
        /// <summary>
        /// Extension for SQLiteCommand to add paramaters with SQLParams
        /// </summary>
        /// <param name="command">SQLiteCommand obj</param>
        /// <param name="whereStatement">Add where statement to command text?</param>
        /// <param name="sqlParams">SQLParams</param>
        internal static void AddParameters(this SQLiteCommand command, bool whereStatement, params SQLiteParams[] sqlParams)
        {
            if (sqlParams.Length > 0)
            {
                if (whereStatement)
                    command.CommandText += " WHERE ";

                foreach (SQLiteParams paramss in sqlParams)
                {
                    if (whereStatement)
                    {
                        // Add paramss to command text
                        // Substring to skip the @
                        command.CommandText += $"{paramss.ParameterKey[1..]}={paramss.ParameterKey}";
                    }

                    // Add parameter
                    command.Parameters.AddWithValue(paramss.ParameterKey, paramss.ParameterValue);
                }
            }
        }
    }
}
