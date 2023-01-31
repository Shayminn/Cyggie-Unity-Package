using Newtonsoft.Json;
using System;

namespace Cyggie.LanguageManager.Runtime.Serializations
{
    /// <summary>
    /// A key-value entry to the <see cref="LanguagePack"/>
    /// </summary>
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
