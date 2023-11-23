#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Cyggie.Plugins.Utils.Helpers
{
    /// <summary>
    /// Helper class for Unity-related stuff
    /// </summary>
    public static class UnityHelper
    {
        /// <summary>
        /// Get the current screen resolution
        /// </summary>
        /// <returns>Resolution struct</returns>
        public static Resolution GetCurrentResolution()
        {
#if UNITY_EDITOR
            string[] res = UnityStats.screenRes.Split('x');
            return new Resolution()
            {
                width = int.Parse(res[0]),
                height = int.Parse(res[1]),
                refreshRate = Screen.currentResolution.refreshRate
            };
#else
            return Screen.currentResolution;
#endif
        }

        /// <summary>
        /// Get the screen size in <see cref="Vector2"/>.
        /// </summary>
        /// <returns>Vector2 of <see cref="Screen.width"/> x <see cref="Screen.height"/></returns>
        public static Vector2 GetVector2ScreenSize() => new Vector2(Screen.width, Screen.height);

        /// <summary>
        /// Get the resolution in <see cref="Vector2"/>.
        /// </summary>
        /// <returns>Vector2 of <see cref="Resolution.width"/> x <see cref="Resolution.height"/></returns>
        public static Vector2 GetVector2Resolution()
        {
            Resolution res = GetCurrentResolution();
            return new Vector2(res.width, res.height);
        }
    }
}
