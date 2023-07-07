using UnityEngine;

namespace Cyggie.Plugins.Logs
{
    /// <summary>
    /// Event arguments for a log of type <see cref="LogTypes.Warning"/>
    /// </summary>
    public class WarningLogEventArgs : LogEventArgs
    {
        /// <inheritdoc/>
        public override LogTypes Type => LogTypes.Warning;

        internal WarningLogEventArgs(string log, string tag, Object? context) : base(log, tag, context) { }

        /// <inheritdoc/>
        internal override void Send() => Debug.LogWarning(Log, Context);
    }
}
