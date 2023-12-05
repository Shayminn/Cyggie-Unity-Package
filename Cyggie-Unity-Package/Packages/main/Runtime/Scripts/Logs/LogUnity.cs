using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Logs.Models.Interfaces;
using System;
using System.Linq;
using UnityEngine;

namespace Cyggie.Main.Runtime
{
    /// <summary>
    /// Log model for logging into the Unity console using <see cref="Debug"/>
    /// </summary>
    public struct LogUnity : ILog
    {
        private string _message;
        /// <inheritdoc/>
        public readonly string Message => _message;

        private string _tag;
        public readonly string Tag => _tag;

        private object[] _args;
        /// <inheritdoc/>
        public readonly object[] Args => _args;

        private DateTime _timestamp;
        public readonly DateTime Timestamp => _timestamp;

        /// <inheritdoc/>
        public readonly string FullLog => $"[{_timestamp:HH:mm:ss:FFF}] {(string.IsNullOrEmpty(_tag) ? ILog.cEmptyTag : _tag)} | {_message}";

        /// <summary>
        /// Context associated to the debug log (first object in <see cref="_args"/>)
        /// </summary>
        public UnityEngine.Object Context { get; private set; }

        /// <inheritdoc/>
        public void Initialize(string message, string tag, params object[] args)
        {
            _message = message;
            _tag = tag;
            _args = args;
            _timestamp = DateTime.Now;

            Context = (UnityEngine.Object) args.FirstOrDefault(x => x is UnityEngine.Object);
        }

        /// <inheritdoc/>
        public string Print(LogTypes type)
        {
            string stackTrace = string.Empty;

            switch (type)
            {
                case LogTypes.None:
                default:
                    break;
                case LogTypes.Debug:
                    Debug.Log(FullLog, Context);
                    stackTrace = StackTraceUtility.ExtractStackTrace();
                    break;
                case LogTypes.Warning:
                    Debug.LogWarning(FullLog, Context);
                    stackTrace = StackTraceUtility.ExtractStackTrace();
                    break;
                case LogTypes.Error:
                    Debug.LogError(FullLog, Context);
                    stackTrace = StackTraceUtility.ExtractStackTrace();
                    break;
            }

            return stackTrace;
        }
    }
}
