using Cyggie.Plugins.Logs.Models.Interfaces;
using System;

namespace Cyggie.Plugins.Logs.Models
{
    /// <summary>
    /// Log model for logging into the console using <see cref="Console.WriteLine()"/>
    /// </summary>
    public struct LogConsole : ILog
    {
        private string _message;
        /// <inheritdoc/>
        public readonly string Message => _message;

        private string _tag;
        /// <inheritdoc/>
        public readonly string Tag => _tag;

        private object[] _args;
        /// <inheritdoc/>
        public readonly object[] Args => _args;

        private DateTime _timestamp;
        /// <inheritdoc/>
        public readonly DateTime Timestamp => _timestamp;

        /// <inheritdoc/>
        public readonly string FullLog => $"[{_timestamp:HH:mm:ss:FFF}] {(string.IsNullOrEmpty(_tag) ? ILog.cEmptyTag : _tag)} | {_message}";

        /// <inheritdoc/>
        public void Initialize(string message, string tag, params object[] args)
        {
            _message = message;
            _tag = tag;
            _args = args;
            _timestamp = DateTime.Now;
        }

        /// <inheritdoc/>
        public string Print(LogTypes type)
        {
            string stackTrace = string.Empty;
            switch (type)
            {
                case LogTypes.Debug:
                    WriteLine(ConsoleColor.Black, ConsoleColor.White, FullLog, out stackTrace);
                    break;
                case LogTypes.Warning:
                    WriteLine(ConsoleColor.Yellow, ConsoleColor.Black, FullLog, out stackTrace);
                    break;
                case LogTypes.Error:
                    WriteLine(ConsoleColor.Red, ConsoleColor.Black, FullLog, out stackTrace);
                    break;
                case LogTypes.None:
                default:
                    break;
            }

            return stackTrace;
        }

        private readonly void WriteLine(ConsoleColor backgroundColor, ConsoleColor foregroundColor, string text, out string stackTrace)
        {
            ConsoleColor oldBgColor = Console.BackgroundColor;
            ConsoleColor oldFgColor = Console.ForegroundColor;

            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(text);
            stackTrace = System.Environment.StackTrace;

            Console.BackgroundColor = oldBgColor;
            Console.ForegroundColor = oldFgColor;
        }
    }
}
