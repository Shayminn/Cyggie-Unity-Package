using Cyggie.Plugins.Logs;
using System;
using UnityEngine;

namespace Cyggie.Plugins.Logs
{
    /// <summary>
    /// Class that holds log settings for a <see cref="RuntimePlatform"/>
    /// </summary>
    [Serializable]
    internal class LogProfile : ScriptableObject
    {
        /// <summary>
        /// Array's display name in Inspector
        /// </summary>
        [HideInInspector]
        public string Name = "";

        [SerializeField, Tooltip("The platform that this profile is associated to.")]
        internal RuntimePlatform Platform = RuntimePlatform.WindowsEditor;

        [SerializeField, Tooltip("The enabled log types.")]
        internal LogTypes Types = LogTypes.Debug | LogTypes.Warning | LogTypes.Error;

        [SerializeField, Tooltip("Whether the Timestamp is displayed in the log.")]
        internal bool ShowTimestamp = true;
        [SerializeField, Tooltip("The timestamp's format.")]
        internal string TimestampFormat = "hh:mm:ss tt"; // default format: [00:00:00 AM]

        [SerializeField, Tooltip("Whether the Stack Trace is displayed in the log.")]
        internal bool ShowStackTrace = true;

        [SerializeField, Tooltip("Whether the Stack Trace should only show the scripts.")]
        internal bool StackTraceScriptOnly = true;

        internal bool IsEnabled(LogTypes type) => Types.HasFlag(type);
    }
}
