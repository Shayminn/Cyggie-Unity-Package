using System;

namespace Cyggie.Plugins.Logs.Models.Interfaces
{
    /// <summary>
    /// Interface for a log model used by <see cref="Log"/> when calling logs
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Message to log
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Arguments with the log
        /// </summary>
        object[] Args { get; }

        /// <summary>
        /// Timestamp of the log following the format (hour:minute:second:millisecond, eg. 14:53:24:460)
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Print the log to wherever
        /// </summary>
        /// <returns>Stacktrace of the log</returns>
        string Print(LogTypes type, string message, string tag, params object[] args);
    }
}
