using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Plugins.Logs;
using Cyggie.SceneChanger.Runtime.Configurations;
using Cyggie.SceneChanger.Runtime.Settings;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cyggie.SceneChanger.Runtime.ServicesNS
{
    /// <summary>
    /// Service class for managing Scene Changes
    /// </summary>
    public sealed class SceneChangerService : Service
    {
        /// <summary>
        /// Called when a Scene Change has started
        /// </summary>
        public delegate void OnSceneChangeStartedEvent(SceneChangeStartedEventArgs args);
        public OnSceneChangeStartedEvent OnSceneChangeStarted = null;
        private SceneChangeStartedEventArgs _sceneChangeStartedArgs = new SceneChangeStartedEventArgs();

        /// <summary>
        /// Called when a Scene Change has completed
        /// </summary>
        public delegate void OnSceneChangeCompletedEvent(SceneChangeCompletedEventArgs args);
        public OnSceneChangeCompletedEvent OnSceneChangeCompleted = null;
        private SceneChangeCompletedEventArgs _sceneChangeCompletedArgs = new SceneChangeCompletedEventArgs();

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
        protected override void OnInitialized()
        {
            _settings = (SceneChangerSettings) _configuration;

            // Create game object from prefab
            _loadingScreen = Instantiate(_settings.LoadingScreenPrefab);

            // Hide object
            _loadingScreen.ToggleCanvas(false);
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
            string sceneName = SceneUtility.GetScenePathByBuildIndex(index);
            if (string.IsNullOrEmpty(sceneName))
            {
                Log.Error($"Unable to change scene, scene by index was not found in the build settings: {index}.", nameof(SceneChangerService));
                return;
            }

            ChangeScene(sceneName, changeSceneSettings);
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

            string sceneName = Path.GetFileNameWithoutExtension(name);

            _sceneChangeStartedArgs.Scene = sceneName;
            _sceneChangeCompletedArgs.PreviousScene = _sceneChangeCompletedArgs.NewScene;
            _sceneChangeCompletedArgs.NewScene = sceneName;

            _loadingScreen.StartCoroutine(ChangeSceneAsync(asyncOperation, changeSceneSettings));
        }

        /// <summary>
        /// Reload the current loaded scene
        /// </summary>
        /// <param name="changeSceneSettings">Change scene settings to apply</param>
        public void ReloadScene(ChangeSceneSettings changeSceneSettings = null) => ChangeScene(SceneManager.GetActiveScene().name, changeSceneSettings);

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
                Log.Error($"Scene Changer was not initialized properly, unable to change scene at {nameof(ChangeScene)}.", nameof(SceneChangerService));
                return false;
            }

            // Make sure a scene change is not already in progress
            if (_inProgress)
            {
                Log.Error($"Scene change already in progress, cancelling new call...", nameof(SceneChangerService));
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

            OnSceneChangeStarted?.Invoke(_sceneChangeStartedArgs);
            _loadingScreen.ToggleCanvas(true);

            // Apply default settings if null
            changeSceneSettings ??= ChangeSceneSettings.Default;

            // Toggle text visibility by its index
            if (changeSceneSettings.HasTextIndexes)
            {
                foreach (int i in changeSceneSettings.TextIndexes)
                {
                    if (i >= _settings.Texts.Length)
                    {
                        Log.Error($"Index is out of range: Settings has {_settings.Texts.Length} texts, but request index was {i} (array is in base 0).", nameof(SceneChangerService));
                        continue;
                    }

                    _loadingScreen.ToggleTextIndex(i);
                }
            }

            asyncOperation.allowSceneActivation = false;

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

                yield return _loadingScreen.StartCoroutine(WaitLoadingScreen(asyncOperation, changeSceneSettings));

                _loadingScreen.ToggleLoadingScreen(false, false);
            }
            else
            {
                yield return _loadingScreen.StartCoroutine(WaitLoadingScreen(asyncOperation, changeSceneSettings));
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

            _loadingScreen.ToggleCanvas(false);

            OnSceneChangeCompleted?.Invoke(_sceneChangeCompletedArgs);

            _inProgress = false;
        }

        /// <summary>
        /// Coroutine to process the loading/waiting of the async operation to change scene
        /// </summary>
        /// <param name="asyncOperation">The loading screen async operation</param>
        /// <param name="changeSceneSettings">The settings applied</param>
        private IEnumerator WaitLoadingScreen(AsyncOperation asyncOperation, ChangeSceneSettings changeSceneSettings)
        {
            float minimumLoadTime = _settings.MinimumLoadTime;

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