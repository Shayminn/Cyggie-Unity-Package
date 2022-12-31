using Cyggie.Main.Runtime.Utils.Extensions;
using Cyggie.SceneChanger.Runtime.Utils;
using UnityEngine;
using System.ComponentModel;
using Cyggie.SceneChanger.Runtime.Utils.Constants;
using System.Runtime.CompilerServices;
using Cyggie.Main.Runtime.Utils.Helpers;
using UnityEngine.UI;
using log4net.Util;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cyggie.SceneChanger.Runtime
{
    // Create a new type of Settings Asset.
    internal class SceneChangerSettings : ScriptableObject
    {
        private const string cSettingsFileName = "SceneChangerSettings.asset";
        private const string cSettingsScriptFileName = "SceneChangerSettingsIMGUI.cs";

        #region Loading Screen fields

        [SerializeField, Tooltip("Loading screen prefabt object")]
        internal LoadingScreen LoadingScreen = null;

        [SerializeField, Tooltip("")]
        internal Texture[] Images = null;

        [SerializeField, Tooltip("")]
        internal bool ScaleImageToResolution = true;

        [SerializeField, Tooltip("")]
        internal bool RandomizeImages = true;

        [SerializeField, Tooltip("")]
        internal SceneChangeRandomType RandomType = SceneChangeRandomType.ResetAfterEach;

        [SerializeField, Tooltip("")]
        internal SceneChangeText[] Texts = null;

        [SerializeField, Tooltip("")]
        internal float MinimumLoadTime = 1f;

        #endregion

        #region Screen size fields

        [SerializeField, Tooltip("")]
        internal bool DynamicScreenSize = true;

        [SerializeField, Tooltip("")]
        internal Vector2 ScreenSize = new Vector2(1920, 1080);

        #endregion

        #region Fade fields

        [SerializeField, Tooltip("")]
        internal FadeSettings FadeIn = new FadeSettings();

        [Space]
        [SerializeField, Tooltip("")]
        internal FadeSettings FadeOut = new FadeSettings();

        #endregion

        #region Other fields

        [SerializeField, Tooltip("")]
        internal bool AutoAdjustToResolution = true;

        [SerializeField, Tooltip("")]
        internal float ResolutionChangeDelay = 0.5f;

        #endregion

        internal void UpdateLoadingScreenPrefab(bool textsUpdated)
        {
            if (LoadingScreen == null) return;

            if (DynamicScreenSize)
            {
                ScreenSize = UnityHelper.GetVector2Resolution();
            }

            LoadingScreen.GetComponent<Canvas>().sortingOrder = 32767;
            LoadingScreen.GetComponent<CanvasScaler>().referenceResolution = ScreenSize;

            if (LoadingScreen.TextsParent != null && textsUpdated)
            {
                Debug.Log("Texts updated");

                //LoadingScreen.
            }
        }

#if UNITY_EDITOR

        private static SerializedObject _serializedSettings = null;
        internal static SerializedObject SerializedSettings => _serializedSettings ??= new SerializedObject(GetOrCreateSettings());

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

        internal static string[] GetKeywords() => new string[]
        {
            nameof(LoadingScreen),
            nameof(Images),
            nameof(ScaleImageToResolution),
            nameof(RandomizeImages),
            nameof(RandomType),
            nameof(Texts),
            nameof(MinimumLoadTime),

            nameof(DynamicScreenSize),
            nameof(ScreenSize),
            
            nameof(FadeIn),
            nameof(FadeOut),

            nameof(AutoAdjustToResolution),
            nameof(ResolutionChangeDelay)
        };
#endif
    }

    internal enum SceneChangeRandomType
    {
        ResetAfterEach,
        RoundRobin
    }

    [Serializable]
    internal struct SceneChangeText
    {
        [SerializeField, Tooltip("")]
        internal string Text;

        [SerializeField, Tooltip("")]
        internal Vector3 Position;
    }
}
