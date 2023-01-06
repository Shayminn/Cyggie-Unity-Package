using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.SceneChanger.Runtime;
using Cyggie.SceneChanger.Runtime.Settings;
using UnityEditor;

namespace Cyggie.SceneChanger.Editor
{
    /// <summary>
    /// IMGUI to <see cref="SceneChangerSettings"/>
    /// </summary>
    static class SceneChangerSettingsIMGUI
    {
        /// <summary>
        /// Create a settings provider at Project Settings/Cyggie/SceneChanger
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

        /// <summary>
        /// GUI Handler for creating the settings UI
        /// </summary>
        private static void OnSettingsGUI(string _)
        {
            SerializedObject serializedSettings = SceneChangerSettings.SerializedSettings;
            SceneChangerSettings settings = serializedSettings.targetObject as SceneChangerSettings;
            LoadingScreen loadingScreen = settings.LoadingScreen;
            bool textsUpdated = false;

            serializedSettings.Update();

            // Loading Screen Settings
            EditorGUILayout.LabelField("Loading Screen Settings", EditorStyles.boldLabel);

            // Images
            EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.Images)));

            // Extra settings if with custom loading screen images
            EditorGUIHelper.DrawAsReadOnly(settings.Images == null || settings.Images.Length == 0, gui: () =>
            {
                EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.ScaleImageToResolution)));
                EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.RandomizeImages)));

                // Extra settings if randomized images
                EditorGUIHelper.DrawAsReadOnly(!settings.RandomizeImages, gui: () =>
                {
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.RandomType)));
                });
            });
            EditorGUILayout.Space(5);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.Texts)));
            textsUpdated = EditorGUI.EndChangeCheck();

            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.MinimumLoadTime)));
            EditorGUILayout.Space(10);

            // Aspect ratios
            EditorGUILayout.LabelField("Resolution Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.AutoAdjustToResolution)));

            EditorGUIHelper.DrawAsReadOnly(settings.AutoAdjustToResolution, gui: () =>
            {
                EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.ScreenSize)));
            });

            EditorGUIHelper.DrawAsReadOnly(!settings.AutoAdjustToResolution, gui: () =>
            {
                EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.ResolutionCheckDelay)));
            });

            EditorGUILayout.Space(10);

            // Fade Settings
            EditorGUILayout.LabelField("Fade Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            serializedSettings.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
