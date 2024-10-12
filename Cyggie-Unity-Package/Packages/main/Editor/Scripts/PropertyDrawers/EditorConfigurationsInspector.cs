using Cyggie.Main.Editor.Utils.Constants;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.PropertyDrawers
{
    /// <summary>
    /// Inspector editor for <see cref="EditorConfigurations"/>
    /// </summary>
    [CustomEditor(typeof(EditorConfigurations))]
    public class EditorConfigurationsInspector : UnityEditor.Editor
    {
        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            // Make fields read-only
            EditorGUILayout.LabelField($"Modify these fields in the window at {EditorMenuItemConstants.cEditorConfigurations}");
            EditorGUILayout.Space(5);

            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}
