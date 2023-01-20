using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Utils.Extensions;
using Cyggie.SceneChanger.Runtime;
using Cyggie.SceneChanger.Runtime.Settings;
using UnityEditor;
using static UnityEngine.UI.Image;

namespace Cyggie.SceneChanger.Editor.SettingsProviders
{
    /// <summary>
    /// IMGUI to <see cref="SceneChangerSettings"/>
    /// </summary>
    static class SceneChangerSettingsIMGUI
    {
        private static readonly string cSettingsPath = "Cyggie/SceneChanger";
        private static readonly string cSettingsLabel = "Scene Changer";

        // GUI Labels
        private static readonly string cLoadingScreenLabel = "Loading Screen Settings";
        private static readonly string cLoadingBarLabel = "Loading Bar Settings";
        private static readonly string cResolutionLabel = "Resolution Settings";

        /// <summary>
        /// Create a settings provider at Project Settings/Cyggie/SceneChanger
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider(cSettingsPath, SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = cSettingsLabel,

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
            LoadingScreen loadingScreen = settings.LoadingScreenPrefab;

            serializedSettings.Update();

            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.LoadingScreenPrefab)));
                EditorGUILayout.Space(10);
            });

            // Loading Screen Settings
            EditorGUILayout.LabelField(cLoadingScreenLabel, EditorStyles.boldLabel);

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

            EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.Texts)));

            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.MinimumLoadTime)));
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField(cLoadingBarLabel, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.LoadingBarImage)));
            EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.LoadingBarImageColor)));
            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.LoadingBarPosition)));
            EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.LoadingBarSize)));
            EditorGUILayout.Space(5);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.LoadingBarFillMethod)));
            if (EditorGUI.EndChangeCheck())
            {
                serializedSettings.FindProperty(nameof(SceneChangerSettings.LoadingBarFillOrigin)).intValue = 0;
            }

            string label = nameof(SceneChangerSettings.LoadingBarFillOrigin).SplitCamelCase();
            switch (settings.LoadingBarFillMethod)
            {
                case FillMethod.Horizontal:
                    settings.LoadingBarFillOrigin = (int) (OriginHorizontal) EditorGUILayout.EnumPopup(label, (OriginHorizontal) settings.LoadingBarFillOrigin);
                    break;
                case FillMethod.Vertical:
                    settings.LoadingBarFillOrigin = (int) (OriginVertical) EditorGUILayout.EnumPopup(label, (OriginVertical) settings.LoadingBarFillOrigin);
                    break;
                case FillMethod.Radial90:
                    settings.LoadingBarFillOrigin = (int) (Origin90) EditorGUILayout.EnumPopup(label, (Origin90) settings.LoadingBarFillOrigin);
                    break;
                case FillMethod.Radial180:
                    settings.LoadingBarFillOrigin = (int) (Origin180) EditorGUILayout.EnumPopup(label, (Origin180) settings.LoadingBarFillOrigin);
                    break;
                case FillMethod.Radial360:
                    settings.LoadingBarFillOrigin = (int) (Origin360) EditorGUILayout.EnumPopup(label, (Origin360) settings.LoadingBarFillOrigin);
                    break;
            }
            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.PreserveAspectRatio)));
            EditorGUILayout.Space(5);

            SerializedProperty textProgress = serializedSettings.FindProperty(nameof(SceneChangerSettings.EnableTextProgress));
            EditorGUILayout.PropertyField(textProgress);
            EditorGUIHelper.DrawAsReadOnly(!textProgress.boolValue, gui: () =>
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.TextProgressPosition)));
                EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.TextProgressObjectSize)));

                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.TextProgressSize)));
                EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(SceneChangerSettings.TextProgressColor)));
            });
            EditorGUILayout.Space(10);

            // Aspect ratios
            EditorGUILayout.LabelField(cResolutionLabel, EditorStyles.boldLabel);
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

            serializedSettings.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
