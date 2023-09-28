using Cyggie.Main.Runtime.Utils.Enums;
using UnityEngine;

namespace Cyggie.Main.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper class for handling Aspect ratio
    /// </summary>
    public static class AspectRatioHelper
    {
        /// <summary>
        /// Get the aspect ratio based on the current <see cref="Screen.currentResolution.width"/> and <see cref="Screen.currentResolution.height"/>
        /// </summary>
        /// <returns>Aspect ratio in enum value</returns>
        public static AspectRatio GetAspectRatio() => GetAspectRatio(Screen.currentResolution.width, Screen.currentResolution.height);

        /// <summary>
        /// Get the aspect ratio from a width and a height
        /// </summary>
        /// <param name="width">Resolution width in int</param>
        /// <param name="height">Resolution height in int</param>
        /// <returns>Aspect ratio in enum value</returns>
        public static AspectRatio GetAspectRatio(int width, int height) => GetAspectRatio((float) width, height);

        /// <summary>
        /// Get the aspect ratio from a width and a height
        /// </summary>
        /// <param name="width">Resolution width in float</param>
        /// <param name="height">Resolution height in float</param>
        /// <returns>Aspect ratio in enum value</returns>
        public static AspectRatio GetAspectRatio(float width, float height)
        {
            float ratio = Mathf.Floor(width / height * 100) / 100;
            return ratio switch
            {
                0.56f => AspectRatio._9x16,
                0.67f => AspectRatio._2x3,
                1.25f => AspectRatio._5x4,
                1.33f => AspectRatio._4x3,
                1.50f => AspectRatio._3x2,
                1.56f or 1.60f => AspectRatio._16x10,
                1.67f or 1.77f or 1.78f => AspectRatio._16x9,
                2.37f or 2.39f => AspectRatio._21x9,
                _ => AspectRatio._16x9,
            };
        }
    }
}