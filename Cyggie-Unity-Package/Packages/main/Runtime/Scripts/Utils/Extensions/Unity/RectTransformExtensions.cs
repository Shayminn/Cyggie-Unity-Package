using Cyggie.Main.Runtime.Utils.Enums;
using UnityEngine;

namespace Cyggie.Main.Runtime.Utils.Extensions
{
    /// <summary>
    /// Extension class to <see cref="RectTransform"/>
    /// </summary>
    public static class RectTransformExtensions
    {
        /// <summary>
        /// Set anchor type of <paramref name="rectTransform"/> <br/>
        /// Assign the right <see cref="RectTransform.anchorMin"/> and <see cref="RectTransform.anchorMin"/> based on <paramref name="anchorType"/>
        /// </summary>
        /// <param name="rectTransform">Rect transform to change</param>
        /// <param name="anchorType">Anchor type to set</param>
        public static void SetAnchorType(this RectTransform rectTransform, RectTransformAnchorType anchorType)
        {
            Vector2 min = Vector2.zero;
            Vector2 max = Vector2.zero;

            string anchorTypeStr = anchorType.ToString();

            if (anchorTypeStr.StartsWith("Stretch"))
            {
                max.x = 1f;
            }

            if (anchorTypeStr.Contains("Top"))
            {
                min.y += 1;
                max.y += 1;
            }

            if (anchorTypeStr.Contains("Middle"))
            {
                min.y += 0.5f;
                max.y += 0.5f;
            }

            if (anchorTypeStr.Contains("Center"))
            {
                min.x += 0.5f;
                max.x += 0.5f;
            }

            if (anchorTypeStr.Contains("Right"))
            {
                min.x += 1f;
                max.x += 1f;
            }

            if (anchorTypeStr.EndsWith("Stretch"))
            {
                max.y = 1f;
            }

            rectTransform.anchorMin = min;
            rectTransform.anchorMax = max;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        /// <summary>
        /// Set anchor type of <paramref name="rectTransform"/> <br/>
        /// Assign the right <see cref="RectTransform.anchorMin"/> and <see cref="RectTransform.anchorMin"/> based on <paramref name="anchorType"/>
        /// </summary>
        /// <param name="rectTransform">Rect transform to change</param>
        /// <param name="anchorType">Anchor type to set</param>
        /// <param name="width">Width to assign to <paramref name="rectTransform"/></param>
        /// <param name="height">Height to assign to <paramref name="rectTransform"/></param>
        public static void SetAnchorType(this RectTransform rectTransform, RectTransformAnchorType anchorType, float width, float height)
        {
            Vector2 min = Vector2.zero;
            Vector2 max = Vector2.zero;

            string anchorTypeStr = anchorType.ToString();

            if (anchorTypeStr.StartsWith("Stretch"))
            {
                max.x = 1f;
            }

            if (anchorTypeStr.Contains("Top"))
            {
                min.y += 1;
                max.y += 1;
            }

            if (anchorTypeStr.Contains("Middle"))
            {
                min.y += 0.5f;
                max.y += 0.5f;
            }

            if (anchorTypeStr.Contains("Center"))
            {
                min.x += 0.5f;
                max.x += 0.5f;
            }

            if (anchorTypeStr.Contains("Right"))
            {
                min.x += 1f;
                max.x += 1f;
            }

            if (anchorTypeStr.EndsWith("Stretch"))
            {
                max.y = 1f;
            }

            rectTransform.anchorMin = min;
            rectTransform.anchorMax = max;
            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }
    }
}
