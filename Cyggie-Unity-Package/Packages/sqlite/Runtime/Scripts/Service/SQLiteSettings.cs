using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.Utils.Constants;
using Cyggie.SQLite.Runtime.Utils.Constants;
using System;
using UnityEngine;

namespace Cyggie.SQLite.Runtime.ServicesNS
{
    /// <summary>
    /// Settings for <see cref="SQLiteService"/>
    /// </summary>
    internal sealed class SQLiteSettings : PackageConfigurationSettings<SQLiteService>
    {
        internal const string cStreamingAssetsFolderPath = "SQLite/";

        [SerializeField]
        internal string DatabaseName = Constants.cDefaultDatabaseName;

        [SerializeField]
        internal bool ReadOnly = false;

        [SerializeField]
        internal bool ReadAllOnStart = false;

        [SerializeField]
        internal bool AddSToTableName = true;

        internal string DatabaseAbsolutePath => FolderConstants.cAssets + 
                                                FolderConstants.cCyggieStreamingAssets + 
                                                DatabasePath;

        internal string DatabasePath => cStreamingAssetsFolderPath +
                                        DatabaseName + FileExtensionConstants.cSQLite;
    }
}
