using Cyggie.Main.Runtime.Utils.Helpers;
using Cyggie.SceneChanger.Runtime.Utils.Constants;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

namespace Cyggie.SceneChanger.Runtime
{
    public static class SceneChanger
    {
        private static SceneChangerSettings _settings = null;
        private static LoadingScreen _loadingScreen = null;

        private static bool _inProgress = false;

        private static bool IsInitialized => _settings != null || _loadingScreen != null;

        static SceneChanger()
        {
            // Get the settings at path
            _settings = Resources.Load<SceneChangerSettings>(SceneChangerConstants.cSettingsFile);

            if (_settings == null)
            {
                Debug.LogError($"Failed to load settings in Resources folder.");
                return;
            }

            _loadingScreen = GameObject.Instantiate(_settings.LoadingScreen);
            _loadingScreen.SetSettings(_settings);
        }

        public static void ChangeScene(int index, ChangeSceneSettings changeSceneSettings = null)
        {
            if (!ProcessChecks()) return;

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
            if (asyncOperation == null) return;

            _loadingScreen.StartCoroutine(ChangeSceneAsync(asyncOperation, changeSceneSettings));
        }

        public static void ChangeScene(string name, ChangeSceneSettings changeSceneSettings = null)
        {
            if (!ProcessChecks()) return;

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);
            if (asyncOperation == null) return;

            _loadingScreen.StartCoroutine(ChangeSceneAsync(asyncOperation, changeSceneSettings));
        }

        public static void ReloadScene(ChangeSceneSettings changeSceneSettings = null)
        {
            if (!ProcessChecks()) return;

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            if (asyncOperation == null) return;

            _loadingScreen.StartCoroutine(ChangeSceneAsync(asyncOperation, changeSceneSettings));
        }

        private static bool ProcessChecks()
        {
            // Make sure Scene Changer is initialized
            if (!IsInitialized)
            {
                Debug.LogError($"Scene Changer was not initialized properly, unable to change scene at {nameof(ChangeScene)}. (Settings: {_settings}, Loading Screen: {_loadingScreen})");
                return false;
            }

            // Make sure a scene change is not already in progress
            if (_inProgress)
            {
                Debug.LogError($"Scene change already in progress, cancelling new call...");
                return false;
            }

            return true;
        }

        private static IEnumerator ChangeSceneAsync(AsyncOperation asyncOperation, ChangeSceneSettings changeSceneSettings = null)
        {
            _inProgress = true;

            if (changeSceneSettings == null)
            {
                changeSceneSettings = ChangeSceneSettings.Default;
            }

            asyncOperation.allowSceneActivation = false;
            float minimumLoadTime = _settings.MinimumLoadTime;

            // Fade In
            bool fadeInCompleted = false;
            if (changeSceneSettings.EnableFadeIn)
            {
                _loadingScreen.FadeIn(onCompleted: () =>
                {
                    fadeInCompleted = true;
                });

                while (!fadeInCompleted) yield return null;
            }

            // Loading screen
            if (changeSceneSettings.EnableLoadingScreen)
            {
                _loadingScreen.ToggleLoadingScreen(true);

                while (!asyncOperation.isDone)
                {
                    minimumLoadTime -= Time.deltaTime;

                    if (asyncOperation.progress >= 0.9f)
                    {
                        _loadingScreen.SetProgress(1);

                        if (minimumLoadTime > 0) yield return new WaitForSeconds(minimumLoadTime);

                        asyncOperation.allowSceneActivation = true;
                    }
                    else
                    {
                        _loadingScreen.SetProgress(asyncOperation.progress);
                    }

                    yield return null;
                }

                _loadingScreen.ToggleLoadingScreen(false);
            }

            // Fade out
            bool fadeOutCompleted = false;
            if (changeSceneSettings.EnableFadeOut)
            {
                _loadingScreen.FadeOut(onCompleted: () =>
                {
                    fadeOutCompleted = true;
                });

                while (!fadeOutCompleted) yield return null;
            }

            _inProgress = false;
            yield break;
        }
    }

    public class ChangeSceneSettings
    {
        public bool EnableLoadingScreen { get; set; }

        public bool EnableLoadingBar { get; set; }

        public FadeSettings FadeSettings { get; set; }

        public bool EnableFadeIn { get; set; }

        public bool EnableFadeOut { get; set; }

        public static readonly ChangeSceneSettings Default = new ChangeSceneSettings()
        {
            EnableLoadingScreen = true,
            EnableLoadingBar = true,
            EnableFadeIn = false,
            EnableFadeOut = false
        };

        public static readonly ChangeSceneSettings EnableAll = new ChangeSceneSettings()
        {
            EnableLoadingScreen = true,
            EnableLoadingBar = true,
            EnableFadeIn = true,
            EnableFadeOut = true
        };

        public static readonly ChangeSceneSettings FadeOnly = new ChangeSceneSettings()
        {
            EnableLoadingScreen = false,
            EnableLoadingBar = false,
            EnableFadeIn = true,
            EnableFadeOut = true
        };
    }
}