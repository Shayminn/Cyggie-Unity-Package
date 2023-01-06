using Cyggie.Main.Runtime.Utils.Enums;
using Cyggie.Main.Runtime.Utils.Extensions;
using Cyggie.Main.Runtime.Utils.Helpers;
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
        [SerializeField, Tooltip("Canvas Scaler for resolution scaling.")]
        private CanvasScaler _canvasScaler = null;

        [Space]
        [SerializeField, Tooltip("Image for displaying Fade In and Fade Out.")]
        private Image _fadeImage = null;

        [SerializeField, Tooltip("Image for diplaying the Loading Screen.")]
        private Image _loadingScreenImage = null;

        [SerializeField, Tooltip("RectTransform of the loading screen image.")]
        private RectTransform _loadingScreenImageTransform = null;

        [Space]
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

        /// <summary>
        /// MonoBehaviour awake called on start up
        /// </summary>
        private void Awake()
        {
            // Set fade image to invisible
            _fadeImage.color = _fadeImage.color.ChangeAlpha(0);
            _fadeImage.gameObject.SetActive(true);

            // Loading screen image
            ToggleLoadingScreen(false);

            DontDestroyOnLoad(gameObject);
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
        internal void ToggleLoadingScreen(bool toggle)
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

            _loadingScreenImage.gameObject.SetActive(toggle);
            _textsParent.gameObject.SetActive(toggle);
        }

        internal void SetProgress(float progress)
        {
            // Set progres in base 100
            progress *= 100;

            // Enable all texts to load whose progress has been passed
            List<SceneChangeText> tempList = new List<SceneChangeText>(_inactiveTextToLoad);
            foreach (SceneChangeText sceneChangeText in tempList)
            {
                if (sceneChangeText.DisplayAtProgress <= progress)
                {
                    // Toggle text based on its index
                    int textIndex = Array.IndexOf(_settings.Texts, sceneChangeText);
                    ToggleTextIndex(textIndex);

                    // Remove loaded text
                    _inactiveTextToLoad.Remove(sceneChangeText);
                }
            }
            
            // TODO
            // Update progress bar slider
        }

        internal void SetTextAtIndex(int index, string text)
        {
            _texts[index].text = text;
        }

        internal void ToggleTextIndex(int index)
        {
            _texts[index].gameObject.SetActive(true);

            void ResetText()
            {
                _texts[index].gameObject.SetActive(false);
                SceneChanger.OnSceneChangeCompleted -= ResetText;
            }
            SceneChanger.OnSceneChangeCompleted += ResetText;
        }

        internal void SetSettings(SceneChangerSettings settings)
        {
            _settings = settings;

            if (_settings.AutoAdjustToResolution)
            {
                _canvasScaler.referenceResolution = _settings.ScreenSize;
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
                textObj.gameObject.SetActive(false);

                _texts.Add(textObj);
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
        /// 
        /// </summary>
        /// <returns></returns>
        private Texture2D GetImage()
        {
            // Checks if Settings has custom images
            if (_settings.HasImages)
            {
                if (_imageQueue == null || _imageQueue.Count == 0)
                {
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
