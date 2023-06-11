using System;

namespace Cyggie.Main.Runtime.Configurations
{
    /// <summary>
    /// Enum flags of the Main package's enabled services
    /// </summary>
    [Flags]
    public enum MainServiceTypes
    {
        Everything = -1,
        None = 0,
        ReferencePool = 1 << 1,
    }
}
