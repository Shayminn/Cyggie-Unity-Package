namespace Cyggie.Main.Runtime
{
    /// <summary>
    /// Event arguments for a log of type <see cref="LogTypes.Debug"/>
    /// </summary>
    public class DebugLogEventArgs : LogEventArgs
    {
        internal DebugLogEventArgs(string log, string tag) : base(log, tag)
        {

        }
    }
}
