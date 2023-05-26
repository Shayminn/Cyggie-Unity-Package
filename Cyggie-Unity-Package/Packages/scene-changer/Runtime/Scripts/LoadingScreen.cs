using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Main.Runtime.Utils.Enums;
using Cyggie.Main.Runtime.Utils.Extensions;
using Cyggie.Main.Runtime.Utils.Helpers;
using Cyggie.SceneChanger.Runtime.Configurations;
using Cyggie.SceneChanger.Runtime.Enums;
using Cyggie.SceneChanger.Runtime.ServicesNS;
using Cyggie.SceneChanger.Runtime.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cyggie.SceneChanger.Runtime
{
    /// <summary>
    /// Object class for a Loading Screen controlling UI elements based on settings from <see cref="SceneChangerSettings"/>
    /// </summary>
    internal class LoadingScreen : MonoBehaviour
    {
        [Header("Canvas")]
        [SerializeField, Tooltip("Canvas Scaler for resolution scaling.")]
        private CanvasScaler _canvasScaler = null;

        [Header("Fade Image")]
        [SerializeField, Tooltip("Image for displaying Fade In and Fade Out.")]
        private Image _fadeImage = null;

        [Header("Loading Screen")]
        [SerializeField, Tooltip("Image for diplaying the Loading Screen.")]
        private Image _loadingScreenImage = null;

        [SerializeField, Tooltip("RectTransform of the loading screen image.")]
        private RectTransform _loadingScreenImageTransform = null;

        [Header("Loading Bar")]
        [SerializeField, Tooltip("Image of the loading bar.")]
        private Image _loadingBarImage = null;

        [SerializeField, Tooltip("Text of the loading bar progress.")]
        private TextMeshProUGUI _loadingBarText = null;

        [Header("Texts")]
        [SerializeField, Tooltip("Parent GameObject for all loading screen texts.")]
        private Transform _textsParent = null;

        [SerializeField, Tooltip("Loading Screen Text prefab to instantiate.")]
        private TextMeshProUGUI _textPrefab = null;

        private SceneChangerSettings _settings = null;
        private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();
        private List<SceneChangeText> _inactiveTextToLoad = new List<SceneChangeText>();

        private Texture2D[] _images = null;
        private Queue<Texture2D> _imageQueue = null; // Queue is only used if it's RandomType.RoundRobin or not randomized
        private int _previousRandomIndex = -1; // Used with RandomType.ResetAfterEachNoPreviousRepeat

        private SceneChangerService _sceneChangerService = null;
        private SceneChangerService SceneChangerService => _sceneChangerService ??= ServiceManager.Get<SceneChangerService>();

        /// <summary>
        /// MonoBehaviour awake called on start up
        /// </summary>
        private void Awake()
        {
            // Set fade image to invisible
            _fadeImage.color = _fadeImage.color.ChangeAlpha(0);
            _fadeImage.gameObject.SetActive(true);

            // Loading screen image
            ToggleLoadingScreen(false, false);
        }

        /// <summary>
        /// Apply a Fade In using <see cref="_fadeImage"/>
        /// </summary>
        /// <param name="onCompleted">Action called when the Fade is completed</param>
        internal void FadeIn(ChangeSceneFade fadeIn, Action onCompleted = null)
        {
            _fadeImage.color = _fadeImage.color.ChangeAlpha(0);
            Fade(targetAlpha: 1, fadeIn, onCompleted);
        }

        /// <summary>
        /// Apply a Fade Out using <see cref="_fadeImage"/>
        /// </summary>
        /// <param name="onCompleted">Action called when the Fade is completed</param>
        internal void FadeOut(ChangeSceneFade fadeOut, Action onCompleted = null)
        {
            _fadeImage.color = _fadeImage.color.ChangeAlpha(1);
            Fade(targetAlpha: 0, fadeOut, onCompleted);
        }

        /// <summary>
        /// Apply a Fade using <see cref="_fadeImage"/>
        /// </summary>
        /// <param name="targetAlpha">Target alpha value</param>
        /// <param name="fadeSettings">FadeSettings to apply to transition</param>
        /// <param name="onCompleted">Action called when the Fade is completed</param>
        private void Fade(float targetAlpha, ChangeSceneFade fadeSettings, Action onCompleted = null)
        {
            ColorHelper.TransitionColorAlpha(
                mono: this,
                color: _fadeImage.color,
                targetAlpha: targetAlpha,
                transitionDuration: fadeSettings.Duration,
                onColorUpdated: color => _fadeImage.color = color,
                onTransitionCompleted: onCompleted);
        }

        /// <summary>
        /// Toggle visibility of <see cref="_loadingScreenImage"/>
        /// </summary>
        /// <param name="toggle"></param>
        internal void ToggleLoadingScreen(bool toggle, bool toggleLoadingBar)
        {
            if (toggle)
            {
                int imageIndex = -1;

                if (_settings.HasImages)
                {
                    Texture2D texture = GetImage();

                    _loadingScreenImage.sprite = texture.ToSprite();
                    imageIndex = Array.IndexOf(_images, texture);
                }

                // Sets all text to load
                _inactiveTextToLoad = _settings.Texts.Where(x => x.ImageSpecific == -1 || x.ImageSpecific == imageIndex).ToList();
                SetProgress(0);
            }

            _loadingBarImage.gameObject.SetActive(toggleLoadingBar);

            _loadingScreenImage.gameObject.SetActive(toggle);
            _textsParent.gameObject.SetActive(toggle);
        }

        /// <summary>
        /// Toggle the canvas to enable mouse clicks through it when the Loading Screen is not active
        /// </summary>
        /// <param name="toggle">Whether the canvas is active in the scene</param>
        internal void ToggleCanvas(bool toggle)
        {
            _canvasScaler.gameObject.SetActive(toggle);
        }

        /// <summary>
        /// Sets the progress of the loading bar <br/>
        /// Display any text that has <see cref="SceneChangeText.DisplayAtProgress"/> if <paramref name="progress"/> has passed it
        /// </summary>
        /// <param name="progress">Current progress</param>
        internal void SetProgress(float progress)
        {
            // progress in base 100
            float progressBase100 = progress * 100;

            // Enable all texts to load whose progress has been passed
            List<SceneChangeText> tempList = new List<SceneChangeText>(_inactiveTextToLoad);
            foreach (SceneChangeText sceneChangeText in tempList)
            {
                if (sceneChangeText.DisplayAtProgress <= progressBase100)
                {
                    // Toggle text based on its index
                    int textIndex = Array.IndexOf(_settings.Texts, sceneChangeText);
                    ToggleTextIndex(textIndex);

                    // Remove loaded text
                    _inactiveTextToLoad.Remove(sceneChangeText);
                }
            }

            // Update progress bar slider
            if (_loadingBarImage.IsActive())
            {
                _loadingBarImage.fillAmount = progress;
                _loadingBarText.text = progressBase100.ToString();
            }
        }

        /// <summary>
        /// Sets the text based on its index during runtime
        /// </summary>
        /// <param name="index">Index of text</param>
        /// <param name="text">Text to set</param>
        internal void SetTextAtIndex(int index, string text)
        {
            _texts[index].text = text;
        }

        /// <summary>
        /// Toggle a text by its index during runtime
        /// </summary>
        /// <param name="index">Index of text</param>
        internal void ToggleTextIndex(int index)
        {
            _texts[index].gameObject.SetActive(true);

            // Reset the text's visibility when the scene load has completed
            void ResetText()
            {
                _texts[index].gameObject.SetActive(false);
                SceneChangerService.OnSceneChangeCompleted -= ResetText;
            }
            SceneChangerService.OnSceneChangeCompleted += ResetText;
        }

        /// <summary>
        /// Sets the settings saved from project settings loaded by <see cref="SceneChangerService"/> <br/>
        /// Initialize the values for the Loading screen
        /// </summary>
        /// <param name="settings"></param>
        internal void SetSettings(SceneChangerSettings settings)
        {
            _settings = settings;

            if (_settings.AutoAdjustToResolution)
            {
                _canvasScaler.referenceResolution = _settings.ScreenSize;
            }

            // Set up image variables
            if (settings.HasImages)
            {
                _images = settings.Images;

                // Set the color to white (instead of the default black)
                _loadingScreenImage.color = Color.white;

                // Set image scaling
                if (settings.ScaleImageToResolution)
                {
                    _loadingScreenImageTransform.SetAnchorType(RectTransformAnchorType.StretchStretch);
                }
                else
                {
                    _loadingScreenImageTransform.SetAnchorType(RectTransformAnchorType.MiddleCenter, settings.ScreenSize.x, settings.ScreenSize.y);
                }
            }

            // Instantiate all the texts
            foreach (SceneChangeText text in _settings.Texts)
            {
                TextMeshProUGUI textObj = Instantiate(_textPrefab, _textsParent);
                textObj.text = text.Text;

                RectTransform rectTransform = textObj.GetComponent<RectTransform>();

                // Set anchor
                rectTransform.anchoredPosition = text.Position;
                rectTransform.sizeDelta = text.ObjectSize;

                textObj.color = text.TextColor;
                textObj.fontSize = text.TextSize;
                textObj.fontSizeMax = text.TextSize;

                // All objects are set to false
                textObj.gameObject.SetActive(text.AlwaysVisible);

                _texts.Add(textObj);
            }

            // Set up loading bar image
            if (settings.LoadingBarImage != null)
            {
                _loadingBarImage.sprite = settings.LoadingBarImage.ToSprite();
            }

            _loadingBarImage.color = settings.LoadingBarImageColor;

            // Set loading bar image transform values
            RectTransform loadingBarRectTransform = _loadingBarImage.GetComponent<RectTransform>();
            loadingBarRectTransform.anchoredPosition = settings.LoadingBarPosition;
            loadingBarRectTransform.sizeDelta = settings.LoadingBarSize;

            // Set loading bar image values
            _loadingBarImage.fillMethod = settings.LoadingBarFillMethod;
            _loadingBarImage.fillOrigin = settings.LoadingBarFillOrigin;
            _loadingBarImage.preserveAspect = settings.PreserveAspectRatio;

            // Set loading bar text progress if enabled
            if (settings.EnableTextProgress)
            {
                RectTransform rectTransform = _loadingBarText.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = settings.TextProgressPosition;
                rectTransform.sizeDelta = settings.TextProgressObjectSize;

                _loadingBarText.fontSize = settings.TextProgressSize;
                _loadingBarText.fontSizeMax = settings.TextProgressSize;
                _loadingBarText.color = settings.TextProgressColor;
            }
            _loadingBarImage.gameObject.SetActive(false);

            // Set up auto adjustments to resolution
            if (_settings.AutoAdjustToResolution)
            {
                StartCoroutine(CheckResolutionChange());
            }
        }

        /// <summary>
        /// Coroutine to check for a change in resolution every <see cref="SceneChangerSettings.ResolutionCheckDelay"/>
        /// </summary>
        /// <returns></returns>
        internal IEnumerator CheckResolutionChange()
        {
            while (true)
            {
                if (_settings.ScreenSize != UnityHelper.GetVector2Resolution())
                {
                    _canvasScaler.referenceResolution = _settings.ScreenSize;
                }

                yield return new WaitForSeconds(_settings.ResolutionCheckDelay);
            }
        }

        /// <summary>
        /// Get the loading screen image based on settings <see cref="_settings"/>>
        /// </summary>
        /// <returns></returns>
        private Texture2D GetImage()
        {
            // Checks if Settings has custom images
            if (_settings.HasImages)
            {
                // Check if queue has something
                if (_imageQueue == null || _imageQueue.Count == 0)
                {
                    // Check if images are randomized
                    if (_settings.RandomizeImages)
                    {
                        switch (_settings.RandomType)
                        {
                            case SceneChangeRandomType.ResetAfterEach:
                                return _images[UnityEngine.Random.Range(0, _images.Length)];

                            case SceneChangeRandomType.ResetAfterEachNoPreviousRepeat:
                                {
                                    int randomIndex;
                                    do
                                    {
                                        randomIndex = UnityEngine.Random.Range(0, _images.Length);

                                        if (_images.Length == 1)
                                        {
                                            Debug.LogError($"Loading Screen's {nameof(SceneChangerSettings.RandomType)} set to {nameof(SceneChangeRandomType.ResetAfterEachNoPreviousRepeat)} but there's only one possible image. Add more images or use {nameof(SceneChangeRandomType.ResetAfterEach)} instead.");
                                            break;
                                        }
                                    }
                                    while (randomIndex == _previousRandomIndex);
                                    _previousRandomIndex = randomIndex;

                                    return _images[randomIndex];
                                }

                            case SceneChangeRandomType.RoundRobin:
                                _imageQueue = new Queue<Texture2D>(_images.Shuffle());
                                break;
                        }
                    }
                    else
                    {
                        _imageQueue = new Queue<Texture2D>(_images);
                    }
                }

                return _imageQueue.Dequeue();
            }

            // Return null (default image)
            return null;
        }
    }
}
