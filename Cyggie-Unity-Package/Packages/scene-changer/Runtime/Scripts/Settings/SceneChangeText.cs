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
        [SerializeField, Tooltip("")]
        internal string Text = "";
        
        [Header("Transform Settings")]
        [SerializeField, Tooltip("")]
        internal Vector3 Position = Vector3.zero;

        [SerializeField, Tooltip("")]
        internal Vector2 ObjectSize = Vector2.zero;

        [Header("Text Settings")]
        [SerializeField, Tooltip("")]
        internal Color TextColor = Color.white;

        [SerializeField, Tooltip("")]
        internal int TextSize = 36;

        [Header("Visibility Settings")]
        [SerializeField, Tooltip("")]
        internal bool AlwaysVisible = true;

        [SerializeField, Tooltip("")]
        internal int ImageSpecific = -1;

        [SerializeField, Tooltip("")]
        [Range(0, 100)]
        internal float DisplayAtProgress = 0;
    }
}
