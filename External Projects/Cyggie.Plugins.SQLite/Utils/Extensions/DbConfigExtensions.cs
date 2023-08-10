namespace Cyggie.Plugins.SQLite.Utils.Extensions
{
    /// <summary>
    /// Extension class for <see cref="DbConfig"/>
    /// </summary>
    internal static class DbConfigExtensions
    {
        /// <summary>
        /// Get the default value for <paramref name="config"/>
        /// </summary>
        /// <param name="config">Database configuration</param>
        /// <returns>Default value (empty if not configured)</returns>
        internal static object GetDefaultValue(this DbConfig config)
        {
            return config switch
            {
                DbConfig.Encrypted => false.ToString(),
                DbConfig.ReadOnly => false.ToString(),
                _ => string.Empty
            };
        }
    }
}
