using Cyggie.LanguageManager.Runtime.Settings;
using Cyggie.Main.Runtime.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        public override void Awake()
        {
            base.Awake();

            // Get the settings at path
            //_settings = Resources.Load<LanguageManagerSettings>();

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

        #endregion
    }

    [Serializable]
    internal class LanguagePack
    {
        [JsonProperty]
        internal string LanguageCode = "";

        [JsonProperty]
        internal List<LanguageEntry> Translations = new List<LanguageEntry>();

        internal bool Any => Count > 0;

        internal int Count => Translations.Count;

        [JsonConstructor]
        internal LanguagePack(string languageCode, List<LanguageEntry> translations = null)
        {
            LanguageCode = languageCode;

            if (translations != null)
            {
                Translations = translations;
            }
        }

        internal void Add(LanguageEntry entry)
        {
            Translations.Add(entry);
            Translations = Translations.OrderBy(x => x.Key).ToList();
        }

        internal bool ContainsKey(string key)
        {
            return Translations.Any(x => x.Key == key);
        }

        internal void Delete(LanguageEntry entry)
        {
            Translations.RemoveAll(x => x.Key == entry.Key);
        }

        internal List<LanguageEntry> GetTranslations(string containsStr)
        {
            return Translations.Where(x => x.Key.Contains(containsStr) || x.Value.Contains(containsStr)).ToList();
        }

        internal string GetTranslation(string key)
        {
            return Translations.FirstOrDefault(x => x.Key == key).Value;
        }
    }

    [Serializable]
    internal class LanguageEntry
    {
        internal string Key { get; set; } = "";
        internal string Value { get; set; } = "";

        internal LanguageEntry(string key = "", string value = "")
        {
            Key = key;
            Value = value;
        }
    }
}