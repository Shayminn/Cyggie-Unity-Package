namespace Cyggie.Main.Runtime
{
    /// <summary>
    /// Event arguments for a log of type <see cref="LogTypes.Error"/>
    /// </summary>
    public class ErrorLogEventArgs : LogEventArgs
    {
        /// <inheritdoc/>
        public override LogTypes Type => LogTypes.Error;

        internal ErrorLogEventArgs(string log, string tag) : base(log, tag)
        {

        }
    }
}
