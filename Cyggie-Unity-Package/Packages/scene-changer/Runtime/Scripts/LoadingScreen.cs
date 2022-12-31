using Cyggie.Main.Runtime.Utils.Extensions;
using Cyggie.Main.Runtime.Utils.Helpers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Cyggie.SceneChanger.Runtime
{
    internal class LoadingScreen : MonoBehaviour
    {
        private const float cResolutionCheckDelay = 0.5f;

        [SerializeField, Tooltip("")]
        private CanvasScaler _canvasScaler = null;

        [Space]
        [SerializeField, Tooltip("")]
        private Image _fadeImage = null;

        [SerializeField, Tooltip("")]
        private GameObject _defaultLoadingScreen = null;

        [SerializeField, Tooltip("")]
        private Image _loadingScreenImage = null;

        private GameObject _loadingScreen = null;
        private SceneChangerSettings _settings = null;

        [Space]
        [SerializeField, Tooltip("")]
        private Transform _textsParent = null;

        public Transform TextsParent => _textsParent;

        private void Awake()
        {
            // Set fade image to invisible
            _fadeImage.color = _fadeImage.color.ChangeAlpha(0);
            _fadeImage.gameObject.SetActive(true);

            // CHECK SETTINGS TODO
            // Loading screen image
            _loadingScreen = _defaultLoadingScreen; // or other game object for loading screen
            _loadingScreen.SetActive(false);

            DontDestroyOnLoad(gameObject);
        }

        public void FadeIn(Action onCompleted = null)
        {
            _fadeImage.color = _fadeImage.color.ChangeAlpha(0);
            Fade(targetAlpha: 1, _settings.FadeIn, onCompleted);
        }

        public void FadeOut(Action onCompleted = null)
        {
            _fadeImage.color = _fadeImage.color.ChangeAlpha(1);
            Fade(targetAlpha: 0, _settings.FadeOut, onCompleted);
        }

        private void Fade(float targetAlpha, FadeSettings fadeSettings, Action onCompleted = null)
        {
            ColorHelper.TransitionColorAlpha(this, _fadeImage.color, targetAlpha,
                transitionDuration: fadeSettings.Duration,
                onColorUpdated: color => _fadeImage.color = color,
                onTransitionCompleted: onCompleted);
        }

        public void ToggleLoadingScreen(bool toggle)
        {
            _loadingScreen.SetActive(toggle);
        }

        public void SetProgress(float progress)
        {

        }

        internal void SetSettings(SceneChangerSettings settings)
        {
            _settings = settings;

            if (_settings.AutoAdjustToResolution)
            {
                StartCoroutine(CheckResolutionChange());
            }
        }

        internal IEnumerator CheckResolutionChange()
        {
            while (true)
            {
                if (_settings.ScreenSize != UnityHelper.GetVector2Resolution())
                {
                    _settings.UpdateLoadingScreenPrefab(false);
                    _canvasScaler.referenceResolution = _settings.ScreenSize;
                }

                yield return new WaitForSeconds(_settings.ResolutionChangeDelay);
            }
        }
    }
}
