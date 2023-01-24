using Cyggie.LanguageManager.Runtime.Settings;
using Cyggie.LanguageManager.Runtime.Utils;
using Cyggie.Main.Runtime.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cyggie.LanguageManager.Runtime.Services
{
    /// <summary>
    /// Service class for managing Language and Localization
    /// </summary>
    public sealed class LanguageService : Service
    {
        private LanguageManagerSettings _settings = null;

        public override void Awake()
        {
            base.Awake();

            // Get the settings at path
            _settings = Resources.Load<LanguageManagerSettings>(LanguageManagerConstants.cSettingsFile);

            if (_settings == null)
            {
                Debug.LogError($"Failed to load Language Manager Settings in Resources folder.");
                return;
            }

            //// Create game object from prefab
            //_loadingScreen = GameObject.Instantiate(_settings.LoadingScreenPrefab);

            //// Hide object
            //_loadingScreen.ToggleLoadingScreen(false, false);

            //// Initialize the settings in the loading screen
            //_loadingScreen.SetSettings(_settings);
        }

        #region Public methods

        //public string Translate(string key)
        //{
        //    return Translate(_selectedPack.LanguageCode, key);
        //}

        //public string Translate(string langCode, string key)
        //{
        //    return _languagePacks.FirstOrDefault(x => x.LanguageCode == langCode).GetTranslation(key);
        //}

        public IEnumerable<string> GetLanguageCodes()
        {
            // Return collection of all existing language codes
            return null;
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