using Cyggie.Main.Editor.Configurations;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Utils.Extensions;
using Cyggie.SceneChanger.Editor.Utils;
using Cyggie.SceneChanger.Editor.Utils.Styles;
using Cyggie.SceneChanger.Runtime;
using Cyggie.SceneChanger.Runtime.Configurations;
using Cyggie.SceneChanger.Runtime.ServicesNS;
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
    internal class SceneChangerTab : PackageConfigurationTab<SceneChangerService, SceneChangerSettings>
    {
        // GUI Labels
        private const string cLoadingScreenLabel = "Loading Screen Settings";
        private const string cLoadingBarLabel = "Loading Bar Settings";
        private const string cResolutionLabel = "Resolution Settings";

        private bool _loadingScreenFoldout = true;
        private bool _loadingBarFoldout = false;
        private bool _resolutionFoldout = false;

        #region Serialized Properties

        private SerializedProperty _loadingScreenPrefab = null;

        private SerializedProperty _images = null;
        private SerializedProperty _scaleImageToResolution = null;
        private SerializedProperty _randomizeImages = null;
        private SerializedProperty _randomType = null;
        private SerializedProperty _texts = null;
        private SerializedProperty _minimumLoadTime = null;

        private SerializedProperty _loadingBarImage = null;
        private SerializedProperty _loadingBarImageColor = null;
        private SerializedProperty _loadingBarPosition = null;
        private SerializedProperty _loadingBarSize = null;
        private SerializedProperty _loadingBarFillMethod = null;
        private SerializedProperty _loadingBarFillOrigin = null;
        private SerializedProperty _preserveAspectRatio = null;
        private SerializedProperty _enableTextProgress = null;
        private SerializedProperty _textProgressPosition = null;
        private SerializedProperty _textProgressObjectSize = null;
        private SerializedProperty _textProgressSize = null;
        private SerializedProperty _textProgressColor = null;

        private SerializedProperty _autoAdjustToResolution = null;
        private SerializedProperty _screenSize = null;
        private SerializedProperty _resolutionCheckDelay = null;

        #endregion

        private SceneChangerSettings Settings => (SceneChangerSettings) _settings;

        /// <inheritdoc/>
        protected override void OnSettingsCreated()
        {
            Settings.LoadingScreenPrefab = AssetDatabase.LoadAssetAtPath<LoadingScreen>(SceneChangerPaths.cLoadingScreenPrefab);
            EditorUtility.SetDirty(Settings);
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            _loadingScreenPrefab = _serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingScreenPrefab));

            _images = _serializedObject.FindProperty(nameof(SceneChangerSettings.Images));
            _scaleImageToResolution = _serializedObject.FindProperty(nameof(SceneChangerSettings.ScaleImageToResolution));
            _randomizeImages = _serializedObject.FindProperty(nameof(SceneChangerSettings.RandomizeImages));
            _randomType = _serializedObject.FindProperty(nameof(SceneChangerSettings.RandomType));
            _texts = _serializedObject.FindProperty(nameof(SceneChangerSettings.Texts));
            _minimumLoadTime = _serializedObject.FindProperty(nameof(SceneChangerSettings.MinimumLoadTime));

            _loadingBarImage = _serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarImage));
            _loadingBarImageColor = _serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarImageColor));
            _loadingBarPosition = _serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarPosition));
            _loadingBarSize = _serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarSize));
            _loadingBarFillMethod = _serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarFillMethod));
            _loadingBarFillOrigin = _serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarFillOrigin));
            _preserveAspectRatio = _serializedObject.FindProperty(nameof(SceneChangerSettings.PreserveAspectRatio));
            _enableTextProgress = _serializedObject.FindProperty(nameof(SceneChangerSettings.EnableTextProgress));
            _textProgressPosition = _serializedObject.FindProperty(nameof(SceneChangerSettings.TextProgressPosition));
            _textProgressObjectSize = _serializedObject.FindProperty(nameof(SceneChangerSettings.TextProgressObjectSize));
            _textProgressSize = _serializedObject.FindProperty(nameof(SceneChangerSettings.TextProgressSize));
            _textProgressColor = _serializedObject.FindProperty(nameof(SceneChangerSettings.TextProgressColor));

            _autoAdjustToResolution = _serializedObject.FindProperty(nameof(SceneChangerSettings.AutoAdjustToResolution));
            _screenSize = _serializedObject.FindProperty(nameof(SceneChangerSettings.ScreenSize));
            _resolutionCheckDelay = _serializedObject.FindProperty(nameof(SceneChangerSettings.ResolutionCheckDelay));
        }

        /// <inheritdoc/>
        protected override void DrawGUI()
        {
            GUIHelper.DrawAsReadOnly(gui: () =>
            {
                EditorGUILayout.PropertyField(_loadingScreenPrefab, GUIContents.cLoadingScreenPrefab);
                EditorGUILayout.Space(10);
            });

            // Loading Screen Settings
            _loadingScreenFoldout = EditorGUILayout.Foldout(_loadingScreenFoldout, cLoadingScreenLabel);

            if (_loadingScreenFoldout)
            {
                // Images
                EditorGUILayout.PropertyField(_images, GUIContents.cImages);

                // Extra settings if with custom loading screen images
                GUIHelper.DrawAsReadOnly(Settings.Images == null || Settings.Images.Length == 0, gui: () =>
                {
                    EditorGUILayout.PropertyField(_scaleImageToResolution, GUIContents.cScaleImageToResolution);
                    EditorGUILayout.PropertyField(_randomizeImages, GUIContents.cRandomizeImages);

                    // Extra settings if randomized images
                    GUIHelper.DrawAsReadOnly(!Settings.RandomizeImages, gui: () =>
                    {
                        EditorGUILayout.PropertyField(_randomType, GUIContents.cRandomType);
                    });
                });
                EditorGUILayout.Space(5);

                EditorGUILayout.PropertyField(_texts, GUIContents.cTexts);
                EditorGUILayout.Space(5);

                EditorGUILayout.PropertyField(_minimumLoadTime, GUIContents.cMinimumLoadTime);
                EditorGUILayout.Space(10);
            }

            // Loading Bar Settings
            _loadingBarFoldout = EditorGUILayout.Foldout(_loadingBarFoldout, cLoadingBarLabel);

            if (_loadingBarFoldout)
            {
                EditorGUILayout.PropertyField(_loadingBarImage, GUIContents.cLoadingBarImage);
                EditorGUILayout.PropertyField(_loadingBarImageColor, GUIContents.cLoadingBarImageColor);
                EditorGUILayout.Space(5);

                // Manually draw label & field instead, for some reason this is printing in two lines
                //EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarPosition)), GUILayout.MaxWidth(500));
                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUILayout.LabelField(GUIContents.cLoadingBarPosition, GUILayout.Width(150));
                    _loadingBarPosition.vector3Value = EditorGUILayout.Vector3Field("", _loadingBarPosition.vector3Value);
                });

                // Manually draw label & field instead, for some reason this is printing in two lines
                //EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChangerSettings.LoadingBarSize)));
                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUILayout.LabelField(GUIContents.cLoadingBarSize, GUILayout.Width(150));
                    _loadingBarSize.vector2Value = EditorGUILayout.Vector2Field("", _loadingBarSize.vector2Value);
                });
                EditorGUILayout.Space(5);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_loadingBarFillMethod, GUIContents.cLoadingBarFillMethod);
                if (EditorGUI.EndChangeCheck())
                {
                    _loadingBarFillOrigin.intValue = 0;
                }

                string label = nameof(SceneChangerSettings.LoadingBarFillOrigin).SplitCamelCase();
                switch ((FillMethod) _loadingBarFillMethod.enumValueFlag)
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

                EditorGUILayout.PropertyField(_preserveAspectRatio, GUIContents.cPreserveAspectRatio);
                EditorGUILayout.Space(5);

                EditorGUILayout.PropertyField(_enableTextProgress, GUIContents.cEnableTextProgress);
                GUIHelper.DrawAsReadOnly(!_enableTextProgress.boolValue, gui: () =>
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.PropertyField(_textProgressPosition, GUIContents.cTextProgressPosition);
                    EditorGUILayout.PropertyField(_textProgressObjectSize, GUIContents.cTextProgressObjectSize);

                    EditorGUILayout.Space(5);
                    EditorGUILayout.PropertyField(_textProgressSize, GUIContents.cTextProgressSize);
                    EditorGUILayout.PropertyField(_textProgressColor, GUIContents.cTextProgressColor);
                });
                EditorGUILayout.Space(10);
            }

            // Aspect ratios
            _resolutionFoldout = EditorGUILayout.Foldout(_resolutionFoldout, cResolutionLabel);

            if (_resolutionFoldout)
            {
                EditorGUILayout.PropertyField(_autoAdjustToResolution, GUIContents.cAutoAdjustToResolution);

                GUIHelper.DrawAsReadOnly(Settings.AutoAdjustToResolution, gui: () =>
                {
                    EditorGUILayout.PropertyField(_screenSize, GUIContents.cScreenSize);
                });

                GUIHelper.DrawAsReadOnly(!Settings.AutoAdjustToResolution, gui: () =>
                {
                    EditorGUILayout.PropertyField(_resolutionCheckDelay, GUIContents.cResolutionCheckDelay);
                });
            }

            EditorGUILayout.Space(10);

            _serializedObject.ApplyModifiedProperties();
        }
    }
}
