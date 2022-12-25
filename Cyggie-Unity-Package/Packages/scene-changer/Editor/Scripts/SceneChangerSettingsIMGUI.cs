using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.SceneChanger.Runtime;
using UnityEditor;

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
        public static SettingsProvider CreateSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Cyggie/SceneChanger", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Scene Changer",

                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = OnSettingsGUI,

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = SceneChangerSettings.GetKeywords()
            };

            return provider;
        }

        private static void OnSettingsGUI(string searchContext)
        {
            SerializedObject serializedSettings = SceneChangerSettings.SerializedSettings;
            SceneChangerSettings settings = serializedSettings.targetObject as SceneChangerSettings;
            LoadingScreen loadingScreen = settings.LoadingScreen;

            UnityEngine.Debug.Log("On settings gui: " + loadingScreen);
            serializedSettings.Update();

            // Loading Screen Settings
            EditorGUILayout.LabelField("Loading Screen Settings", EditorStyles.boldLabel);

            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                EditorGUILayout.PropertyField(serializedSettings.FindProperty("_loadingScreen"));
            });

            EditorGUILayout.PropertyField(serializedSettings.FindProperty("_textures"));

            // Fade Settings
            EditorGUILayout.LabelField("Fade Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedSettings.FindProperty("_fadeIn"));
            EditorGUILayout.PropertyField(serializedSettings.FindProperty("_fadeOut"));

            serializedSettings.ApplyModifiedPropertiesWithoutUndo();

        }
    }
}
