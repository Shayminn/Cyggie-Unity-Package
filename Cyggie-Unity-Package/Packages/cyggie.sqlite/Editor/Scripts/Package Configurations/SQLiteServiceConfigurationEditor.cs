using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Plugins.Editor.Helpers;
using Cyggie.Plugins.SQLite;
using Cyggie.SQLite.Editor.Utils.Styles;
using Cyggie.SQLite.Runtime.ServicesNS;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.SQLite.Editor.Configurations
{
    /// <summary>
    /// Inspector editor for <see cref="SQLiteSettings"/>
    /// </summary>
    [CustomEditor(typeof(SQLiteServiceConfiguration))]
    internal class SQLiteServiceConfigurationEditor : UnityEditor.Editor
    {
        private SQLiteServiceConfiguration _config = null;
        private SerializedProperty _openAllOnStart = null;
        private SQLiteService _service = null;
        private SQLiteDatabase _selectedDb = null;

        private int _selectedDbIndex = -1;
        private bool _deletingDatabase = false;

        /// <inheritdoc/>
        private void OnEnable()
        {
            _config = target as SQLiteServiceConfiguration;

            _service = PackageServiceMono<SQLiteServiceConfiguration>.Create<SQLiteService>();

            // Make sure all connections are open when managing databases
            if (!_config.OpenAllOnStart)
            {
                _service.OpenAllConnections();
            }

            _openAllOnStart = serializedObject.FindProperty(nameof(SQLiteServiceConfiguration.OpenAllOnStart));
        }

        public override void OnInspectorGUI()
        {
            // Settings
            EditorGUILayout.PropertyField(_openAllOnStart, GUIContents.cOpenAllOnStart);
            EditorGUILayout.Space(10);

            DrawDatabaseList();
            EditorGUILayout.Space(10);

            DrawDatabaseControls();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDatabaseList()
        {
            EditorGUILayout.LabelField($"List of Databases ({_service.DbConns.Count})", EditorStyles.boldLabel);

            if (_service.DbConns.Count > 0)
            {
                if (EditorGUILayoutHelper.CheckChange(gui: () => _selectedDbIndex = GUILayout.SelectionGrid(_selectedDbIndex, _service.DbConns.Select(x => x.DatabaseName).ToArray(), 1, GUILayout.Width(250))))
                {
                    if (_selectedDbIndex < 0 || _selectedDbIndex >= _service.DbConns.Count)
                    {
                        _selectedDbIndex = 0;
                    }

                    _selectedDb = _service.DbConns[_selectedDbIndex];
                }
            }
            else
            {
                EditorGUILayout.LabelField("No databases found.");
            }
        }

        private void DrawDatabaseControls()
        {
            if (GUILayout.Button("Create database", GUILayout.Width(130)))
            {
                _service.CreateDatabase();
            }
            EditorGUILayout.Space(10);

            if (_selectedDb != null)
            {
                EditorGUILayoutHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUILayout.LabelField($"Selected database: ", EditorStyles.boldLabel, GUILayout.Width(120));

                    string newName = "";
                    if (EditorGUILayoutHelper.CheckChange(gui:
                        () => newName = EditorGUILayout.DelayedTextField(_selectedDb.DatabaseName, GUILayout.Width(150))))
                    {
                        _selectedDb.ChangeDatabaseName(newName);
                        AssetDatabase.Refresh();
                    }
                });
                EditorGUILayout.Space(2);

                // Draw encrypted field
                bool isEncrypted = _selectedDb.IsEncrypted;
                EditorGUILayoutHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUILayout.LabelField(GUIContents.cIsEncrypted, GUILayout.Width(85));
                    if (EditorGUILayoutHelper.CheckChange(gui:
                            () => isEncrypted = EditorGUILayout.Toggle(_selectedDb.IsEncrypted)))
                    {
                        _selectedDb.IsEncrypted = isEncrypted;
                    }
                });

                // Draw read only field
                GUIHelper.DrawAsReadOnly(_selectedDb.IsEncrypted, gui: () =>
                {
                    bool isReadOnly = _selectedDb.IsReadOnly;
                    EditorGUILayoutHelper.DrawHorizontal(gui: () =>
                    {
                        EditorGUILayout.LabelField(GUIContents.cIsReadOnly, GUILayout.Width(85));
                        if (EditorGUILayoutHelper.CheckChange(gui:
                                () => isReadOnly = EditorGUILayout.Toggle(_selectedDb.IsReadOnly)))
                        {
                            _selectedDb.IsReadOnly = isReadOnly;
                        }
                    });
                    EditorGUILayout.Space(10);
                });

                // Buttons
                EditorGUILayoutHelper.DrawWithConfirm(ref _deletingDatabase,
                    confirmLabel: $"Delete database \"{_selectedDb.DatabaseName}\"?",
                    onConfirm: () =>
                    {
                        if (_service.DeleteDatabase(_selectedDb))
                        {
                            _selectedDb = null;
                            _selectedDbIndex = -1;
                        }
                    },
                    onInactiveGUI: () =>
                    {
                        EditorGUILayoutHelper.DrawHorizontal(gui: () =>
                        {
                            if (_selectedDb.IsEncrypted)
                            {
                                if (GUILayout.Button("Decrypt", GUILayout.Width(75)))
                                {
                                    _selectedDb.Decrypt();
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Encrypt", GUILayout.Width(75)))
                                {
                                    _selectedDb.Encrypt();
                                }
                            }

                            if (GUILayout.Button("Open", GUILayout.Width(50)))
                            {
                                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(_selectedDb.DatabasePath, 1);
                            }
                        });
                        EditorGUILayout.Space(1);

                        if (GUILayout.Button(GUIContents.cCreateBlueprint, GUILayout.Width(130)))
                        {
                            _service.CreateBlueprint(_selectedDb);
                        }
                        EditorGUILayout.Space(1);

                        if (GUILayout.Button("Delete database", GUILayout.Width(130)))
                        {
                            _deletingDatabase = true;
                        }
                    }
                );
            }
        }
    }
}
