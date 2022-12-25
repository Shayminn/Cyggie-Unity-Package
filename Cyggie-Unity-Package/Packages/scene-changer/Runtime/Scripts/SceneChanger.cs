using Cyggie.SceneChanger.Runtime.Utils;
using System.Collections;
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

        private static bool IsInitialized => _settings == null || _loadingScreen == null;

        static SceneChanger()
        {
            // Get the settings at path
            _settings = Resources.Load<SceneChangerSettings>(SceneChangerPaths.cSettingsPath);

            _loadingScreen = GameObject.Instantiate(_settings.LoadingScreen);
        }

        public static void ChangeScene(int index, bool enableLoadingScreen = true, bool enableLoadingBar = true, bool enableFadeIn = true, bool enableFadeOut = true)
        {
            ChangeScene(SceneManager.GetSceneAt(index), enableLoadingScreen, enableLoadingBar, enableFadeIn, enableFadeOut);
        }

        public static void ChangeScene(string name, bool enableLoadingScreen = true, bool enableLoadingBar = true, bool enableFadeIn = true, bool enableFadeOut = true)
        {
            ChangeScene(SceneManager.GetSceneByName(name), enableLoadingScreen, enableLoadingBar, enableFadeIn, enableFadeOut);
        }

        private static void ChangeScene(Scene scene, bool enableLoadingScreen, bool enableLoadingBar, bool enableFadeIn, bool enableFadeOut)
        {
            // Make sure Scene Changer is initialized
            if (!IsInitialized)
            {
                Debug.LogError($"Scene Changer was not initialized properly, unable to change scene at {nameof(ChangeScene)}. (Settings: {_settings}, Loading Screen: {_loadingScreen})");
                return;
            }

            // Make sure a scene change is not already in progress
            if (_inProgress)
            {
                Debug.LogError($"Scene change already in progress, cancelling new call...");
                return;
            }

            _loadingScreen.StartCoroutine(ChangeSceneAsync(scene));
        }

        private static IEnumerator ChangeSceneAsync(Scene scene)
        {
            _inProgress = true;

            // Change scene
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene.buildIndex);

            while (!asyncOperation.isDone)
            {
                if (asyncOperation.progress < 1)
                {
                    _loadingScreen.SetProgress(asyncOperation.progress);
                    yield return null;
                }

                asyncOperation.allowSceneActivation = true;
            }

            _inProgress = false;
            yield break;
        }
    }
}