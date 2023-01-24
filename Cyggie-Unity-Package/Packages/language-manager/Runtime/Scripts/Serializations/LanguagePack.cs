using Cyggie.LanguageManager.Runtime.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cyggie.LanguageManager.Runtime.Serializations
{
    /// <summary>
    /// Model class for a list of translations associated to a language code
    /// </summary>
    [Serializable]
    internal class LanguagePack
    {
        [JsonProperty]
        internal string LanguageCode = "";

        [JsonProperty]
        internal Dictionary<string, string> Translations = new Dictionary<string, string>();

        internal bool Any => Count > 0;

        internal int Count => Translations.Count;

        internal void Add(string key, string value)
        {
            Translations.Add(key, value);
            Translations = Translations.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
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
