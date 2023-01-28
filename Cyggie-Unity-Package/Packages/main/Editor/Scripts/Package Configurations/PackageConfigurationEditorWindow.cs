using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.Utils.Extensions;
using Cyggie.Main.Runtime.Utils.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// Editor window for Package Configurations, managing all existing tabs in project <see cref="PackageConfigurationTab"/>
    /// </summary>
    internal class PackageConfigurationEditorWindow : EditorWindow
    {
        // Const error messages for Configuration Setting path
        private const string cNotADirectory = "Configuration Path must be a valid Directory/Folder path.";
        private const string cNotInResources = "Configuration Path must be contained within a Resources folder.";

        private List<PackageConfigurationTab> _tabs = null;
        private PackageConfigurationTab _selectedTab = null;
        private string[] _tabStrings = null;

        private int _selectedTabIndex = 0;
        private Vector2 _tabScrollViewPos = Vector2.zero;

        private ConfigurationSettings _configSettings = null;
        private SerializedObject _serializedObject = null;

        /// <summary>
        /// Initialize the window with tabs
        /// </summary>
        /// <param name="tabs">List of tabs in projects</param>
        internal void Initialize(List<PackageConfigurationTab> tabs)
        {
            _tabs = tabs;
            _selectedTab = _tabs.FirstOrDefault();

            _tabStrings = _tabs.Select(x => x.ClassName).ToArray();

            if (FileHelper.TryGetRelativePath(ConfigurationSettings.cFileName, out string path, suppressError: true))
            {
                _configSettings = AssetDatabase.LoadAssetAtPath<ConfigurationSettings>(path);
            }
            else
            {
                // Settings not found, create a new one
                Debug.Log($"Couldn't find configuration settings file, creating a new one...");

                _configSettings = CreateInstance<ConfigurationSettings>();

                // Create asset
                Directory.CreateDirectory(ConfigurationSettings.cDefaultFolderPath);
                AssetDatabase.CreateAsset(_configSettings, $"{ConfigurationSettings.cDefaultFolderPath}{ConfigurationSettings.cFileName}");

                // Save asset
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            _serializedObject = new SerializedObject(_configSettings);

            // Initialize all tabs with config settings
            _tabs.ForEach(x => x.Initialize(_configSettings));
        }

        /// <summary>
        /// Draw GUI for the window
        /// </summary>
        internal void OnGUI()
        {
            if (_tabStrings == null || _tabs == null || _selectedTabIndex >= _tabs.Count)
            {
                Close();
                return;
            }

            // Draw the whole window with a vertical scroll view
            EditorGUIHelper.DrawWithScrollview(ref _tabScrollViewPos, gui: () =>
            {
                EditorGUILayout.Space(5);

                EditorGUIUtility.labelWidth = 140;

                // Draw configuration path text field
                SerializedProperty pathProperty = _serializedObject.FindProperty(nameof(ConfigurationSettings.ConfigurationsPath));
                string oldPath = pathProperty.stringValue;
                if (EditorGUIHelper.CheckChange(gui: () => pathProperty.stringValue = EditorGUILayout.DelayedTextField("Configurations Path", pathProperty.stringValue)))
                {
                    string newPath = pathProperty.stringValue;
                    string error = "";

                    // Automatically add the / at the end
                    if (!newPath.EndsWith('/'))
                    {
                        newPath = $"{newPath}/";
                        pathProperty.stringValue = newPath;

                        if (oldPath == newPath)
                        {
                            Debug.Log("Path didn't change.");
                            return;
                        }
                    }

                    if (!Directory.Exists(newPath))
                    {
                        error = cNotADirectory;
                    }
                    else if (!newPath.Contains("/Resources/"))
                    {
                        error = cNotInResources;
                    }

                    // Check for error
                    if (string.IsNullOrEmpty(error))
                    {
                        Debug.Log($"Configuration Folder changed from \"{oldPath}\" to \"{newPath}\"");

                        // Move configuration settings file
                        AssetDatabase.MoveAsset($"{oldPath}{ConfigurationSettings.cFileName}", $"{newPath}{ConfigurationSettings.cFileName}");

                        // Move package configuration settings file
                        foreach (PackageConfigurationTab tab in _tabs)
                        {
                            AssetDatabase.MoveAsset($"{oldPath}{tab.FileName}", $"{newPath}{tab.FileName}");

                            foreach (string otherPath in tab.SettingsOtherPaths)
                            {
                                AssetDatabase.MoveAsset($"{oldPath}{otherPath}", $"{newPath}{otherPath}");
                            }
                        }

                        AssetDatabase.Refresh();
                        _serializedObject.ApplyModifiedProperties();
                    }
                    // Has error
                    // Revert changes in path
                    else
                    {
                        Debug.LogError($"Failed to change Configuration path to: {newPath}");
                        Debug.LogError(error);
                        pathProperty.stringValue = oldPath;
                    }
                }
                EditorGUILayout.Space(5);

                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUILayout.LabelField("Select configuration: ", GUILayout.Width(140));

                    if (EditorGUIHelper.CheckChange(gui: () => _selectedTabIndex = EditorGUILayout.Popup(_selectedTabIndex, _tabStrings)))
                    {
                        _selectedTab = _tabs[_selectedTabIndex];
                    }
                });
                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField(_selectedTab.SettingsType.Name.SplitCamelCase(), EditorStyles.boldLabel);
                _selectedTab.DrawGUI();

                EditorGUILayout.Space(5);
            }, horizontalStyle: GUIStyle.none);
        }
    }
}
