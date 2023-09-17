using Cyggie.Main.Editor.Utils.Extensions;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Editor.Utils.Styles;
using Cyggie.Main.Runtime.Serializations;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.PropertyDrawers
{
    /// <summary>
    /// Property drawer for a serialized dictionary
    /// </summary>
    [CustomPropertyDrawer(typeof(AbstractSerializedDictionary), true)]
    public class SerializedDictionaryPropertyDrawer : PropertyDrawer
    {
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Height for label and foldout
            float height = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                // +20 for headers (Keys Values)
                // +20 for add new field (and button)
                height += EditorGUIUtility.singleLineHeight * 2;

                SerializedProperty keys = property.FindPropertyRelative("_keys");

                for (int i = 0; i < keys.arraySize; i++)
                {
                    SerializedProperty keyProperty = keys.GetArrayElementAtIndex(i);
                    height += EditorGUI.GetPropertyHeight(keyProperty);
                }

                if (keys.arraySize > 0)
                {
                    // +20 for Clear button
                    // +5 for spacing between Clear buttons and values
                    height += EditorGUIUtility.singleLineHeight + 5;
                }
                else
                {
                    // +20 for empty space
                    height += EditorGUIUtility.singleLineHeight;
                }
            }

            return height;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty addKey = property.FindPropertyRelative("_addKey");
            SerializedProperty keys = property.FindPropertyRelative("_keys");
            SerializedProperty values = property.FindPropertyRelative("_values");

            EditorGUI.BeginProperty(position, label, property);

            // Draw dictionary label and foldout
            label.text = $"{label.text} ({keys.arraySize})"; // add the number of elements in the property label
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property, label);

            if (property.isExpanded)
            {
                float keyWidth = position.width / 2.5f;
                float valueWidth = position.width - keyWidth - 10 - 20 - 20 - 10; // -10 for spacing, -20 for indentation, -10 for the remove button, -10 for more spacing
                bool valueExists = false;

                // Add indentation
                position.x += 20;

                // Draw headers (Keys   Values)
                position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), new GUIContent($"Keys"), EditorStyles.boldLabel);

                position.x += keyWidth + 10; // 10 for some extra spacing
                EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), new GUIContent($"Values"), EditorStyles.boldLabel);
                position.x -= keyWidth + 10;

                // Draw every line of keys and values
                for (int i = 0; i < keys.arraySize; i++)
                {
                    position.y += EditorGUIUtility.singleLineHeight;

                    // Draw the key field
                    SerializedProperty keyProperty = keys.GetArrayElementAtIndex(i);
                    float keyHeight = EditorGUI.GetPropertyHeight(keyProperty);
                    GUI.enabled = false;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, keyWidth, keyHeight), keyProperty, GUIContent.none, includeChildren: true);
                    GUI.enabled = true;

                    // Draw the value field
                    position.x += keyWidth + 10; // 10 for some extra spacing
                    SerializedProperty valueProperty = values.GetArrayElementAtIndex(i);
                    float valueHeight = EditorGUI.GetPropertyHeight(valueProperty);
                    EditorGUI.PropertyField(new Rect(position.x, position.y, valueWidth, valueHeight), valueProperty, GUIContent.none, includeChildren: true);

                    // Draw the remove button
                    position.x += valueWidth + 10; // 10 for some extra spacing
                    if (GUI.Button(new Rect(position.x, position.y, 20, EditorGUIUtility.singleLineHeight), " - "))
                    {
                        keys.DeleteArrayElementAtIndex(i);
                        values.DeleteArrayElementAtIndex(i);
                    }
                    else
                    {
                        // Check if the add key is the same as one of our existing keys
                        if (!valueExists)
                        {
                            if (SerializedPropertyHelper.IsEqual(addKey, keyProperty))
                            {
                                valueExists = true;
                            }
                        }
                    }

                    // Revert the position x to the original value
                    position.x -= keyWidth + 10 + valueWidth + 10;
                }

                if (keys.arraySize > 0)
                {
                    // Draw the clear button on bottom right to remove all entries
                    float offsetX = position.width - 60 - 20; // 60 for button size, 20 for indent    
                    position.x += offsetX;
                    position.y += EditorGUIUtility.singleLineHeight + 5;
                    if (GUI.Button(new Rect(position.x, position.y, 60, EditorGUIUtility.singleLineHeight), "Clear"))
                    {
                        keys.ClearArray();
                        values.ClearArray();
                    }
                    position.x -= offsetX;
                }
                else
                {
                    position.y += EditorGUIUtility.singleLineHeight;
                }

                // Draw add field and button
                position.y += EditorGUIUtility.singleLineHeight;

                object addValue = addKey.GetValue();
                bool isReadOnly = addValue == null || addValue.Equals(null) || addValue.Equals("") || valueExists;

                float addKeyWidth = position.width / 2.5f;

                if (addValue == null)
                {
                    GUIHelper.DrawWithColor(Color.red, gui: () =>
                    {
                        EditorGUI.LabelField(new Rect(position.x, position.y, addKeyWidth, EditorGUIUtility.singleLineHeight), GUIContents.cUnsupportedType);
                    });
                }
                else
                {
                    EditorGUI.PropertyField(new Rect(position.x, position.y, addKeyWidth, EditorGUIUtility.singleLineHeight), addKey, GUIContent.none, includeChildren: true);
                }

                position.x += addKeyWidth + 10;
                GUIHelper.DrawAsReadOnly(isReadOnly, gui: () =>
                {
                    if (GUI.Button(new Rect(position.x, position.y, 50, EditorGUIUtility.singleLineHeight), "Add"))
                    {
                        keys.arraySize++;
                        SerializedProperty newKey = keys.GetArrayElementAtIndex(keys.arraySize - 1); // get last element
                        SerializedPropertyHelper.Copy(addKey, newKey);

                        values.arraySize++;
                        addKey.ClearValue();
                    }
                });

                if (valueExists && addValue != null)
                {
                    float offsetX = 50 + 10; // 50 for button size, 10 for spacing
                    position.x += offsetX;
                    GUIHelper.DrawWithColor(Color.red, gui: () =>
                    {
                        EditorGUI.LabelField(new Rect(position.x, position.y, 200, EditorGUIUtility.singleLineHeight), GUIContents.cKeyAlreadyExists);
                    });
                    position.x -= offsetX;
                }

                position.x -= 50 + 10;
            }

            EditorGUI.EndProperty();
        }
    }
}
