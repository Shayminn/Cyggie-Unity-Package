using System;

namespace Cyggie.Plugins.Logs.Models.Interfaces
{
    /// <summary>
    /// Interface for a log model used by <see cref="Log"/> when calling logs
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Tag used in replacement when no tag is set in the log
        /// </summary>
        protected const string cEmptyTag = "(no tag)";

        /// <summary>
        /// Message to log
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Tag associated to the message
        /// </summary>
        string Tag { get; }

        /// <summary>
        /// Arguments with the log
        /// </summary>
        object[] Args { get; }

        /// <summary>
        /// Timestamp of the log following the format (hour:minute:second:millisecond, eg. 14:53:24:460)
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The full log that is displayed (combination of <see cref="Timestamp"/>, <see cref="Tag"/> and <see cref="Message"/>
        /// </summary>
        string FullLog { get; }

        /// <summary>
        /// Initialize the log's values
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tag">Tag of the log</param>
        /// <param name="args">Arguments to the log</param>
        void Initialize(string message, string tag, params object[] args);

        /// <summary>
        /// Print the log to wherever
        /// </summary>
        /// <param name="type">Type of log</param>
        /// <returns>Stacktrace of the log</returns>
        string Print(LogTypes type);
    }
}
