using System;

namespace Cyggie.Main.Runtime
{
    /// <summary>
    /// The supported log types
    /// </summary>
    [Flags]
    internal enum LogTypes
    {
        None = 0,
        Debug = 1 << 0,
        Warning = 1 << 1,
        Error = 1 << 2
    }
}
