using Cyggie.Utils.Helpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cyggie.SceneChanger.Editor
{
    /// <summary>
    /// 
    /// </summary>
    static class SceneChangerSettingsIMGUI
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Cyggie/SceneChanger", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Scene Changer",

                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = SceneChangerSettings.SerializedSettings;
                    EditorGUILayout.PropertyField(settings.FindProperty("m_Number"), new GUIContent("My Number"));
                    EditorGUILayout.PropertyField(settings.FindProperty("m_SomeString"), new GUIContent("My String"));
                    settings.ApplyModifiedPropertiesWithoutUndo();
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Number", "Some String" })
            };

            return provider;
        }
    }
}
