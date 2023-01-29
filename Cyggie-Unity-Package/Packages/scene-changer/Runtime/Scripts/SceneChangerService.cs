using Cyggie.Main.Runtime.Services;
using Cyggie.SceneChanger.Runtime.Configurations;
using Cyggie.SceneChanger.Runtime.Settings;
using Cyggie.SceneChanger.Runtime.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cyggie.SceneChanger.Runtime.Services
{
    /// <summary>
    /// Service class for managing Scene Changes
    /// </summary>
    public sealed class SceneChangerService : Service
    {
        /// <summary>
        /// Called when a Scene Change has started
        /// </summary>
        public Action OnSceneChangeStarted = null;

        /// <summary>
        /// Called when a Scene Change has completed
        /// </summary>
        public Action OnSceneChangeCompleted = null;

        /// <summary>
        /// Settings object
        /// </summary>
        private SceneChangerSettings _settings = null;

        /// <summary>
        /// Loading screen hierarchy object created from _settings.LoadingScreen (prefab)
        /// </summary>
        private LoadingScreen _loadingScreen = null;

        private bool _inProgress = false;

        private bool IsInitialized => _settings != null || _loadingScreen != null;

        /// <inheritdoc/>
        protected override void OnInitialized(ServiceConfiguration configuration)
        {
            base.OnInitialized(configuration);

            if (configuration == null || configuration is not SceneChangerSettings settings)
            {
                Debug.Log($"Scene Changer's configuration was not found in the Service Manager Configurations.");
                return;
            }

            _settings = settings;

            // Create game object from prefab
            _loadingScreen = GameObject.Instantiate(_settings.LoadingScreenPrefab);

            // Hide object
            _loadingScreen.ToggleLoadingScreen(false, false);

            // Initialize the settings in the loading screen
            _loadingScreen.SetSettings(_settings);
        }

        /// <summary>
        /// Change scene by its <paramref name="index"/> in the build settings
        /// </summary>
        /// <param name="index">Index of the scene</param>
        /// <param name="changeSceneSettings">Change scene settings to apply</param>
        public void ChangeScene(int index, ChangeSceneSettings changeSceneSettings = null)
        {
            if (!ProcessChecks()) return;

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
            if (asyncOperation == null) return;

            _loadingScreen.StartCoroutine(ChangeSceneAsync(asyncOperation, changeSceneSettings));
        }

        /// <summary>
        /// Change scene by its <paramref name="name"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="changeSceneSettings">Change scene settings to apply</param>
        public void ChangeScene(string name, ChangeSceneSettings changeSceneSettings = null)
        {
            if (!ProcessChecks()) return;

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);
            if (asyncOperation == null) return;

            _loadingScreen.StartCoroutine(ChangeSceneAsync(asyncOperation, changeSceneSettings));
        }

        /// <summary>
        /// Reload the current loaded scene
        /// </summary>
        /// <param name="changeSceneSettings">Change scene settings to apply</param>
        public void ReloadScene(ChangeSceneSettings changeSceneSettings = null)
        {
            if (!ProcessChecks()) return;

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            if (asyncOperation == null) return;

            _loadingScreen.StartCoroutine(ChangeSceneAsync(asyncOperation, changeSceneSettings));
        }

        /// <summary>
        /// Apply a fade without changing scenes
        /// </summary>
        /// <param name="waitTime">Wait time between the fade in and the fade out</param>
        /// <param name="fadeIn">Custom fade in settings (set to <see cref="ChangeSceneFade.Default"/> by default)</param>
        /// <param name="fadeOut">Custom fade out settings (set to <see cref="ChangeSceneFade.Default"/> by default)</param>
        /// <param name="onFadedIn">Action called when the Fade In is complete</param>
        /// <param name="onFadedOut">Action called when the Fade Out is complete</param>
        public void Fade(float waitTime = 0f, ChangeSceneFade fadeIn = null, ChangeSceneFade fadeOut = null, Action onFadedIn = null, Action onFadedOut = null)
        {
            if (!ProcessChecks()) return;

            fadeIn ??= ChangeSceneFade.Default;
            fadeOut ??= ChangeSceneFade.Default;

            _loadingScreen.StartCoroutine(FadeInAndOut(waitTime, fadeIn, fadeOut, onFadedIn, onFadedOut));
        }

        /// <summary>
        /// Set the text by its <paramref name="index"/> during runtime
        /// </summary>
        /// <param name="index">Index of text</param>
        /// <param name="text">Text to set</param>
        public void SetText(int index, string text) => _loadingScreen.SetTextAtIndex(index, text);

        /// <summary>
        /// Process the necessary checks for changing scenes or applying a fade
        /// </summary>
        /// <returns></returns>
        private bool ProcessChecks()
        {
            // Make sure Scene Changer is initialized
            if (!IsInitialized)
            {
                Debug.LogError($"Scene Changer was not initialized properly, unable to change scene at {nameof(ChangeScene)}.");
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

        /// <summary>
        /// Coroutine to change scene asynchronously for <see cref="ChangeScene"/> and <see cref="ReloadScene"/>
        /// </summary>
        /// <param name="asyncOperation">Async operation to change scene</param>
        /// <param name="changeSceneSettings">Change scene settings to apply</param>
        private IEnumerator ChangeSceneAsync(AsyncOperation asyncOperation, ChangeSceneSettings changeSceneSettings = null)
        {
            _inProgress = true;
            OnSceneChangeStarted?.Invoke();

            // Apply default settings if null
            changeSceneSettings ??= ChangeSceneSettings.Default;

            // Toggle text visibility by its index
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
                _loadingScreen.ToggleLoadingScreen(true, changeSceneSettings.EnableLoadingBar);

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

                _loadingScreen.ToggleLoadingScreen(false, false);
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

        /// <summary>
        /// Coroutine to apply a fade for <see cref="Fade"/>
        /// </summary>
        /// <param name="waitTime">Wait time between the fade in and the fade out</param>
        /// <param name="fadeIn">Custom fade in settings (set to <see cref="ChangeSceneFade.Default"/> by default)</param>
        /// <param name="fadeOut">Custom fade out settings (set to <see cref="ChangeSceneFade.Default"/> by default)</param>
        /// <param name="onFadedIn">Action called when the Fade In is complete</param>
        /// <param name="onFadedOut">Action called when the Fade Out is complete</param>
        private IEnumerator FadeInAndOut(float waitTime, ChangeSceneFade fadeIn, ChangeSceneFade fadeOut, Action onFadedIn, Action onFadedOut)
        {
            // Wait for fade in
            bool fadeInCompleted = false;
            _loadingScreen.FadeIn(fadeIn, onCompleted: () => fadeInCompleted = true);

            while (!fadeInCompleted) yield return null;

            // Call wait action
            onFadedIn?.Invoke();
            yield return new WaitForSeconds(waitTime);

            // Wait for fade out
            bool fadeOutCompleted = false;
            _loadingScreen.FadeOut(fadeOut, onCompleted: () => fadeOutCompleted = true);

            while (!fadeOutCompleted) yield return null;

            // Call complete action
            onFadedOut?.Invoke();
        }
    }
}