using System;
using UnityEngine;

namespace Cyggie.Main.Runtime
{
    /// <summary>
    /// Event arguments for a log
    /// </summary>
    public abstract class LogEventArgs
    {
        /// <summary>
        /// The full log that's displayed in the console based on the <see cref="LogProfile"/> that was set
        /// </summary>
        public string FullLog { get; private set; }

        /// <summary>
        /// The log message that was inputted
        /// </summary>
        public string Log { get; private set; }

        /// <summary>
        /// Tag that was inputted with the log <br/>
        /// This can be used for Class name, method name, etc.
        /// </summary>
        public string Tag { get; private set; }

        /// <summary>
        /// The timestamp that the log was sent
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// The log's stack trace
        /// </summary>
        public string StackTrace { get; private set; }

        internal LogEventArgs(string log, string tag)
        {
            Log = log;
            Tag = tag;
        }
    }
}
