using Cyggie.Main.Runtime.Utils.Extensions;
using Cyggie.Main.Runtime.Utils.Helpers;
using Cyggie.SceneChanger.Editor.Models;
using Cyggie.SceneChanger.Runtime;
using Cyggie.SceneChanger.Runtime.Utils;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cyggie.SceneChanger.Editor
{
    // Create a new type of Settings Asset.
    public class SceneChangerSettings : ScriptableObject
    {
        private const string cSettingsFileName = "SceneChangerSettings.asset";
        private const string cSettingsScriptFileName = "SceneChangerSettingsIMGUI.cs";

        [Header("Loading Screen Settings")]

        [SerializeField, Tooltip("")]
        private LoadingScreen _loadingScreen = null;

        [SerializeField, Tooltip("")]
        private Texture[] _textures = null;

        [Header("Loading Bar Settings")]

        [Header("Fade Settings")]
        [SerializeField, Tooltip("")]
        private FadeSettings _fadeIn = new FadeSettings();

        [Space]
        [SerializeField, Tooltip("")]
        private FadeSettings _fadeOut = new FadeSettings();

        public LoadingScreen LoadingScreen => _loadingScreen.AssignIfNull(GetOrCreateLoadingScreen());

        public Texture[] Textures => _textures;

        public FadeSettings FadeIn => _fadeIn;

        public FadeSettings FadeOut => _fadeOut;

        private static SerializedObject _serializedSettings = null;
        public static SerializedObject SerializedSettings => _serializedSettings ??= new SerializedObject(GetOrCreateSettings());

        private static SceneChangerSettings GetOrCreateSettings()
        {
            // Try get the relative path to file
            SceneChangerSettings settings = AssetDatabase.LoadAssetAtPath<SceneChangerSettings>(SceneChangerPaths.cSettingsPath);

            if (settings == null)
            {
                // Settings not found, create a new one
                Debug.Log($"Couldn't find default settings file, creating a new one...");

                settings = CreateInstance<SceneChangerSettings>();
                AssetDatabase.CreateAsset(settings, SceneChangerPaths.cSettingsPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"New settings ({nameof(SceneChangerSettings)}) created at path: {SceneChangerPaths.cSettingsPath}.");
            }

            return settings;
        }

        private static LoadingScreen GetOrCreateLoadingScreen()
        {
            LoadingScreen loadingScreen = AssetDatabase.LoadAssetAtPath<LoadingScreen>(SceneChangerPaths.cLoadingScreenPath);

            if (loadingScreen == null)
            {
                // Prefab not found, create a new one
                Debug.Log($"Couldn't find loading screen prefab, creating a new one...");

                // GameObject prefab = Instante
                GameObject obj = new GameObject();
                loadingScreen = obj.AddComponent<LoadingScreen>();

                // Save as prefab at path
                PrefabUtility.SaveAsPrefabAsset(obj, SceneChangerPaths.cLoadingScreenPath);

                // Remove created object from scene
                UnityEngine.Object.DestroyImmediate(obj);

                Debug.Log($"New prefab ({nameof(LoadingScreen)}) created at path: {SceneChangerPaths.cLoadingScreenPath}.");
            }

            return loadingScreen;
        }

        public static string[] GetKeywords() => new string[]
        {
            nameof(_loadingScreen),
            nameof(_fadeIn),
            nameof(_fadeOut)
        };
    }
}
