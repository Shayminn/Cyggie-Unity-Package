using System;
using UnityEngine;

namespace Cyggie.SceneChanger.Runtime.Settings
{
    /// <summary>
    /// Model class for creating Text GameObjects for <see cref="LoadingScreen"/>
    /// </summary>
    [Serializable]
    public class SceneChangeText
    {
        [SerializeField]
        internal bool PropertyFoldout = true;

        [SerializeField, Tooltip("Text value.")]
        public string Text = "Text";

        [SerializeField]
        internal bool TransformSettingsFoldout = true;

        [SerializeField, Tooltip("Position of the text.")]
        public Vector3 Position = Vector3.zero;

        [SerializeField, Tooltip("Transform size of the text.")]
        public Vector2 ObjectSize = new Vector2(500, 150);

        [SerializeField]
        internal bool TextSettingsFoldout = true;

        [SerializeField, Tooltip("Color of the text.")]
        public Color TextColor = Color.white;

        [SerializeField, Tooltip("Size of the text.")]
        public int TextSize = 36;

        [SerializeField, HideInInspector]
        internal bool VisibilitySettingsFoldout = true;

        [SerializeField, Tooltip("Whether the text is always visible.")]
        public bool AlwaysVisible = true;

        [SerializeField, Tooltip("Index of the image that this text should always apply to.")]
        public int ImageSpecific = -1;

        [SerializeField, Tooltip("Loading % progress to display this text.")]
        [Range(0, 100)]
        public float DisplayAtProgress = 0;
    }
}
