using System;
using UnityEngine;

namespace Cyggie.Plugins.Logs
{
    /// <summary>
    /// The supported log types
    /// </summary>
    [Flags]
    public enum LogTypes
    {
        /// <summary>
        /// None enabled
        /// </summary>
        None = 0,

        /// <summary>
        /// <see cref="Debug.Log(object)"/> enabled
        /// </summary>
        Debug = 1 << 0,

        /// <summary>
        /// <see cref="Debug.LogWarning(object)"/> enabled
        /// </summary>
        Warning = 1 << 1,

        /// <summary>
        /// <see cref="Debug.LogError(object)"/> enabled
        /// </summary>
        Error = 1 << 2
    }
}
