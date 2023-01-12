using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [SerializeField, Tooltip("")]
        internal string Text = "";

        [SerializeField, HideInInspector]
        internal bool TransformSettingsFoldOut = true;

        [SerializeField, Tooltip("")]
        internal Vector3 Position = Vector3.zero;

        [SerializeField, Tooltip("")]
        internal Vector2 ObjectSize = new Vector2(500, 150);

        [SerializeField, HideInInspector]

        internal bool TextSettingsFoldOut = true;

        [SerializeField, Tooltip("")]
        internal Color TextColor = Color.white;

        [SerializeField, Tooltip("")]
        internal int TextSize = 36;

        [SerializeField, HideInInspector]

        internal bool VisibilitySettingsFoldOut = true;

        [SerializeField, Tooltip("")]
        internal bool AlwaysVisible = true;

        [SerializeField, Tooltip("")]
        internal int ImageSpecific = -1;

        [SerializeField, Tooltip("")]
        [Range(0, 100)]
        internal float DisplayAtProgress = 0;
    }
}
