using Cyggie.Main.Runtime.Configurations;
using System;
using UnityEngine;

namespace Cyggie.FileManager.Runtime.Services
{
    /// <summary>
    /// Settings for <see cref="FileManagerService"/>
    /// </summary>
    internal class FileManagerSettings : PackageConfigurationSettings
    {
        internal const string cResourcesPath = ConfigurationSettings.cResourcesFolderPath + nameof(FileManagerSettings);

        [Tooltip("Whether to use Unity's persistent data path (\"C:\\Users\\[User]\\AppData\\LocalLow\\[CompanyName]\\[GameName]\\\") or a local path")]
        public bool UsePersistentDataPath = true;

        [Tooltip("The local save path to use (when UsePersistentDataPath is not enabled)")]
        public string LocalSavePath = "";

        [Tooltip("The default file extension to use for all saved files")]
        public string DefaultFileExtension = ".save";

        [Tooltip("Whether saved files are encrypted")]
        public bool Encrypted = true;

        [Tooltip("List of files to ignore from being read in the folder path")]
        public string[] FilesToIgnore =
        {
            "Player.log",
            "Player-prev.log"
        };

        public override Type ServiceType => typeof(FileManagerService);
    }
}
