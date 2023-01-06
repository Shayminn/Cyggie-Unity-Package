using Cyggie.SceneChanger.Runtime.Settings;
using Cyggie.SceneChanger.Runtime.Utils.Constants;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cyggie.SceneChanger.Runtime
{
    /// <summary>
    /// Static class to call for Scene Changes from anywhere within the game
    /// </summary>
    public static class SceneChanger
    {
        /// <summary>
        /// Called when a Scene Change has started
        /// </summary>
        public static Action OnSceneChangeStarted = null;

        /// <summary>
        /// Called when a Scene Change has completed
        /// </summary>
        public static Action OnSceneChangeCompleted = null;

        /// <summary>
        /// Settings object
        /// </summary>
        private static SceneChangerSettings _settings = null;

        /// <summary>
        /// Loading screen hierarchy object created from _settings.LoadingScreen (prefab)
        /// </summary>
        private static LoadingScreen _loadingScreen = null;

        private static bool _inProgress = false;

        private static bool IsInitialized => _settings != null || _loadingScreen != null;

        /// <summary>
        /// Static constructor to scene changer <br/>
        /// Initialize loading screen
        /// </summary>
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

        public static void Fade(float waitTime = 0f, ChangeSceneFade fadeIn = null, ChangeSceneFade fadeOut = null, Action onWait = null, Action onComplete = null)
        {
            if (fadeIn == null)
            {
                fadeIn = ChangeSceneFade.Default;
            }    

            if (fadeOut == null)
            {
                fadeOut = ChangeSceneFade.Default;
            }

            _loadingScreen.StartCoroutine(FadeInAndOut(waitTime, fadeIn, fadeOut, onWait, onComplete));
        }

        public static void SetText(int index, string text) => _loadingScreen.SetTextAtIndex(index, text);

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
            OnSceneChangeStarted?.Invoke();

            if (changeSceneSettings == null)
            {
                changeSceneSettings = ChangeSceneSettings.Default;
            }

            if (changeSceneSettings.HasTextIndexes)
            {
                foreach (int i in changeSceneSettings.TextIndexes)
                {
                    if (i >= _settings.Texts.Length)
                    {
                        Debug.LogError($"Index is out of range: Settings has {_settings.Texts.Length} texts, but request index was {i} (array is in base 0).");
                        continue;
                    }

                    _loadingScreen.ToggleTextIndex(i);
                }
            }

            asyncOperation.allowSceneActivation = false;
            float minimumLoadTime = _settings.MinimumLoadTime;

            // Fade In
            bool fadeInCompleted = false;
            if (changeSceneSettings.HasFadeIn)
            {
                _loadingScreen.FadeIn(changeSceneSettings.FadeIn, onCompleted: () =>
                {
                    fadeInCompleted = true;
                });

                while (!fadeInCompleted) yield return null;
            }

            // Loading screen
            if (changeSceneSettings.EnableLoadingScreen)
            {
                _loadingScreen.ToggleLoadingScreen(true);

                // Loop till scene change is done
                while (!asyncOperation.isDone)
                {
                    minimumLoadTime -= Time.deltaTime;

                    // Progress stays at 0.9f (90%) as long as allowSceneActivation is false
                    // So at 90%, enable scene activation to complete scene change
                    if (asyncOperation.progress >= 0.9f)
                    {
                        _loadingScreen.SetProgress(1);

                        // Wait till remaining minimum loading time is reached
                        if (minimumLoadTime > 0) yield return new WaitForSeconds(minimumLoadTime);

                        // Wait till 
                        if (changeSceneSettings.HasInputSettings) yield return _loadingScreen.StartCoroutine(changeSceneSettings.InputSettings.WaitForInput());

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
            if (changeSceneSettings.HasFadeOut)
            {
                _loadingScreen.FadeOut(changeSceneSettings.FadeOut, onCompleted: () =>
                {
                    fadeOutCompleted = true;
                });

                while (!fadeOutCompleted) yield return null;
            }

            OnSceneChangeCompleted?.Invoke();
            _inProgress = false;
        }

        private static IEnumerator FadeInAndOut(float waitTime, ChangeSceneFade fadeIn, ChangeSceneFade fadeOut, Action onWait, Action onComplete)
        {
            // Wait for fade in
            bool fadeInCompleted = false;
            _loadingScreen.FadeIn(fadeIn, onCompleted: () => fadeInCompleted = true);

            while (!fadeInCompleted) yield return null;

            // Call wait action
            onWait?.Invoke();
            yield return new WaitForSeconds(waitTime);

            // Wait for fade out
            bool fadeOutCompleted = false;
            _loadingScreen.FadeOut(fadeOut, onCompleted: () => fadeOutCompleted = true);

            while (!fadeOutCompleted) yield return null;

            // Call complete action
            onComplete?.Invoke();
        }
    }
}