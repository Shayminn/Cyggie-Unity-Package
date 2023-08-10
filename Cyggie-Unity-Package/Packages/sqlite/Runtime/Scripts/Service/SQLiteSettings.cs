using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.Utils.Constants;
using Cyggie.SQLite.Runtime.Utils.Constants;
using UnityEngine;

namespace Cyggie.SQLite.Runtime.ServicesNS
{
    /// <summary>
    /// Settings for <see cref="SQLiteService"/>
    /// </summary>
    internal sealed class SQLiteSettings : PackageConfigurationSettings<SQLiteService>
    {
        internal const string cStreamingAssetsFolderPath = "SQLite/Databases/";

        [SerializeField, Tooltip("Enabling this will open and load all databases right at the start.")]
        internal bool OpenAllOnStart = false;
    }
}
