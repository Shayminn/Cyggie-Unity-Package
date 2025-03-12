using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Plugins.Logs;
using Cyggie.Plugins.SQLite;
using Cyggie.Plugins.Utils.Helpers;
using Cyggie.SQLite.Runtime.Utils.Constants;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Cyggie.SQLite.Runtime.ServicesNS
{
    /// <summary>
    /// Service for managing an SQLite database <br/>
    /// Execute SQLite queries <br/>
    /// Uses model class <see cref="SQLiteObject"/>
    /// </summary>
    public class SQLiteService : PackageServiceMono<SQLiteServiceConfiguration>
    {
        private readonly List<SQLiteDatabase> _dbConns = new List<SQLiteDatabase>();
        internal List<SQLiteDatabase> DbConns => _dbConns;

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if (Configuration == null) return;

            // Open all databases at path
            if (Configuration.OpenAllOnStart)
            {
                OpenAllConnections();
            }
        }

#nullable enable
        /// <summary>
        /// Get an SQLiteDatabase by its name <br/>
        /// This will automatically create a new connection to the database path if not found <br/>
        /// This will also automatically create a new database if no existing database was found <br/>
        /// </summary>
        /// <param name="databaseName">Name of the database</param>
        /// <returns>SQLiteDatabase connection (null if Create fails)</returns>
        public SQLiteDatabase? this[string databaseName]
        {
            get
            {
                TryGetDatabase(databaseName, out SQLiteDatabase? db);
                return db;
            }
        }

        /// <summary>
        /// Try get an SQLiteDatabase by its name <br/>
        /// This will automatically create a new connection to the database path if not found <br/>
        /// This will also automatically create a new database if no existing database was found <br/>
        /// </summary>
        /// <param name="databaseName">Name of the database</param>
        /// <param name="db">Output SQLiteDatabase connection (null if Open and Create fails)</param>
        /// <returns>Success?</returns>
        public bool TryGetDatabase(string databaseName, out SQLiteDatabase? db)
        {
            db = _dbConns.FirstOrDefault(x => x.DatabaseName == databaseName);

            if (db == null)
            {
                // Open new database connection at path
                string path = $"{FolderConstants.cSQLiteStreamingAssets}{databaseName}{FileExtensionConstants.cSQLite}";
                if (!SQLiteDatabase.TryOpen(path, out db)) return false;
            }

            return db != null;
        }
#nullable disable

        #region Internals

#if UNITY_EDITOR

        /// <summary>
        /// Create a new database at the Streaming assets path 
        /// </summary>
        /// <returns>Created sqlite database</returns>
        internal SQLiteDatabase CreateDatabase()
        {
            string path = $"{FolderConstants.cSQLiteStreamingAssets}{Constants.cDefaultDatabaseName}{FileExtensionConstants.cSQLite}";
            if (SQLiteDatabase.TryCreate(path, out SQLiteDatabase db) && db != null)
            {
                _dbConns.Add(db);
                AssetDatabase.Refresh();
            }

            return db;
        }

        /// <summary>
        /// Delete an existing database, removing the file from the Streaming assets path
        /// </summary>
        /// <param name="db">SQLite database to delete</param>
        /// <returns>Success?</returns>
        internal bool DeleteDatabase(SQLiteDatabase db)
        {
            if (db == null)
            {
                Log.Error("Unable to delete database, argument is null.", nameof(SQLiteService));
                return false;
            }

            if (!_dbConns.Contains(db))
            {
                Log.Error($"Unable to find database to delete: {db.DatabaseName}", nameof(SQLiteService));
                return false;
            }

            db.Delete();
            _dbConns.Remove(db);
            AssetDatabase.Refresh();

            return true;
        }

        /// <summary>
        /// Create a blueprint of the database <br/>
        /// This blueprint can be used to recreate the whole database
        /// </summary>
        /// <returns>Success?</returns>
        internal bool CreateBlueprint(SQLiteDatabase db)
        {
            string uniquePath = FileHelper.GenerateUniquePath(
                $"{FolderConstants.cSQLiteBlueprints}{db.DatabaseName}{Constants.cBlueprintSuffix}{FileExtensionConstants.cSQL}"
            );

            string blueprint = db.CreateBlueprint(uniquePath);
            AssetDatabase.Refresh();

            return !string.IsNullOrEmpty(blueprint);
        }

#endif

        /// <summary>
        /// Open connections to all SQLite database at the Streaming assets path
        /// </summary>
        /// <returns>Number of database connections opened</returns>
        internal int OpenAllConnections()
        {
            foreach (string dbPath in Directory.EnumerateFiles(FolderConstants.cSQLiteStreamingAssets, $"*{FileExtensionConstants.cSQLite}"))
            {
                if (SQLiteDatabase.TryOpen(dbPath, out SQLiteDatabase db) && db != null)
                {
                    _dbConns.Add(db);
                }
            }

            return _dbConns.Count;
        }

        #endregion
    }
}
