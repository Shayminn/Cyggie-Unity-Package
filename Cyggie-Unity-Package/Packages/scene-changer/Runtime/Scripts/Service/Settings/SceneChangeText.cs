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
        [SerializeField]
        internal bool PropertyFoldout = true;

        [SerializeField]
        internal string Text = "Text";

        [SerializeField]
        internal bool TransformSettingsFoldout = true;

        [SerializeField]
        internal Vector3 Position = Vector3.zero;

        [SerializeField]
        internal Vector2 ObjectSize = new Vector2(500, 150);

        [SerializeField]
        internal bool TextSettingsFoldout = true;

        [SerializeField]
        internal Color TextColor = Color.white;

        [SerializeField]
        internal int TextSize = 36;

        [SerializeField, HideInInspector]
        internal bool VisibilitySettingsFoldout = true;

        [SerializeField]
        internal bool AlwaysVisible = true;

        [SerializeField]
        internal int ImageSpecific = -1;

        [SerializeField]
        [Range(0, 100)]
        internal float DisplayAtProgress = 0;
    }
}
