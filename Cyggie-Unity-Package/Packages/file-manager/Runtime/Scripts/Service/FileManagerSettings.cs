using Cyggie.Main.Runtime.Configurations;
using System;
using UnityEngine;

namespace Cyggie.FileManager.Runtime.ServicesNS
{
    /// <summary>
    /// Settings for <see cref="FileManagerService"/>
    /// </summary>
    internal class FileManagerSettings : PackageConfigurationSettings
    {
        internal const string cResourcesPath = ConfigurationSettings.cResourcesFolderPath + nameof(FileManagerSettings);

        [SerializeField]
        internal bool UsePersistentDataPath = true;

        [SerializeField]
        internal string LocalSavePath = "";

        [SerializeField]
        internal string DefaultFileExtension = ".save";

        [SerializeField]
        internal bool Encrypted = true;

        [SerializeField]
        internal string[] FilesToIgnore =
        {
            "Player.log",
            "Player-prev.log"
        };

        public override Type ServiceType => typeof(FileManagerService);
    }
}
