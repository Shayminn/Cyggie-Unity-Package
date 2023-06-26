namespace Cyggie.Main.Runtime
{
    /// <summary>
    /// Event arguments for a log of type <see cref="LogTypes.Debug"/>
    /// </summary>
    public class DebugLogEventArgs : LogEventArgs
    {
        /// <inheritdoc/>
        public override LogTypes Type => LogTypes.Debug;

        internal DebugLogEventArgs(string log, string tag) : base(log, tag)
        {

        }
    }
}
