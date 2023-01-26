using Cyggie.Main.Runtime.Serializations;
using Cyggie.Main.Runtime.Utils.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cyggie.LanguageManager.Runtime.Serializations
{
    /// <summary>
    /// Model class for a list of translations associated to a language code
    /// </summary>
    [Serializable]
    internal class LanguagePack
    {
        [SerializeField, Tooltip("Language code to identify this language pack."), JsonProperty]
        internal string LanguageCode = "";

        [SerializeField, Tooltip("Translations related to this language code."), JsonProperty]
        internal SerializedDictionary<string, string> Translations = new SerializedDictionary<string, string>();

        internal bool Any => Count > 0;

        internal int Count => Translations.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal void Add(string key, string value)
        {
            Translations.Add(key, value);
            Translations = Translations.OrderBy(x => x.Key).ToSerializedDictionary(x => x.Key, y => y.Value);
        }

        internal bool ContainsKey(string key)
        {
            return Translations.Any(x => x.Key == key);
        }

        internal void Delete(string key)
        {
            Translations.Remove(key);
        }

        internal Dictionary<string, string> GetTranslations(string containsStr)
        {
            return Translations.Where(x => x.Key.Contains(containsStr) || x.Value.Contains(containsStr)).ToDictionary(x => x.Key, y => y.Value);
        }
    }
}
