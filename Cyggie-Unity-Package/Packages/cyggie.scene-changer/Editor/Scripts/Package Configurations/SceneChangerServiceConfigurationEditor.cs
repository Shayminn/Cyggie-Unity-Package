using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Plugins.Editor.Helpers;
using Cyggie.Plugins.Utils.Extensions;
using Cyggie.SceneChanger.Editor.Utils.Styles;
using Cyggie.SceneChanger.Runtime;
using Cyggie.SceneChanger.Runtime.Configurations;
using Cyggie.SceneChanger.Runtime.Utils.Constants;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Cyggie.SceneChanger.Editor.Configurations
{
    /// <summary>
    /// Inspector editor for <see cref="SceneChangerServiceConfiguration"/>
    /// </summary>
    [CustomEditor(typeof(SceneChangerServiceConfiguration))]
    internal class SceneChangerServiceConfigurationEditor : UnityEditor.Editor
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

        private SceneChangerServiceConfiguration _config = null;

        #endregion

        private void OnEnable()
        {
            _config = target as SceneChangerServiceConfiguration;

            if (_config.LoadingScreenPrefab == null)
            {
                _config.LoadingScreenPrefab = AssetDatabase.LoadAssetAtPath<LoadingScreen>(SceneChangerPaths.cLoadingScreenPrefab);
                EditorUtility.SetDirty(_config);
            }

            _loadingScreenPrefab = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.LoadingScreenPrefab));

            _images = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.Images));
            _scaleImageToResolution = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.ScaleImageToResolution));
            _randomizeImages = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.RandomizeImages));
            _randomType = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.RandomType));
            _texts = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.Texts));
            _minimumLoadTime = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.MinimumLoadTime));

            _loadingBarImage = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.LoadingBarImage));
            _loadingBarImageColor = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.LoadingBarImageColor));
            _loadingBarPosition = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.LoadingBarPosition));
            _loadingBarSize = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.LoadingBarSize));
            _loadingBarFillMethod = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.LoadingBarFillMethod));
            _loadingBarFillOrigin = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.LoadingBarFillOrigin));
            _preserveAspectRatio = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.PreserveAspectRatio));
            _enableTextProgress = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.EnableTextProgress));
            _textProgressPosition = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.TextProgressPosition));
            _textProgressObjectSize = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.TextProgressObjectSize));
            _textProgressSize = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.TextProgressSize));
            _textProgressColor = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.TextProgressColor));

            _autoAdjustToResolution = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.AutoAdjustToResolution));
            _screenSize = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.ScreenSize));
            _resolutionCheckDelay = serializedObject.FindProperty(nameof(SceneChangerServiceConfiguration.ResolutionCheckDelay));
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            if (_config == null) return;

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

                // Extra _config if with custom loading screen images
                GUIHelper.DrawAsReadOnly(_config.Images == null || _config.Images.Length == 0, gui: () =>
                {
                    EditorGUILayout.PropertyField(_scaleImageToResolution, GUIContents.cScaleImageToResolution);
                    EditorGUILayout.PropertyField(_randomizeImages, GUIContents.cRandomizeImages);

                    // Extra _config if randomized images
                    GUIHelper.DrawAsReadOnly(!_config.RandomizeImages, gui: () =>
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

            // Loading Bar _config
            _loadingBarFoldout = EditorGUILayout.Foldout(_loadingBarFoldout, cLoadingBarLabel);

            if (_loadingBarFoldout)
            {
                EditorGUILayout.PropertyField(_loadingBarImage, GUIContents.cLoadingBarImage);
                EditorGUILayout.PropertyField(_loadingBarImageColor, GUIContents.cLoadingBarImageColor);
                EditorGUILayout.Space(5);

                // Manually draw label & field instead, for some reason this is printing in two lines
                //EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChanger_config.LoadingBarPosition)), GUILayout.MaxWidth(500));
                EditorGUILayoutHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUILayout.LabelField(GUIContents.cLoadingBarPosition, GUILayout.Width(150));
                    _loadingBarPosition.vector3Value = EditorGUILayout.Vector3Field("", _loadingBarPosition.vector3Value);
                });

                // Manually draw label & field instead, for some reason this is printing in two lines
                //EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(SceneChanger_config.LoadingBarSize)));
                EditorGUILayoutHelper.DrawHorizontal(gui: () =>
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

                string label = nameof(SceneChangerServiceConfiguration.LoadingBarFillOrigin).SplitCamelCase();
                switch ((FillMethod) _loadingBarFillMethod.enumValueFlag)
                {
                    case FillMethod.Horizontal:
                        _config.LoadingBarFillOrigin = (int) (OriginHorizontal) EditorGUILayout.EnumPopup(label, (OriginHorizontal) _config.LoadingBarFillOrigin);
                        break;
                    case FillMethod.Vertical:
                        _config.LoadingBarFillOrigin = (int) (OriginVertical) EditorGUILayout.EnumPopup(label, (OriginVertical) _config.LoadingBarFillOrigin);
                        break;
                    case FillMethod.Radial90:
                        _config.LoadingBarFillOrigin = (int) (Origin90) EditorGUILayout.EnumPopup(label, (Origin90) _config.LoadingBarFillOrigin);
                        break;
                    case FillMethod.Radial180:
                        _config.LoadingBarFillOrigin = (int) (Origin180) EditorGUILayout.EnumPopup(label, (Origin180) _config.LoadingBarFillOrigin);
                        break;
                    case FillMethod.Radial360:
                        _config.LoadingBarFillOrigin = (int) (Origin360) EditorGUILayout.EnumPopup(label, (Origin360) _config.LoadingBarFillOrigin);
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

                GUIHelper.DrawAsReadOnly(_config.AutoAdjustToResolution, gui: () =>
                {
                    EditorGUILayout.PropertyField(_screenSize, GUIContents.cScreenSize);
                });

                GUIHelper.DrawAsReadOnly(!_config.AutoAdjustToResolution, gui: () =>
                {
                    EditorGUILayout.PropertyField(_resolutionCheckDelay, GUIContents.cResolutionCheckDelay);
                });
            }

            EditorGUILayout.Space(10);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
