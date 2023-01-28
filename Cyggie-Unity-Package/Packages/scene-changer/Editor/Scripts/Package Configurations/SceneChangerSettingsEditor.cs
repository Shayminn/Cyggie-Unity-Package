using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.SceneChanger.Runtime.Configurations;
using UnityEditor;
using UnityEngine;

namespace Cyggie.SceneChanger.Editor.Configurations
{
    /// <summary>
    /// Service Manager Settings inspector editor
    /// </summary>
    [CustomEditor(typeof(SceneChangerSettings))]
    internal class SceneChangerSettingsEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Override the default inspector GUI to make it readonly
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                base.OnInspectorGUI();
            });
        }
    }
}
