using Cyggie.Main.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.PropertyDrawers
{
    /// <summary>
    /// Property drawer for drawing the <see cref="ReadOnlyAttribute"/> as read-only
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    internal class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = false;
        }
    }
}
