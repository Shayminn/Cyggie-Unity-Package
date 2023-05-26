using Cyggie.Main.Runtime.Configurations;
using Cyggie.SQLite.Runtime.Utils.Constants;
using System;
using UnityEngine;

namespace Cyggie.SQLite.Runtime.ServicesNS
{
    /// <summary>
    /// Settings for <see cref="SQLiteService"/>
    /// </summary>
    internal sealed class SQLiteSettings : PackageConfigurationSettings
    {
        internal const string cResourcesPath = ConfigurationSettings.cResourcesFolderPath + nameof(SQLiteSettings);
        internal const string cStreamingAssetsFolderPath = ConfigurationSettings.cStreamingAssetsFolderPath + "SQLite/";

        [SerializeField]
        internal string DatabaseName = Constants.cDefaultDatabaseName;

        [SerializeField]
        internal bool ReadOnly = false;

        [SerializeField]
        internal bool ReadAllOnStart = false;

        [SerializeField]
        internal bool AddSToTableName = true;

        internal string DatabasePath => cStreamingAssetsFolderPath + DatabaseName;

        public override Type ServiceType => typeof(SQLiteService);
    }
}
