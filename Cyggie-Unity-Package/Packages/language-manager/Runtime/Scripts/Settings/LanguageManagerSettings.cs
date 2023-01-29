using Cyggie.LanguageManager.Runtime.Serializations;
using Cyggie.LanguageManager.Runtime.Services;
using Cyggie.Main.Runtime.Configurations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cyggie.LanguageManager.Runtime.Configurations
{
    /// <summary>
    /// Settings for <see cref="LanguageService"/>
    /// </summary>
    internal class LanguageManagerSettings : PackageConfigurationSettings
    {
        internal const string cLanguageCodePrefKey = "LanguageManager/LanguageCode";

        [SerializeField, Tooltip("List of language packs, each having a language code and its associated translations.")]
        internal List<LanguagePack> LanguagePacks = new List<LanguagePack>();

        [SerializeField, Tooltip("The language pack that should be used by default.")]
        internal LanguagePack DefaultLanguagePack = null;

        public override Type ServiceType => typeof(LanguageService);

#if UNITY_EDITOR

        internal const string cLanguageFolderPath = "Language Manager/";

        [SerializeField, Tooltip("Whether some debug logs should be displayed (Editor only).")]
        internal bool DebugLogs = true;

        [SerializeField, HideInInspector]
        internal string DataPath = "";

        internal override void Initialize(ConfigurationSettings configSettings)
        {
            DataPath = configSettings.ConfigurationsPath + cLanguageFolderPath;
        }
#endif
    }
}
