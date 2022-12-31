using UnityEngine;

namespace Cyggie.Main.Runtime.Utils.Extensions
{
    /// <summary>
    /// Extension class to <see cref="UnityEngine.Color"/>
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Change the alpha (<see cref="Color.a"/>) of <paramref name="color"/> to <paramref name="newAlpha"/>
        /// </summary>
        /// <param name="color">Color struct to change</param>
        /// <param name="newAlpha">New alpha value to <paramref name="color"/></param>
        /// <returns><paramref name="color"/> with alpha value of <paramref name="newAlpha"/></returns>
        public static Color ChangeAlpha(this Color color, float newAlpha) => new Color(color.r, color.g, color.b, newAlpha);
    }
}
