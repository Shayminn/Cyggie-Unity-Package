using UnityEngine;

namespace Cyggie.Main.Runtime.Utils.Extensions
{
    /// <summary>
    /// Extension class to <see cref="Texture"/> and <see cref="Texture2D"/>
    /// </summary>
    public static class TextureExtensions
    {
        /// <summary>
        /// Convert a texture 2d to a sprite
        /// </summary>
        /// <param name="texture">Texture 2d to convert</param>
        /// <returns>Sprite equivalent</returns>
        public static Sprite ToSprite(this Texture2D texture) => Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
