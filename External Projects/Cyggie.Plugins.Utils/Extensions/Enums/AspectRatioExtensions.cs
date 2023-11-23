using Cyggie.Plugins.Utils.Enums;
using UnityEngine;

namespace Cyggie.Plugins.Utils.Extensions
{
    /// <summary>
    /// Extension class for <see cref="AspectRatio"/>
    /// </summary>
    public static class AspectRatioExtensions
    {
        /// <summary>
        /// Get the Vector2 equivalent of an aspect ratio <br/>
        /// where Vector2.x => width and Vector2.y = height
        /// </summary>
        /// <param name="ratio">Aspect ratio to convert</param>
        /// <returns>Vector2 equivalent</returns>
        public static Vector2 ToVector2(this AspectRatio ratio)
        {
            return ratio switch
            {
                AspectRatio._9x16 => new Vector2(9, 16),
                AspectRatio._2x3 => new Vector2(2, 3),
                AspectRatio._16x9 => new Vector2(16, 9),
                AspectRatio._16x10 => new Vector2(16, 10),
                AspectRatio._3x2 => new Vector2(3, 2),
                AspectRatio._4x3 => new Vector2(4, 3),
                AspectRatio._5x4 => new Vector2(5, 4),
                AspectRatio._21x9 => new Vector2(21, 9),
                AspectRatio._32x9 => new Vector2(32, 9),
                _ => Vector2.zero,
            };
        }

        /// <summary>
        /// Get the Vector2 equivalent of an aspect ratio multiplied by <paramref name="multiplier"/> <br/>
        /// where Vector2.x => width, Vector2.y => height
        /// </summary>
        /// <param name="ratio">Aspect ratio to convert</param>
        /// <param name="multiplier">Multiplier to apply</param>
        /// <returns>Vector2 equivalent</returns>
        public static Vector2 ToVector2(this AspectRatio ratio, float multiplier) => ratio.ToVector2() * multiplier;
    }
}
