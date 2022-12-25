using System;
using UnityEditor;
using UnityEngine;

namespace Cyggie.SceneChanger.Editor.Models
{

    [Serializable]
    public class FadeSettings
    {
        [SerializeField, Tooltip("")]
        private float _duration = 0.5f;

        [SerializeField, Tooltip("")]
        private Color _color = Color.black;

        public float Duration => _duration;

        public Color Color => _color;
    }

    [CustomPropertyDrawer(typeof(FadeSettings))]
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
