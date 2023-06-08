using UnityEngine;

namespace Cyggie.Main.Runtime.Utils.Extensions
{
    /// <summary>
    /// Extension class for <see cref="LogTypes"/>
    /// </summary>
    internal static class LogTypesExtensions
    {
        /// <summary>
        /// Convert <see cref="LogTypes"/> to the Unity equivalent
        /// </summary>
        /// <param name="logTypes">Log type to convert</param>
        /// <returns></returns>
        internal static LogType ToUnityType(this LogTypes logTypes)
        {
            return logTypes switch
            {
                LogTypes.Debug => LogType.Log,
                LogTypes.Warning => LogType.Warning,
                LogTypes.Error => LogType.Error,
                _ => LogType.Log
            };
        }
    }
}
