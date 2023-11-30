using Cyggie.Plugins.Logs.Models.Interfaces;

namespace Cyggie.Plugins.Logs
{
    /// <summary>
    /// Event arguments for a log
    /// </summary>
    public class LogEventArgs
    {
        /// <summary>
        /// Type of log
        /// </summary>
        public LogTypes Type { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ILog Log { get; private set; }

        /// <summary>
        /// The log's stack trace
        /// </summary>
        public string StackTrace { get; internal set; } = "";

        internal LogEventArgs(LogTypes type, ILog log, string stackTrace)
        {
            Type = type;
            Log = log;
            StackTrace = stackTrace;
        }
    }
}
