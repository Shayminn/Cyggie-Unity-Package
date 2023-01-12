using Cyggie.SceneChanger.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Cyggie.SceneChanger.Editor.PropertyDrawers
{
    /// <summary>
    /// Property drawer for <see cref="SceneChangeText"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneChangeText))]
    internal class SceneChangeTextPropertyDrawer : PropertyDrawer
    {
        private const string cTransformSettings = "Transform Settings";
        private const string cTextSettings = "Text Settings";
        private const string cVisibilitySettings = "Visibility Settings";

        /// <summary>
        /// Override property height depending on foldouts
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;
            if (property.FindPropertyRelative(nameof(SceneChangeText.PropertyFoldout)).boolValue)
            {
                height += 20; // for text property
                height += 60; // for each category
                height += 15; // for extra spacing

                if (property.FindPropertyRelative(nameof(SceneChangeText.TransformSettingsFoldOut)).boolValue)
                {
                    height += 40;
                }

                if (property.FindPropertyRelative(nameof(SceneChangeText.TextSettingsFoldOut)).boolValue)
                {
                    height += 40;
                }

                if (property.FindPropertyRelative(nameof(SceneChangeText.VisibilitySettingsFoldOut)).boolValue)
                {
                    if (!property.FindPropertyRelative(nameof(SceneChangeText.AlwaysVisible)).boolValue)
                    {
                        height += 20;
                    }

                    height += 40;
                }
            }

            return height + 20;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.x = 25;

            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty propertyFoldout = property.FindPropertyRelative(nameof(SceneChangeText.PropertyFoldout));
            propertyFoldout.boolValue = EditorGUI.Foldout(new Rect(position.x, position.y, 500, 20), propertyFoldout.boolValue, property.FindPropertyRelative(nameof(SceneChangeText.Text)).stringValue);

            if (!propertyFoldout.boolValue) return;

            position.x += 15;
            position.y += 20;

            // Draw text field
            EditorGUI.PropertyField(new Rect(position.x, position.y, 500, 20), property.FindPropertyRelative(nameof(SceneChangeText.Text)));
            position.y += 20;


            // Transform settings
            SerializedProperty transformSettingsFoldout = property.FindPropertyRelative(nameof(SceneChangeText.TransformSettingsFoldOut));
            transformSettingsFoldout.boolValue = EditorGUI.Foldout(new Rect(position.x, position.y, 200, 20), transformSettingsFoldout.boolValue, cTransformSettings, true);
            position.y += 20;

            if (transformSettingsFoldout.boolValue)
            {
                position.x += 15;
                EditorGUI.PropertyField(new Rect(position.x, position.y, 500, 20), property.FindPropertyRelative(nameof(SceneChangeText.Position)));

                position.y += 20;
                EditorGUI.PropertyField(new Rect(position.x, position.y, 500, 20), property.FindPropertyRelative(nameof(SceneChangeText.ObjectSize)));

                position.x -= 15;
                position.y += 25;
            }

            // Text settings
            SerializedProperty textSettingsFoldout = property.FindPropertyRelative(nameof(SceneChangeText.TextSettingsFoldOut));
            textSettingsFoldout.boolValue = EditorGUI.Foldout(new Rect(position.x, position.y, 200, 20), textSettingsFoldout.boolValue, cTextSettings, true);
            position.y += 20;

            if (textSettingsFoldout.boolValue)
            {
                position.x += 15;
                EditorGUI.PropertyField(new Rect(position.x, position.y, 500, 20), property.FindPropertyRelative(nameof(SceneChangeText.TextColor)));

                position.y += 22.5f;
                EditorGUI.PropertyField(new Rect(position.x, position.y, 500, 20), property.FindPropertyRelative(nameof(SceneChangeText.TextSize)));

                position.x -= 15;
                position.y += 25;
            }

            // Visibility settings
            SerializedProperty visibilitySettingsFoldout = property.FindPropertyRelative(nameof(SceneChangeText.VisibilitySettingsFoldOut));
            visibilitySettingsFoldout.boolValue = EditorGUI.Foldout(new Rect(position.x, position.y, 200, 20), visibilitySettingsFoldout.boolValue, cVisibilitySettings, true);
            position.y += 20;

            if (visibilitySettingsFoldout.boolValue)
            {
                position.x += 15;

                SerializedProperty alwaysVisibleProperty = property.FindPropertyRelative(nameof(SceneChangeText.AlwaysVisible));
                EditorGUI.PropertyField(new Rect(position.x, position.y, 500, 20), alwaysVisibleProperty);

                if (!alwaysVisibleProperty.boolValue)
                {
                    position.y += 20;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, 500, 20), property.FindPropertyRelative(nameof(SceneChangeText.ImageSpecific)));
                }

                position.y += 20;
                EditorGUI.PropertyField(new Rect(position.x, position.y, 500, 20), property.FindPropertyRelative(nameof(SceneChangeText.DisplayAtProgress)));

                position.x -= 15;
            }

            EditorGUI.EndProperty();
        }
    }
}
