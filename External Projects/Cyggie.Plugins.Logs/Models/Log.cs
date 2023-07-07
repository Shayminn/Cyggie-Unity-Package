using Cyggie.Plugins.Logs.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Cyggie.Plugins.Logs
{
    /// <summary>
    /// Static class to manage Logs
    /// </summary>
    public static class Log
    {
        private const string cPipeSeparator = " | ";

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

        /// <summary>
        /// Event called when there's a new Debug log
        /// </summary>
        /// <param name="args">Event args object</param>
        public delegate void DebugLogEvent(DebugLogEventArgs args);
        /// <summary>
        /// Event called when there's a new Debug log
        /// </summary>
        public static DebugLogEvent? OnDebugEvent = null;

        /// <summary>
        /// Event called when there's a new Warning log
        /// </summary>
        /// <param name="args">Event args object</param>
        public delegate void WarningLogEvent(WarningLogEventArgs args);
        /// <summary>
        /// Event called when there's a new Warning log
        /// </summary>
        public static WarningLogEvent? OnWarningEvent = null;

        /// <summary>
        /// Event called when there's a new Error log
        /// </summary>
        /// <param name="args">Event args object</param>
        public delegate void ErrorLogEvent(ErrorLogEventArgs args);
        /// <summary>
        /// Event called when there's a new Error log
        /// </summary>
        public static ErrorLogEvent? OnErrorEvent = null;

        #endregion

        private static readonly Queue<LogEventArgs> _args = new Queue<LogEventArgs>();
        private static LogProfile? _profile = null;
        private static bool _enabled = false;

        private static void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (_args.Count == 0) return;

            LogEventArgs args = _args.Dequeue();
            args.StackTrace = stackTrace;

            switch (args)
            {
                case DebugLogEventArgs debugArgs:
                    OnDebugEvent?.Invoke(debugArgs);
                    break;
                case WarningLogEventArgs warningArgs:
                    OnWarningEvent?.Invoke(warningArgs);
                    break;
                case ErrorLogEventArgs errorArgs:
                    OnErrorEvent?.Invoke(errorArgs);
                    break;
            }

            OnLogEvent?.Invoke(args);
        }

        #region Public methods

        /// <summary>
        /// Send a debug message <br/>
        /// If the current profile does not include Debugs, this will be ignored <br/>
        /// You can manage profiles in the toolbar at Cyggie/Package Configurations <br/>
        /// Format: Tag | [Timestamp] Message StackTrace
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tag">Tag to add</param>
        /// <param name="context">The Object that this log is associated to, clicking on this log will select the Object</param>
        public static void Debug(object message, string tag = "", UnityEngine.Object? context = null) => SendLog(LogTypes.Debug, message, tag, context);

        /// <summary>
        /// Send a warning message <br/>
        /// If the current profile does not include Warnings, this will be ignored <br/>
        /// You can manage profiles in the toolbar at Cyggie/Package Configurations
        /// Format: Tag | [Timestamp] Message StackTrace
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tag">Tag to add</param>
        /// <param name="context">The Object that this log is associated to, clicking on this log will select the Object</param>
        public static void Warning(object message, string tag = "", UnityEngine.Object? context = null) => SendLog(LogTypes.Warning, message, tag, context);

        /// <summary>
        /// Send a error message <br/>
        /// If the current profile does not include Errors, this will be ignored <br/>
        /// You can manage profiles in the toolbar at Cyggie/Package Configurations
        /// Format: Tag | [Timestamp] Message StackTrace
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tag">Tag to add</param>
        /// <param name="context">The Object that this log is associated to, clicking on this log will select the Object</param>
        public static void Error(object message, string tag = "", UnityEngine.Object? context = null) => SendLog(LogTypes.Error, message, tag, context);

        /// <summary>
        /// Set the profile's log types during runtime <br/>
        /// This will not update the profile's Serialized Object (if any) <br/>
        /// Changes only affect the current run
        /// </summary>
        /// <param name="types">Filter types (flags)</param>
        public static void SetTypes(LogTypes types)
        {
            if (_profile == null) return;

            _profile.Types = types;
        }

        /// <summary>
        /// Enable logs
        /// </summary>
        public static void Enable()
        {
            _enabled = true;
            Application.logMessageReceived += OnLogMessageReceived;
        }

        /// <summary>
        /// Disable logs
        /// </summary>
        public static void Disable()
        {
            _enabled = false;
            Application.logMessageReceived -= OnLogMessageReceived;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Set the current profile based on the <see cref="Application.platform"/>
        /// </summary>
        /// <param name="profile">Platform's profile</param>
        internal static void SetProfile(LogProfile profile) => _profile = profile;

        #endregion

        private static void SendLog(LogTypes type, object message, string tag, UnityEngine.Object? context)
        {
            if (!_enabled) return;

            // Use default if not set
            if (_profile == null)
            {
                _profile = ScriptableObject.CreateInstance<LogProfile>();
            }

            if (!_profile.IsEnabled(type)) return;

            string log = BuildLog(message, tag);

            // Change stack trace
            LogType unityType = type.ToUnityType();
            StackTraceLogType previousStackLogType = Application.GetStackTraceLogType(unityType);
            StackTraceLogType newStackLogType = _profile.ShowStackTrace ?
                                                _profile.StackTraceScriptOnly ? StackTraceLogType.ScriptOnly : StackTraceLogType.Full :
                                                StackTraceLogType.None;

            Application.SetStackTraceLogType(unityType, newStackLogType);

            LogEventArgs? args = type switch
            {
                LogTypes.Debug => new DebugLogEventArgs(log, tag, context),
                LogTypes.Warning => new WarningLogEventArgs(log, tag, context),
                LogTypes.Error => new ErrorLogEventArgs(log, tag, context),
                _ => null
            };

            if (args == null) return;

            _args.Enqueue(args);
            args.Send();

            Application.SetStackTraceLogType(unityType, previousStackLogType);
        }

        /// <summary>
        /// Build the complete log
        /// </summary>
        /// <param name="message">The origianl message</param>
        /// <param name="tag">The input tag</param>
        /// <returns></returns>
        private static string BuildLog(object message, string tag)
        {
            StringBuilder builder = new StringBuilder();

            if (!string.IsNullOrEmpty(tag))
            {
                builder.Append(tag);
                builder.Append(cPipeSeparator);
            }

            if (_profile != null && _profile.ShowTimestamp)
            {
                builder.Append('[');
                builder.Append(DateTime.Now.ToString(_profile.TimestampFormat));
                builder.Append(']');
                builder.Append(' ');
            }

            builder.Append(message);
            return builder.ToString();
        }
    }
}
