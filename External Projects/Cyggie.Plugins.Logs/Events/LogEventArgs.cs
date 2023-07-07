using System;

namespace Cyggie.Plugins.Logs
{
    /// <summary>
    /// Event arguments for a log
    /// </summary>
    public abstract class LogEventArgs
    {
        /// <summary>
        /// Type of log
        /// </summary>
        public abstract LogTypes Type { get; }

        /// <summary>
        /// The full log that's displayed in the console based on the <see cref="LogProfile"/> that was set
        /// </summary>
        public string FullLog { get; private set; } = "";

        /// <summary>
        /// The log message that was inputted
        /// </summary>
        public string Log { get; private set; } = "";

        /// <summary>
        /// Tag that was inputted with the log <br/>
        /// This can be used for Class name, method name, etc.
        /// </summary>
        public string Tag { get; private set; } = "";

        /// <summary>
        /// The timestamp that the log was sent
        /// </summary>
        public DateTime Timestamp { get; private set; } = default;

        /// <summary>
        /// The log's stack trace
        /// </summary>
        public string StackTrace { get; internal set; } = "";

        /// <summary>
        /// The object's context
        /// </summary>
        public UnityEngine.Object? Context { get; private set; } = null;

        internal LogEventArgs(string log, string tag, UnityEngine.Object? context)
        {
            Log = log;
            Tag = tag;
            Context = context;
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Send the log
        /// </summary>
        internal abstract void Send();
    }
}
