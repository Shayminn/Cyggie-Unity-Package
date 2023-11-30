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

        private object[] _args;
        /// <inheritdoc/>
        public readonly object[] Args => _args;

        private DateTime _timestamp;
        /// <inheritdoc/>
        public readonly DateTime Timestamp => _timestamp;

        /// <inheritdoc/>
        public string Print(LogTypes type, string message, string tag, params object[] args)
        {
            _message = message;
            _args = args;
            _timestamp = DateTime.Now;

            string stackTrace = string.Empty;
            string fullMessage = $"[{_timestamp:HH:mm:ss:FFF}] {tag} | {message}";
            switch (type)
            {
                case LogTypes.Debug:
                    WriteLine(ConsoleColor.Black, ConsoleColor.White, fullMessage, out stackTrace);
                    break;
                case LogTypes.Warning:
                    WriteLine(ConsoleColor.Yellow, ConsoleColor.Black, fullMessage, out stackTrace);
                    break;
                case LogTypes.Error:
                    WriteLine(ConsoleColor.Red, ConsoleColor.Black, fullMessage, out stackTrace);
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
