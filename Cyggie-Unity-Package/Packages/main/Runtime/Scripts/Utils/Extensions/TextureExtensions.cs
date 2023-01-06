using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cyggie.Main.Runtime.Utils.Extensions
{
    /// <summary>
    /// Extension class to <see cref="Texture"/> and <see cref="Texture2D"/>
    /// </summary>
    public static class TextureExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Sprite ToSprite(this Texture2D texture) => Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
