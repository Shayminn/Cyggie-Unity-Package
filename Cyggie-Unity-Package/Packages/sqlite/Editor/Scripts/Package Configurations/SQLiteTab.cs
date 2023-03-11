using Cyggie.Main.Editor.Configurations;
using Cyggie.Main.Runtime.Utils.Extensions;
using Cyggie.SQLite.Editor.Utils.Styles;
using Cyggie.SQLite.Runtime.Services;
using Cyggie.SQLite.Runtime.Utils.Constants;
using Mono.Data.Sqlite;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cyggie.SQLite.Editor.Configurations
{
    /// <summary>
    /// Package tab for <see cref="SQLiteSettings"/> <br/>
    /// Accessible through Cyggie/Package Configurations
    /// </summary>
    internal class SQLiteTab : PackageConfigurationTab
    {
        private SerializedProperty _databaseName = null;
        private SerializedProperty _readOnly = null;
        private SerializedProperty _readAllOnStart = null;
        private SerializedProperty _addSToTableName = null;

        /// <inheritdoc/>
        internal override string ResourcesPath => SQLiteSettings.cResourcesPath;

        /// <inheritdoc/>
        internal override Type SettingsType => typeof(SQLiteSettings);

        /// <inheritdoc/>
        internal override string DropdownName => "SQLite";

        /// <inheritdoc/>
        internal override string Title => "SQLite Settings";

        protected SQLiteSettings Settings => _settings as SQLiteSettings;

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            _databaseName = _serializedObject.FindProperty(nameof(SQLiteSettings.DatabaseName));
            _readOnly = _serializedObject.FindProperty(nameof(SQLiteSettings.ReadOnly));
            _readAllOnStart = _serializedObject.FindProperty(nameof(SQLiteSettings.ReadAllOnStart));
            _addSToTableName = _serializedObject.FindProperty(nameof(SQLiteSettings.AddSToTableName));

            string databaseAbsPath = $"{Application.streamingAssetsPath}/{SQLiteSettings.cStreamingAssetsFolderPath}{Settings.DatabaseName}".InsertEndsWith(FileExtensionConstants.cSQLite);
            if (!File.Exists(databaseAbsPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(databaseAbsPath));

                // This will create a new .sqlite database at path
                SqliteConnection conn = null;
                try
                {
                    conn = new SqliteConnection($"{Constants.cDatabaseURI}{databaseAbsPath}");
                    conn.Open();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed on {nameof(OnInitialized)}, test connection failed, connection string: {conn.ConnectionString}\n, exception: {ex}.");
                    PackageConfigurationEditorWindow.Window.Close();
                    return;
                }

                AssetDatabase.Refresh();
            }
        }

        /// <inheritdoc/>
        protected override void DrawGUI()
        {
            EditorGUILayout.PropertyField(_databaseName, GUIContents.cDatabaseName);

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(_readOnly, GUIContents.cReadOnly);
            EditorGUILayout.PropertyField(_readAllOnStart, GUIContents.cReadAllOnStart);

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(_addSToTableName, GUIContents.cAddSToTableName);
        }
    }
}
