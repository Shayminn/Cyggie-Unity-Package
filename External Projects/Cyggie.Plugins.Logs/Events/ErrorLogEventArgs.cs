using UnityEngine;

namespace Cyggie.Plugins.Logs
{
    /// <summary>
    /// Event arguments for a log of type <see cref="LogTypes.Error"/>
    /// </summary>
    public class ErrorLogEventArgs : LogEventArgs
    {
        /// <inheritdoc/>
        public override LogTypes Type => LogTypes.Error;

        internal ErrorLogEventArgs(string log, string tag, UnityEngine.Object? context) : base(log, tag, context) { }

        /// <inheritdoc/>
        internal override void Send() => Debug.LogError(Log, Context);
    }
}
