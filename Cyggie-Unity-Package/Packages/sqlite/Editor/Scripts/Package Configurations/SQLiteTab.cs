using Cyggie.Main.Editor.Configurations;
using Cyggie.SQLite.Editor.Utils.Styles;
using Cyggie.SQLite.Runtime.Services;
using System;
using UnityEditor;

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

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            _databaseName = _serializedObject.FindProperty(nameof(SQLiteSettings.DatabaseName));
            _readOnly = _serializedObject.FindProperty(nameof(SQLiteSettings.ReadOnly));
            _readAllOnStart = _serializedObject.FindProperty(nameof(SQLiteSettings.ReadAllOnStart));
            _addSToTableName = _serializedObject.FindProperty(nameof(SQLiteSettings.AddSToTableName));
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
