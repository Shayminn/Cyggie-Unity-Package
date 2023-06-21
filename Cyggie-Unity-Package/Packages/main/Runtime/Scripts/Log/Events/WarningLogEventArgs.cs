namespace Cyggie.Main.Runtime
{
    /// <summary>
    /// Event arguments for a log of type <see cref="LogTypes.Warning"/>
    /// </summary>
    public class WarningLogEventArgs : LogEventArgs
    {
        internal WarningLogEventArgs(string log, string tag) : base(log, tag)
        {

        }
    }
}