using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.SceneChanger.Editor.Utils.Styles;
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

        private SerializedProperty _propertyFoldout = null;
        private SerializedProperty _textSettingsFoldout = null;
        private SerializedProperty _transformSettingsFoldout = null;
        private SerializedProperty _visibilitySettingsFoldout = null;

        private SerializedProperty _text = null;
        private SerializedProperty _position = null;
        private SerializedProperty _objectSize = null;
        private SerializedProperty _textColor = null;
        private SerializedProperty _textSize = null;
        private SerializedProperty _alwaysVisible = null;
        private SerializedProperty _imageSpecific = null;
        private SerializedProperty _displayAtProgress = null;

        /// <summary>
        /// Override property height depending on foldouts
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SetProperties(property);

            float height = 0;
            if (_propertyFoldout.boolValue)
            {
                height += 20; // for text property
                height += 60; // for each category
                height += 15; // for extra spacing

                if (_transformSettingsFoldout.boolValue)
                {
                    height += 40;
                }

                if (_textSettingsFoldout.boolValue)
                {
                    height += 40;
                }

                if (_visibilitySettingsFoldout.boolValue)
                {
                    if (!_alwaysVisible.boolValue)
                    {
                        height += 20;
                    }

                    height += 45;
                }
            }

            return height + 20;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SetProperties(property);

            position.x = 25;

            EditorGUI.BeginProperty(position, label, property);

            _propertyFoldout.boolValue = EditorGUI.Foldout(new Rect(position.x, position.y, 500, 20), _propertyFoldout.boolValue, _text.stringValue);

            if (!_propertyFoldout.boolValue) return;

            EditorGUIUtility.labelWidth = 165;
            position.x += 15;
            position.y += 20;

            // Draw text field
            EditorGUI.PropertyField(new Rect(position.x, position.y, 650, 20), _text, GUIContents.cText);
            position.y += 20;

            // Transform settings
            _transformSettingsFoldout.boolValue = EditorGUI.Foldout(new Rect(position.x, position.y, 300, 20), _transformSettingsFoldout.boolValue, cTransformSettings, true);
            position.y += 20;

            if (_transformSettingsFoldout.boolValue)
            {
                EditorGUIUtility.labelWidth = 150;
                position.x += 15;

                // Manually draw label & field instead, for some reason this is printing in two lines
                //EditorGUI.PropertyField(new Rect(position.x, position.y, 500, 20), property.FindPropertyRelative(nameof(SceneChangeText.Position)));
                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUI.LabelField(new Rect(position.x, position.y, 150, 20), GUIContents.cPosition);
                    position.x += 150;
                    _position.vector3Value = EditorGUI.Vector3Field(new Rect(position.x, position.y, 485, 20), "", _position.vector3Value);
                    position.x -= 150;
                });

                position.y += 20;

                // Manually draw label & field instead, for some reason this is printing in two lines
                //EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarSize)));
                //EditorGUI.PropertyField(new Rect(position.x, position.y, 500, 20), property.FindPropertyRelative(nameof(SceneChangeText.ObjectSize)));
                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUI.LabelField(new Rect(position.x, position.y, 120, 20), GUIContents.cObjectSize);
                    position.x += 150;
                    _objectSize.vector2Value = EditorGUI.Vector2Field(new Rect(position.x, position.y, 485, 20), "", _objectSize.vector2Value);
                    position.x -= 150;
                });

                position.x -= 15;
                position.y += 25;
            }

            // Text settings
            _textSettingsFoldout.boolValue = EditorGUI.Foldout(new Rect(position.x, position.y, 300, 20), _textSettingsFoldout.boolValue, cTextSettings, true);
            position.y += 20;

            if (_textSettingsFoldout.boolValue)
            {
                position.x += 15;
                EditorGUI.PropertyField(new Rect(position.x, position.y, 635, 20), _textColor, GUIContents.cTextColor);

                position.y += 22.5f;
                EditorGUI.PropertyField(new Rect(position.x, position.y, 635, 20), _textSize, GUIContents.cTextSize);

                position.x -= 15;
                position.y += 25;
            }

            // Visibility settings
            _visibilitySettingsFoldout.boolValue = EditorGUI.Foldout(new Rect(position.x, position.y, 300, 20), _visibilitySettingsFoldout.boolValue, cVisibilitySettings, true);
            position.y += 20;

            if (_visibilitySettingsFoldout.boolValue)
            {
                position.x += 15;

                EditorGUI.PropertyField(new Rect(position.x, position.y, 635, 20), _alwaysVisible, GUIContents.cAlwaysVisible);

                if (!_alwaysVisible.boolValue)
                {
                    position.y += 20;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, 635, 20), _imageSpecific, GUIContents.cImageSpecific);
                }

                position.y += 25;
                EditorGUI.PropertyField(new Rect(position.x, position.y, 635, 20), _displayAtProgress, GUIContents.cDisplayAtProgress);

                position.x -= 15;
            }

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Set all the serialized properties of <see cref="ScenechangeText"/>
        /// </summary>
        /// <param name="property">Base property</param>
        private void SetProperties(SerializedProperty property)
        {
            _propertyFoldout = property.FindPropertyRelative(nameof(SceneChangeText.PropertyFoldout));
            _textSettingsFoldout = property.FindPropertyRelative(nameof(SceneChangeText.TextSettingsFoldout));
            _transformSettingsFoldout = property.FindPropertyRelative(nameof(SceneChangeText.TransformSettingsFoldout));
            _visibilitySettingsFoldout = property.FindPropertyRelative(nameof(SceneChangeText.VisibilitySettingsFoldout));

            _text = property.FindPropertyRelative(nameof(SceneChangeText.Text));
            _position = property.FindPropertyRelative(nameof(SceneChangeText.Position));
            _objectSize = property.FindPropertyRelative(nameof(SceneChangeText.ObjectSize));
            _textColor = property.FindPropertyRelative(nameof(SceneChangeText.TextColor));
            _textSize = property.FindPropertyRelative(nameof(SceneChangeText.TextSize));

            _alwaysVisible = property.FindPropertyRelative(nameof(SceneChangeText.AlwaysVisible));
            _imageSpecific = property.FindPropertyRelative(nameof(SceneChangeText.ImageSpecific));
            _displayAtProgress = property.FindPropertyRelative(nameof(SceneChangeText.DisplayAtProgress));
        }
    }
}
