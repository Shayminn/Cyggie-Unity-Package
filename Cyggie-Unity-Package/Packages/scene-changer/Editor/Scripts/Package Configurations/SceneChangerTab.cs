using Cyggie.Main.Editor.Configurations;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Utils.Extensions;
using Cyggie.SceneChanger.Runtime;
using Cyggie.SceneChanger.Runtime.Configurations;
using Cyggie.SceneChanger.Runtime.Utils.Constants;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Cyggie.SceneChanger.Editor.Configurations
{
    /// <summary>
    /// Package tab for <see cref="SceneChangerSettings"/> <br/>
    /// Accessible through Cyggie/Package Configurations
    /// </summary>
    internal class SceneChangerTab : PackageConfigurationTab
    {
        // GUI Labels
        private const string cLoadingScreenLabel = "Loading Screen Settings";
        private const string cLoadingBarLabel = "Loading Bar Settings";
        private const string cResolutionLabel = "Resolution Settings";

        private bool _loadingScreenFoldout = true;
        private bool _loadingBarFoldout = false;
        private bool _resolutionFoldout = false;

        /// <inheritdoc/>
        internal override System.Type SettingsType => typeof(SceneChangerSettings);

        /// <inheritdoc/>
        internal override string ResourcesPath => SceneChangerSettings.cResourcesPath;

        private SceneChangerSettings Settings => (SceneChangerSettings) _settings;

        internal override void OnSettingsCreated()
        {
            Settings.LoadingScreenPrefab = AssetDatabase.LoadAssetAtPath<LoadingScreen>(SceneChangerPaths.cLoadingScreen);
        }

        /// <inheritdoc/>
        internal override void DrawGUI()
        {
            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingScreenPrefab)));
                EditorGUILayout.Space(10);
            });

            // Loading Screen Settings
            _loadingScreenFoldout = EditorGUILayout.Foldout(_loadingScreenFoldout, cLoadingScreenLabel);

            if (_loadingScreenFoldout)
            {
                // Images
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.Images)));

                // Extra settings if with custom loading screen images
                EditorGUIHelper.DrawAsReadOnly(Settings.Images == null || Settings.Images.Length == 0, gui: () =>
                {
                    EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.ScaleImageToResolution)));
                    EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.RandomizeImages)));

                    // Extra settings if randomized images
                    EditorGUIHelper.DrawAsReadOnly(!Settings.RandomizeImages, gui: () =>
                    {
                        EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.RandomType)));
                    });
                });
                EditorGUILayout.Space(5);

                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.Texts)));
                EditorGUILayout.Space(5);

                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.MinimumLoadTime)));
                EditorGUILayout.Space(10);
            }

            // Loading Bar Settings
            _loadingBarFoldout = EditorGUILayout.Foldout(_loadingBarFoldout, cLoadingBarLabel);

            if (_loadingBarFoldout)
            {
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarImage)));
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarImageColor)));
                EditorGUILayout.Space(5);

                // Manually draw label & field instead, for some reason this is printing in two lines
                //EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarPosition)), GUILayout.MaxWidth(500));
                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUILayout.LabelField("Loading Bar Position", GUILayout.Width(150));
                    Settings.LoadingBarPosition = EditorGUILayout.Vector3Field("", Settings.LoadingBarPosition);
                });

                // Manually draw label & field instead, for some reason this is printing in two lines
                //EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarSize)));
                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUILayout.LabelField("Loading Bar Size", GUILayout.Width(150));
                    Settings.LoadingBarSize = EditorGUILayout.Vector2Field("", Settings.LoadingBarSize);
                });
                EditorGUILayout.Space(5);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarFillMethod)));
                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarFillOrigin)).intValue = 0;
                }

                string label = nameof(SceneChangerSettings.LoadingBarFillOrigin).SplitCamelCase();
                switch (Settings.LoadingBarFillMethod)
                {
                    case FillMethod.Horizontal:
                        Settings.LoadingBarFillOrigin = (int) (OriginHorizontal) EditorGUILayout.EnumPopup(label, (OriginHorizontal) Settings.LoadingBarFillOrigin);
                        break;
                    case FillMethod.Vertical:
                        Settings.LoadingBarFillOrigin = (int) (OriginVertical) EditorGUILayout.EnumPopup(label, (OriginVertical) Settings.LoadingBarFillOrigin);
                        break;
                    case FillMethod.Radial90:
                        Settings.LoadingBarFillOrigin = (int) (Origin90) EditorGUILayout.EnumPopup(label, (Origin90) Settings.LoadingBarFillOrigin);
                        break;
                    case FillMethod.Radial180:
                        Settings.LoadingBarFillOrigin = (int) (Origin180) EditorGUILayout.EnumPopup(label, (Origin180) Settings.LoadingBarFillOrigin);
                        break;
                    case FillMethod.Radial360:
                        Settings.LoadingBarFillOrigin = (int) (Origin360) EditorGUILayout.EnumPopup(label, (Origin360) Settings.LoadingBarFillOrigin);
                        break;
                }
                EditorGUILayout.Space(5);

                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.PreserveAspectRatio)));
                EditorGUILayout.Space(5);

                SerializedProperty textProgress = _serializedObject.FindProperty(nameof(SceneChangerSettings.EnableTextProgress));
                EditorGUILayout.PropertyField(textProgress);
                EditorGUIHelper.DrawAsReadOnly(!textProgress.boolValue, gui: () =>
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.TextProgressPosition)));
                    EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.TextProgressObjectSize)));

                    EditorGUILayout.Space(5);
                    EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.TextProgressSize)));
                    EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.TextProgressColor)));
                });
                EditorGUILayout.Space(10);
            }

            // Aspect ratios
            _resolutionFoldout = EditorGUILayout.Foldout(_resolutionFoldout, cResolutionLabel);

            if (_resolutionFoldout)
            {
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.AutoAdjustToResolution)));

                EditorGUIHelper.DrawAsReadOnly(Settings.AutoAdjustToResolution, gui: () =>
                {
                    EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.ScreenSize)));
                });

                EditorGUIHelper.DrawAsReadOnly(!Settings.AutoAdjustToResolution, gui: () =>
                {
                    EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.ResolutionCheckDelay)));
                });
            }

            EditorGUILayout.Space(10);

            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
