using UnityEngine;

namespace Cyggie.Plugins.Logs
{
    /// <summary>
    /// Event arguments for a log of type <see cref="LogTypes.Debug"/>
    /// </summary>
    public class DebugLogEventArgs : LogEventArgs
    {
        /// <inheritdoc/>
        public override LogTypes Type => LogTypes.Debug;

        internal DebugLogEventArgs(string log, string tag, UnityEngine.Object? context) : base(log, tag, context) { }

        /// <inheritdoc/>
        internal override void Send() => Debug.Log(Log, Context);
    }
}
