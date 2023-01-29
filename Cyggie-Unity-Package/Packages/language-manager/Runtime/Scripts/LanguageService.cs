using Cyggie.LanguageManager.Runtime.Configurations;
using Cyggie.LanguageManager.Runtime.Serializations;
using Cyggie.Main.Runtime.Services;
using Newtonsoft.Json;
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

        public string Translate(string key)
        {
            if (!VerifyInitialization()) return string.Empty;

            return Translate(_currentPack, key);
        }

        public string Translate(string languageCode, string key)
        {
            if (!VerifyInitialization()) return string.Empty;

            if (TryGetLanguagePack(languageCode, out LanguagePack languagePack))
            {
                return Translate(languagePack, key);
            }

            return string.Empty;
        }

        public void ChangeLanguagePack(string languageCode)
        {
            if (TryGetLanguagePack(languageCode, out LanguagePack languagePack))
            {
                ChangeLanguagePack(languagePack);
            }
        }

        public IEnumerable<string> GetLanguageCodes()
        {
            return _settings.LanguagePacks.Select(x => x.LanguageCode);
        }

        #endregion

        #region Private methods

        private void ChangeLanguagePack(LanguagePack languagePack)
        {
            _currentPack = languagePack;
            PlayerPrefs.SetString(LanguageManagerSettings.cLanguageCodePrefKey, _currentPack.LanguageCode);
        }
        
        private bool TryGetLanguagePack(string languageCode, out LanguagePack languagePack)
        {
            languagePack = _settings.LanguagePacks.FirstOrDefault(x => x.LanguageCode == languageCode);

            if (languagePack == null)
            {
                Debug.LogError($"Trying to get language pack but language code ({languageCode}) was not found.");
            }

            return languagePack != null;
        }

        private string Translate(LanguagePack languagePack, string key)
        {
            if (!languagePack.ContainsKey(key))
            {
                Debug.LogError($"Language pack ({languagePack.LanguageCode}) does not contains any translation for key: {key}.");
                return string.Empty;
            }

            return languagePack.Translations[key];
        }

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

    [Serializable]
    internal class LanguageEntry
    {
        [JsonProperty]
        internal string Key { get; set; } = "";

        [JsonProperty]
        internal string Value { get; set; } = "";

        [JsonConstructor]
        internal LanguageEntry(string key = "", string value = "")
        {
            Key = key;
            Value = value;
        }
    }
}