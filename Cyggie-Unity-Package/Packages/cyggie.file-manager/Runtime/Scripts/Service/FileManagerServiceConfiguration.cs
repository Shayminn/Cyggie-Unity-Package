using Cyggie.Main.Runtime.ServicesNS.ScriptableObjects;
using UnityEngine;

namespace Cyggie.FileManager.Runtime.ServicesNS
{
    /// <summary>
    /// Settings for <see cref="FileManagerService"/>
    /// </summary>
    public class FileManagerServiceConfiguration : PackageServiceConfiguration
    {
        [SerializeField, Tooltip("Whether the service should use Unity's persistent data path.")]
        public bool UsePersistentDataPath = true;

        [SerializeField, Tooltip("When UsePersistentDataPath is false, the local save path to use.")]
        public string LocalSavePath = "";

        [SerializeField, Tooltip("Default file extension for save files.")]
        public string DefaultFileExtension = ".save";

        [SerializeField, Tooltip("Whether the save file should be encrypted.")]
        public bool Encrypted = true;

        [SerializeField, Tooltip("List of files to ignore from the path.")]
        public string[] FilesToIgnore =
        {
            "Player.log",
            "Player-prev.log"
        };
    }
}
