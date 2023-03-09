using Cyggie.Main.Editor.Configurations;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Utils.Extensions;
using Cyggie.SQLite.Editor.Utils;
using Cyggie.SQLite.Editor.Utils.Styles;
using Cyggie.SQLite.Runtime.Services;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cyggie.SQLite.Editor
{
    /// <summary>
    /// Editor Window for tools related to SQLite
    /// </summary>
    internal class SQLiteToolsWindow : EditorWindow
    {
        private readonly GUIContent cWindowTitle = new GUIContent("SQLite Tools");
        private readonly Vector2 cWindowSize = new Vector2(500, 600);

        private SQLiteSettings _settings = null;
        private SQLiteService _service = null;

        private string _blueprintPath = "Cyggie/SQLite/Backups/blueprint.sql";

        private string _quickRunPath = "Cyggie/SQLite/Backups/blueprint.sql";
        private bool _includeSubfolders = true;

        /// <summary>
        /// Menu item in Unity's toolbar
        /// </summary>
        [MenuItem("Cyggie/SQLite/Tools")]
        internal static void OpenTools()
        {
            // Retrieve the SQLite settings scriptable object
            SQLiteToolsWindow window = EditorWindow.GetWindow<SQLiteToolsWindow>();
            window.titleContent = new GUIContent(window.cWindowTitle);
            window.minSize = window.cWindowSize;
            window.maxSize = window.cWindowSize;

            window.Initialize();
            window.Show();
        }

        /// <summary>
        /// Initialize window's fields
        /// </summary>
        private void Initialize()
        {
            // Get sqlite settings or create new if not found
            _settings = Resources.Load<SQLiteSettings>(SQLiteSettings.cResourcesPath)
                .AssignIfNull(PackageConfigurationTab.GetOrCreateSettings<SQLiteSettings>(SQLiteSettings.cResourcesPath));

            _service = new SQLiteService();
        }

        /// <summary>
        /// Draw window's GUI
        /// </summary>
        private void OnGUI()
        {
            if (_settings == null || _service == null)
            {
                Close();
                return;
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Blueprint", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("This creates an sql file that can restore the current state of your database.");

            EditorGUILayout.Space(3);
            EditorGUIUtility.labelWidth = 125;
            if (EditorGUIHelper.CheckChange(gui: () => _blueprintPath = EditorGUILayout.TextField(GUIContents.cBlueprintPath, _blueprintPath)))
            {
                _blueprintPath = _blueprintPath.InsertEndsWith(".sql");
            }

            EditorGUILayout.Space(5);
            if (GUILayout.Button("Create Blueprint (backup)", GUILayout.Width(200)))
            {
                string blueprintPath = $"{Application.dataPath}/{_blueprintPath}";

                // Create directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(blueprintPath));

                // Create blueprint
                _service.CreateBlueprint(blueprintPath, _settings);

                AssetDatabase.Refresh();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Quick run", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("This will allow you to run all sql files in a specified folder to your database.");

            EditorGUILayout.Space(3);
            _quickRunPath = EditorGUILayout.TextField(GUIContents.cQuickRunPath, _quickRunPath);
            string quickRunPath = $"{Application.dataPath}/{_quickRunPath}";

            bool isDirectory = _quickRunPath.EndsWith("/");
            bool canExecute = false;
            if (isDirectory)
            {
                _includeSubfolders = EditorGUILayout.Toggle(GUIContents.cIncludeSubfolders, _includeSubfolders);
                canExecute = Directory.Exists(quickRunPath);
            }
            else
            {
                canExecute = File.Exists(quickRunPath);
            }

            EditorGUILayout.Space(5);
            EditorGUIHelper.DrawAsReadOnly(!canExecute, gui: () =>
            {
                if (GUILayout.Button("Execute files", GUILayout.Width(150)))
                {
                    _service.ExecuteFromPath(quickRunPath, _settings, _includeSubfolders);
                }
            });

            if (!canExecute && !string.IsNullOrEmpty(quickRunPath))
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.HelpBox("File/Directory does not exists.", MessageType.Error);
            }
        }
    }
}
