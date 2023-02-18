using System.Collections.Generic;
using UnityEngine;

namespace Cyggie.Main.Runtime.Configurations
{
    /// <summary>
    /// Configuration Setting Scriptable Object used to save settings for all <see cref="PackageConfigurationSettings"/>
    /// </summary>
    internal class ConfigurationSettings : ScriptableObject
    {
        internal const string cDefaultResourcesFolderPath = "Assets/Resources/";

        internal const string cResourcesFolderPath = "Cyggie/Package Configurations/";

        /// <summary>
        /// Resources path of the Configuration Settings Scriptable object
        /// </summary>
        internal const string cResourcesPath = cResourcesFolderPath + nameof(ConfigurationSettings);

        internal const string cFileName = "ConfigurationSettings.asset";

        [SerializeField, Tooltip("Folder Path for all files related to Cyggie's Package Configurations")]
        internal string ResourcesPath = cDefaultResourcesFolderPath;
    }
}
