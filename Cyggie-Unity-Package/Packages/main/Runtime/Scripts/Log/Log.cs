using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Main.Runtime.Utils.Constants;
using Cyggie.Main.Runtime.Utils.Extensions;
using System;
using System.Text;
using UnityEngine;

namespace Cyggie.Main.Runtime
{
    /// <summary>
    /// Static class to manage Logs
    /// </summary>
    public static class Log
    {
        private static LogProfile _profile = null;

        #region Events

        public delegate void LogEvent(LogEventArgs args);
        /// <summary>
        /// Event called when there's a new log
        /// </summary>
        public static LogEvent OnLogEvent = null;

        public delegate void DebugLogEvent(DebugLogEventArgs args);
        /// <summary>
        /// Event called when there's a new Debug log
        /// </summary>
        public static DebugLogEvent OnDebugEvent = null;

        public delegate void WarningLogEvent(WarningLogEventArgs args);
        /// <summary>
        /// Event called when there's a new Warning log
        /// </summary>
        public static WarningLogEvent OnWarningEvent = null;

        public delegate void ErrorLogEvent(ErrorLogEventArgs args);
        /// <summary>
        /// Event called when there's a new Error log
        /// </summary>
        public static ErrorLogEvent OnErrorEvent = null;

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
        /// <param name="context">The Object that this log is associated to, clicking on this log will select the Object</param>
        public static void Debug(object message, string tag = "", UnityEngine.Object context = null) => SendLog(LogTypes.Debug, message, tag, context);

        /// <summary>
        /// Send a warning message <br/>
        /// If the current profile does not include Warnings, this will be ignored <br/>
        /// You can manage profiles in the toolbar at Cyggie/Package Configurations
        /// Format: Tag | [Timestamp] Message StackTrace
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tag">Tag to add</param>
        /// <param name="context">The Object that this log is associated to, clicking on this log will select the Object</param>
        public static void Warning(object message, string tag = "", UnityEngine.Object context = null) => SendLog(LogTypes.Warning, message, tag, context);

        /// <summary>
        /// Send a error message <br/>
        /// If the current profile does not include Errors, this will be ignored <br/>
        /// You can manage profiles in the toolbar at Cyggie/Package Configurations
        /// Format: Tag | [Timestamp] Message StackTrace
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tag">Tag to add</param>
        /// <param name="context">The Object that this log is associated to, clicking on this log will select the Object</param>
        public static void Error(object message, string tag = "", UnityEngine.Object context = null) => SendLog(LogTypes.Error, message, tag, context);

        /// <summary>
        /// Set the profile's log types during runtime <br/>
        /// This will not update the profile's Serialized Object (if any) <br/>
        /// Changes only affect the current run
        /// </summary>
        /// <param name="types">Filter types (flags)</param>
        public static void SetTypes(LogTypes types) => _profile.Types = types;

        /// <summary>
        /// Toggle logs during runtime
        /// </summary>
        /// <param name="toggle">Enabled/Disabled</param>
        public static void Toggle(bool toggle) => ServiceManager.Settings.EnableLog = toggle;

        #endregion

        #region Internal methods

        /// <summary>
        /// Set the current profile based on the <see cref="Application.platform"/>
        /// </summary>
        /// <param name="profile">Platform's profile</param>
        internal static void SetProfile(LogProfile profile) => _profile = profile;

        #endregion

        private static void SendLog(LogTypes type, object message, string tag, UnityEngine.Object context)
        {
            if (!ServiceManager.Settings.EnableLog) return;

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
            LogEventArgs args = null;

            switch (type)
            {
                case LogTypes.Debug:
                    UnityEngine.Debug.Log(log, context);
                    args = new DebugLogEventArgs(log, tag);
                    OnDebugEvent?.Invoke(args as DebugLogEventArgs);
                    break;

                case LogTypes.Warning:
                    UnityEngine.Debug.LogWarning(log, context);
                    args = new WarningLogEventArgs(log, tag);
                    OnWarningEvent?.Invoke(args as WarningLogEventArgs);
                    break;

                case LogTypes.Error:
                    UnityEngine.Debug.LogError(log, context);
                    args = new ErrorLogEventArgs(log, tag);
                    OnErrorEvent?.Invoke(args as ErrorLogEventArgs);
                    break;

                default:
                    UnityEngine.Debug.LogError($"Unhandled switch-case type of {typeof(LogTypes)}: {type}");
                    break;
            }

            OnLogEvent?.Invoke(args);
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
                builder.Append(StringConstants.cPipeSeparator);
            }

            if (_profile.ShowTimestamp)
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
