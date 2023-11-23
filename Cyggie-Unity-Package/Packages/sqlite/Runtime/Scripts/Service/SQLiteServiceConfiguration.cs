using Cyggie.Main.Runtime.ServicesNS.ScriptableObjects;
using UnityEngine;

namespace Cyggie.SQLite.Runtime.ServicesNS
{
    /// <summary>
    /// Settings for <see cref="SQLiteService"/>
    /// </summary>
    [CreateAssetMenu(menuName ="SQLiteService")]
    public class SQLiteServiceConfiguration : PackageServiceConfiguration
    {
        public const string cStreamingAssetsFolderPath = "SQLite/Databases/";

        [SerializeField, Tooltip("Enabling this will open and load all databases right at the start.")]
        public bool OpenAllOnStart = false;
    }
}
