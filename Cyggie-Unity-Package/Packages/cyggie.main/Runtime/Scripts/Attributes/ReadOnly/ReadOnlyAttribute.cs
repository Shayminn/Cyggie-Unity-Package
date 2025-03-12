using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cyggie.Main.Runtime.Attributes
{
    /// <summary>
    /// Custom Attribute for serializing a property in the inspector as read-only
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR

    //
    // This needs to be in here because it needs to stay within the Cyggie.Main.Runtime assembly definition
    //

    /// <summary>
    /// Property drawer for drawing the <see cref="ReadOnlyAttribute"/> as read-only
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    internal class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = false;
        }
    }
#endif
}
