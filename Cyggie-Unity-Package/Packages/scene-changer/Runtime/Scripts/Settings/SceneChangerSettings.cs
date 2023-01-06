using UnityEngine;
using Cyggie.SceneChanger.Runtime.Utils.Constants;
using Cyggie.Main.Runtime.Utils.Extensions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cyggie.SceneChanger.Runtime.Settings
{
    /// <summary>
    /// Scriptable object to hold settings for <see cref="SceneChanger"/> <br/>
    /// This is managed in Project Settings/Cyggie/SceneChanger
    /// </summary>
    internal class SceneChangerSettings : ScriptableObject
    {
        private const string cSettingsFileName = "SceneChangerSettings.asset";
        private const string cSettingsScriptFileName = "SceneChangerSettingsIMGUI.cs";

        #region Loading Screen fields

        [SerializeField, Tooltip("Images to use as loading screen.")]
        internal Texture2D[] Images = null;

        [SerializeField, Tooltip("Auto scale image to resolution or the image will simply be cut off.")]
        internal bool ScaleImageToResolution = true;

        [SerializeField, Tooltip("Randomize the order of the image.")]
        internal bool RandomizeImages = true;

        [SerializeField, Tooltip("Type of randomization\n " +
                                 "ResetAfterEach: Resets the list after every load allowing the same images to be used sequentially.\n" +
                                 "ResetAfterEachNoPreviousRepeat: Resets the list but will also make sure the loading image won't ever repeat.\n" +
                                 "RoundRobin: Does not reset the list until every image has been loaded at least once.")]
        internal SceneChangeRandomType RandomType = SceneChangeRandomType.ResetAfterEach;

        [SerializeField, Tooltip("Text objects to create for the loading screens.")]
        internal SceneChangeText[] Texts = null;

        [SerializeField, Tooltip("Minimum amount of time to load before changing scene even if the scene is ready to be loaded.")]
        internal float MinimumLoadTime = 0f;

        #endregion

        #region Resolution fields

        [SerializeField, Tooltip("Auto adjust the loading screen to the resolution.")]
        internal bool AutoAdjustToResolution = true;

        [SerializeField, Tooltip("Fixed resolution screen size to use.")]
        internal Vector2 ScreenSize = new Vector2(1920, 1080);

        [SerializeField, Tooltip("Delay between each check for a change in resolution.")]
        internal float ResolutionCheckDelay = 0.5f;

        #endregion

        /// <summary>
        /// Loading Screen prefab object
        /// </summary>
        [SerializeField, HideInInspector]
        private LoadingScreen _loadingScreen = null;
        internal LoadingScreen LoadingScreen => _loadingScreen.AssignIfNull(GetLoadingScreen());

        internal bool HasImages => Images != null && Images.Length > 0;

        internal bool HasTexts => Texts != null && Texts.Length > 0;

#if UNITY_EDITOR
        private static SerializedObject _serializedSettings = null;
        internal static SerializedObject SerializedSettings => _serializedSettings ??= new SerializedObject(GetOrCreateSettings());

        /// <summary>
        /// Get or create if not found new default settings
        /// </summary>
        /// <returns>SceneChangerSettings object</returns>
        public static SceneChangerSettings GetOrCreateSettings()
        {
            // Try get the relative path to file
            SceneChangerSettings settings = AssetDatabase.LoadAssetAtPath<SceneChangerSettings>(SceneChangerPaths.cSettings);

            if (settings == null)
            {
                // Settings not found, create a new one
                Debug.Log($"Couldn't find default settings file, creating a new one...");

                settings = CreateInstance<SceneChangerSettings>();
                AssetDatabase.CreateAsset(settings, SceneChangerPaths.cSettings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"New settings ({nameof(SceneChangerSettings)}) created at path: {SceneChangerPaths.cSettings}.");
            }

            return settings;
        }

        /// <summary>
        /// Get the loading screen prefab from the absolute path <see cref="SceneChangerPaths.cLoadingScreen"/>
        /// </summary>
        /// <returns></returns>
        public static LoadingScreen GetLoadingScreen()
        {
            LoadingScreen loadingScreen = AssetDatabase.LoadAssetAtPath<LoadingScreen>(SceneChangerPaths.cLoadingScreen);

            if (loadingScreen == null)
            {
                // Loading screen prefab not found
                Debug.Log($"Loading screen prefab was not found, try reimporting the package.");
            }

            return loadingScreen;
        }

        /// <summary>
        /// Validate inputs
        /// </summary>
        internal void OnValidate()
        {

        }

        /// <summary>
        /// Keywords for search in Project Settings
        /// </summary>
        /// <returns></returns>
        internal static string[] GetKeywords() => new string[]
        {
            nameof(LoadingScreen),
            nameof(Images),
            nameof(ScaleImageToResolution),
            nameof(RandomizeImages),
            nameof(RandomType),
            nameof(Texts),
            nameof(MinimumLoadTime),

            nameof(AutoAdjustToResolution),
            nameof(ScreenSize),
            nameof(ResolutionCheckDelay),
        };
#endif
    }

    /// <summary>
    /// Enum of RandomType when scene changing randomly
    /// </summary>
    internal enum SceneChangeRandomType
    {
        ResetAfterEach,
        ResetAfterEachNoPreviousRepeat,
        RoundRobin
    }
}
