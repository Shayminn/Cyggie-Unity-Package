namespace Cyggie.Plugins.SQLite
{
    /// <summary>
    /// List of database configurations for each <see cref="SQLiteDatabase"/>
    /// </summary>
    internal enum DbConfig
    {
        /// <summary>
        /// Determines if the database is encrypted (bool) <br/>
        /// If enabled, <see cref="DbConfig.ReadOnly"/> is also enabled <br/>
        /// If takes slightly more time to read when encrypted as it decrypts during runtime
        /// </summary>
        Encrypted,

        /// <summary>
        /// Determines if the database is in read-only mode (bool) <br/>
        /// If enabled, only Read methods can be performed on the database
        /// </summary>
        ReadOnly
    }
}
