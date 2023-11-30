using System;

namespace Cyggie.Plugins.Logs
{
    /// <summary>
    /// Type of supported logs
    /// </summary>
    [Flags]
    public enum LogTypes
    {
        /// <summary>
        /// None enabled
        /// </summary>
        None = 0,

        /// <summary>
        /// <see cref="Log.Debug(object, string, object[])"/> enabled
        /// </summary>
        Debug = 1 << 0,

        /// <summary>
        /// <see cref="Log.Warning(object, string, object[])"/> enabled
        /// </summary>
        Warning = 1 << 1,

        /// <summary>
        /// <see cref="Log.Error(object, string, object[])"/> enabled
        /// </summary>
        Error = 1 << 2
    }
}
