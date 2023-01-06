using Cyggie.SceneChanger.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Cyggie.SceneChanger.Editor
{
    /// <summary>
    /// Property drawer for <see cref="ChangeSceneFade"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(ChangeSceneFade))]
    public class FadeSettingsEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw property label
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Draw class' properties
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_duration"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_color"));

            EditorGUI.EndProperty();
        }
    }
}
