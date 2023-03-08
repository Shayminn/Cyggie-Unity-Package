using System;
using UnityEngine;

namespace Cyggie.SceneChanger.Runtime.Settings
{
    /// <summary>
    /// Model class for creating Text GameObjects for <see cref="LoadingScreen"/>
    /// </summary>
    [Serializable]
    internal class SceneChangeText
    {
        [SerializeField, HideInInspector]
        internal bool PropertyFoldout = true;

        [SerializeField, Tooltip("Text to write.")]
        internal string Text = "Text";

        [SerializeField, HideInInspector]
        internal bool TransformSettingsFoldOut = true;

        [SerializeField, Tooltip("RectTransform's position.")]
        internal Vector3 Position = Vector3.zero;

        [SerializeField, Tooltip("RectTransform's width and height.")]
        internal Vector2 ObjectSize = new Vector2(500, 150);

        [SerializeField, HideInInspector]
        internal bool TextSettingsFoldOut = true;

        [SerializeField, Tooltip("The color of the Text.")]
        internal Color TextColor = Color.white;

        [SerializeField, Tooltip("The font size of the Text.")]
        internal int TextSize = 36;

        [SerializeField, HideInInspector]
        internal bool VisibilitySettingsFoldOut = true;

        [SerializeField, Tooltip("Whether this Text should always be visible.")]
        internal bool AlwaysVisible = true;

        [SerializeField, Tooltip("Whether this Text should only be visible to ")]
        internal int ImageSpecific = -1;

        [SerializeField, Tooltip("Whether this Text should only be displayed when the loading has reached a specific progress.")]
        [Range(0, 100)]
        internal float DisplayAtProgress = 0;
    }
}
