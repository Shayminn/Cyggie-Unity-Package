using Cyggie.Plugins.Logs.Models;
using Cyggie.Plugins.Logs.Models.Interfaces;
using System;

namespace Cyggie.Plugins.Logs
{
    /// <summary>
    /// Static class to manage Logs
    /// </summary>
    public static class Log
    {
        private static LogTypes _enabledTypes = LogTypes.Debug | LogTypes.Warning | LogTypes.Error;
        private static Type _logModel = typeof(LogConsole);

        #region Events

        /// <summary>
        /// Event called when there's a new log
        /// </summary>
        /// <param name="args">Event args object</param>
        public delegate void LogEvent(LogEventArgs args);

        /// <summary>
        /// Event called when there's a new log
        /// </summary>
        public static LogEvent? OnLogEvent = null;

        #endregion

        #region Public methods

        /// <summary>
        /// Send a debug message <br/>
        /// If the current profile does not include Debugs, this will be ignored <br/>
        /// You can manage profiles in the toolbar at Cyggie/Package Configurations <br/>
        /// Format: Tag | [Timestamp] Message StackTrace
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tag">Tag to add</param>
        /// <param name="args"></param>
        public static void Debug(object message, string tag = "", params object[] args) => SendLog(LogTypes.Debug, message, tag, args);

        /// <summary>
        /// Send a warning message <br/>
        /// If the current profile does not include Warnings, this will be ignored <br/>
        /// You can manage profiles in the toolbar at Cyggie/Package Configurations
        /// Format: Tag | [Timestamp] Message StackTrace
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tag">Tag to add</param>
        /// <param name="args"></param>
        public static void Warning(object message, string tag = "", params object[] args) => SendLog(LogTypes.Warning, message, tag, args);

        /// <summary>
        /// Send a error message <br/>
        /// If the current profile does not include Errors, this will be ignored <br/>
        /// You can manage profiles in the toolbar at Cyggie/Package Configurations
        /// Format: Tag | [Timestamp] Message StackTrace
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tag">Tag to add</param>
        /// <param name="args"></param>
        public static void Error(object message, string tag = "", params object[] args) => SendLog(LogTypes.Error, message, tag, args);

        /// <summary>
        /// Toggle the enabled log types
        /// </summary>
        public static void ToggleLogs(LogTypes logTypes)
        {
            _enabledTypes = logTypes;
        }

        /// <summary>
        /// Set the log model to use
        /// </summary>
        /// <typeparam name="T">Log model to use for any logs that are sent through</typeparam>
        public static void SetLogModel<T>()
            where T : ILog
        {
            if (typeof(T).IsAbstract)
            {
                return;
            }

            _logModel = typeof(T);
        }

        #endregion

        private static void SendLog(LogTypes type, object message, string tag, params object[] args)
        {
            if (!_enabledTypes.HasFlag(type)) return;

            ILog log = (ILog) Activator.CreateInstance(_logModel);
            string stackTrace = log.Print(type, message.ToString(), tag, args);

            LogEventArgs eventArgs = new LogEventArgs(type, log, stackTrace);
            OnLogEvent?.Invoke(eventArgs);
        }
    }
}
