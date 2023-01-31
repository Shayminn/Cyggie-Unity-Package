using Cyggie.LanguageManager.Runtime.Configurations;
using Cyggie.LanguageManager.Runtime.Serializations;
using Cyggie.Main.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cyggie.LanguageManager.Runtime.Services
{
    /// <summary>
    /// Service class for managing Language and Localization
    /// </summary>
    public sealed class LanguageService : Service
    {
        public Action<string> OnLanguageChanged;

        private LanguageManagerSettings _settings = null;
        private LanguagePack _currentPack = null;

        /// <inheritdoc/>
        protected override void OnInitialized(ServiceConfiguration configuration)
        {
            base.OnInitialized(configuration);

            if (configuration == null || configuration is not LanguageManagerSettings settings)
            {
                Debug.Log($"Language Manager's configuration was not found in the Service Manager Configurations.");
                return;
            }

            // Get the settings at path
            _settings = settings;

            if (_settings.LanguagePacks.Count == 0) return;

            string languageCode = PlayerPrefs.GetString(LanguageManagerSettings.cLanguageCodePrefKey, _settings.DefaultLanguagePack.LanguageCode);
            _currentPack = _settings.LanguagePacks.FirstOrDefault(x => x.LanguageCode == languageCode);

            if (_currentPack == null)
            {
                ChangeLanguagePack(_settings.LanguagePacks.First());
                Debug.LogError($"Couldn't find Language Pack with code: {languageCode}, assigned new language pack: {_currentPack.LanguageCode}");
            }
        }

        #region Public methods

        /// <summary>
        /// Translate value with a given <paramref name="key"/> <br/>
        /// This will use the currently selected language pack automatically
        /// </summary>
        /// <param name="key">Translation key</param>
        /// <returns>Translation value based on key (Empty if not found)</returns>
        public string Translate(string key)
        {
            if (!VerifyInitialization()) return string.Empty;

            return Translate(_currentPack, key);
        }

        /// <summary>
        /// Translate value with a given <paramref name="key"/> on a specific <paramref name="languageCode"/>
        /// </summary>
        /// <param name="languageCode">Target language code</param>
        /// <param name="key">Translation key</param>
        /// <returns>Translation value based on key (Empty if not found)</returns>
        public string Translate(string languageCode, string key)
        {
            if (!VerifyInitialization()) return string.Empty;

            if (TryGetLanguagePack(languageCode, out LanguagePack languagePack))
            {
                return Translate(languagePack, key);
            }

            return string.Empty;
        }

        /// <summary>
        /// Change the selected language to <paramref name="languageCode"/>
        /// </summary>
        /// <param name="languageCode">Language code of language pack</param>
        public void ChangeLanguage(string languageCode)
        {
            if (TryGetLanguagePack(languageCode, out LanguagePack languagePack))
            {
                ChangeLanguagePack(languagePack);
            }
        }

        /// <summary>
        /// Get a list of all language codes available in the game
        /// </summary>
        /// <returns>IEnumerable of language codes</returns>
        public IEnumerable<string> GetLanguageCodes()
        {
            return _settings.LanguagePacks.Select(x => x.LanguageCode);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Change the language pack to the selected one <br/>
        /// Updating the PlayerPrefs for the next time the game is boot up
        /// </summary>
        /// <param name="languagePack"></param>
        private void ChangeLanguagePack(LanguagePack languagePack)
        {
            _currentPack = languagePack;
            PlayerPrefs.SetString(LanguageManagerSettings.cLanguageCodePrefKey, _currentPack.LanguageCode);

            OnLanguageChanged(_currentPack.LanguageCode);
        }

        /// <summary>
        /// Try get a language pack by its code
        /// </summary>
        /// <param name="languageCode">Language code to search</param>
        /// <param name="languagePack">Outputted language pack</param>
        /// <returns>Whether it was found or not</returns>
        private bool TryGetLanguagePack(string languageCode, out LanguagePack languagePack)
        {
            languagePack = _settings.LanguagePacks.FirstOrDefault(x => x.LanguageCode == languageCode);

            if (languagePack == null)
            {
                Debug.LogError($"Trying to get language pack but language code ({languageCode}) was not found.");
            }

            return languagePack != null;
        }

        /// <summary>
        /// Translate within a language pack by a key
        /// </summary>
        /// <param name="languagePack">Target language pack</param>
        /// <param name="key">Translation key</param>
        /// <returns>Translation value based on key (Empty if not found)</returns>
        private string Translate(LanguagePack languagePack, string key)
        {
            if (!languagePack.ContainsKey(key))
            {
                Debug.LogError($"Language pack ({languagePack.LanguageCode}) does not contains any translation for key: {key}.");
                return string.Empty;
            }

            return languagePack.Translations[key];
        }

        /// <summary>
        /// Verify that the Language Service has been initialized correctly
        /// </summary>
        /// <returns></returns>
        private bool VerifyInitialization()
        {
            if (_currentPack == null)
            {
                Debug.LogError($"The {nameof(LanguageService)}'s Language Pack is null.");
                return false;
            }

            return true;
        }

        #endregion
    }
}