using System.Collections.Generic;
using UnityEngine;

namespace Cyggie.Main.Runtime.Configurations
{
    /// <summary>
    /// Configuration Setting Scriptable Object used to save settings for all <see cref="PackageConfigurationSettings"/>
    /// </summary>
    internal class ConfigurationSettings : ScriptableObject
    {
        internal const string cFileName = "ConfigurationSettings.asset";
        internal const string cDefaultFolderPath = "Assets/Resources/Cyggie/Package Configurations/";

        [SerializeField, Tooltip("Folder Path for all files related to Cyggie's Package Configurations")]
        internal string ConfigurationsPath = cDefaultFolderPath;
    }
}
