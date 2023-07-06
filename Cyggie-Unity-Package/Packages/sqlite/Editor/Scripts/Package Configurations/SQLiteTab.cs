//using Cyggie.Main.Editor.Configurations;
//using Cyggie.Main.Runtime;
//using Cyggie.Main.Runtime.Utils.Constants;
//using Cyggie.Main.Runtime.Utils.Extensions;
//using Cyggie.SQLite.Editor.Utils.Styles;
//using Cyggie.SQLite.Runtime.ServicesNS;
//using Cyggie.SQLite.Runtime.Utils.Constants;
//using Mono.Data.Sqlite;
//using System;
//using System.IO;
//using UnityEditor;
//using UnityEngine;

//namespace Cyggie.SQLite.Editor.Configurations
//{
//    /// <summary>
//    /// Package tab for <see cref="SQLiteSettings"/> <br/>
//    /// Accessible through Cyggie/Package Configurations
//    /// </summary>
//    internal class SQLiteTab : PackageConfigurationTab<SQLiteService, SQLiteSettings>
//    {
//        internal override string DropdownName => "SQLite";

//        private SerializedProperty _databaseName = null;
//        private SerializedProperty _readOnly = null;
//        private SerializedProperty _readAllOnStart = null;
//        private SerializedProperty _addSToTableName = null;

//        protected SQLiteSettings Settings => _settings as SQLiteSettings;

//        /// <inheritdoc/>
//        protected override void OnInitialized()
//        {
//            base.OnInitialized();

//            _databaseName = _serializedObject.FindProperty(nameof(SQLiteSettings.DatabaseName));
//            _readOnly = _serializedObject.FindProperty(nameof(SQLiteSettings.ReadOnly));
//            _readAllOnStart = _serializedObject.FindProperty(nameof(SQLiteSettings.ReadAllOnStart));
//            _addSToTableName = _serializedObject.FindProperty(nameof(SQLiteSettings.AddSToTableName));

//            string databaseAbsPath = Settings.DatabaseAbsolutePath;
//            if (!File.Exists(databaseAbsPath))
//            {
//                Log.Debug($"Database file not found, creating a new one...", nameof(SQLiteTab));
//                Directory.CreateDirectory(Path.GetDirectoryName(databaseAbsPath));

//                // This will create a new .sqlite database at path
//                SqliteConnection conn = null;
//                try
//                {
//                    conn = new SqliteConnection($"{Constants.cDatabaseURI}{databaseAbsPath}");
//                    conn.Open();
//                    conn.Close();

//                    Log.Debug($"Created a new sqlite database at {databaseAbsPath}", nameof(SQLiteTab));
//                }
//                catch (Exception ex)
//                {
//                    Log.Error($"Test connection failed, connection string: {conn.ConnectionString}\n, exception: {ex}.", nameof(SQLiteTab));
//                    PackageConfigurationEditorWindow.Window.Close();
//                    return;
//                }

//                AssetDatabase.Refresh();
//            }
//        }

//        /// <inheritdoc/>
//        protected override void DrawGUI()
//        {
//            EditorGUILayout.PropertyField(_databaseName, GUIContents.cDatabaseName);

//            EditorGUILayout.Space(5);
//            EditorGUILayout.PropertyField(_readOnly, GUIContents.cReadOnly);
//            EditorGUILayout.PropertyField(_readAllOnStart, GUIContents.cReadAllOnStart);

//            EditorGUILayout.Space(5);
//            EditorGUILayout.PropertyField(_addSToTableName, GUIContents.cAddSToTableName);
//        }
//    }
//}
