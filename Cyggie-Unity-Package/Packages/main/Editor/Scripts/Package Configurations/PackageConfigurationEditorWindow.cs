using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.Utils.Extensions;
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
        private const string cEditorPrefSelectedTabKey = "CyggiePackageConfigurationSelectedTabIndex";

        private List<PackageConfigurationTab> _tabs = null;
        private PackageConfigurationTab _selectedTab = null;
        private string[] _tabStrings = null;

        private int _selectedTabIndex = 0;
        private Vector2 _tabScrollViewPos = Vector2.zero;

        private ConfigurationSettings _configSettings = null;
        private SerializedObject _serializedObject = null;

        internal static PackageConfigurationEditorWindow Window = null;

        /// <summary>
        /// Initialize the window with tabs
        /// </summary>
        /// <param name="tabs">List of tabs in projects</param>
        internal void Initialize(List<PackageConfigurationTab> tabs)
        {
            Window = this;
            _tabs = tabs;
            _selectedTab = _tabs.FirstOrDefault();

            _tabStrings = _tabs.Select(x => x.ClassName).ToArray();

            _configSettings = Resources.Load<ConfigurationSettings>(ConfigurationSettings.cResourcesPath);
            if (_configSettings == null)
            {
                // Settings not found, create a new one
                Debug.Log($"Couldn't find configuration settings file, creating a new one...");

                _configSettings = CreateInstance<ConfigurationSettings>();

                // Create asset at default folder path
                string directory = $"{ConfigurationSettings.cDefaultResourcesFolderPath}{ConfigurationSettings.cResourcesFolderPath}";
                string configPath = $"{directory}{ConfigurationSettings.cFileName}";

                AssetDatabaseHelper.CreateAsset(_configSettings, configPath, true);

                // Save asset
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"Created configuration settings file at {configPath}.");
            }

            // Initialize all tabs with config settings
            _tabs.ForEach(x =>
            {
                // Initialize the tab with configuration settings
                x.Initialize(_configSettings);
            });

            _serializedObject = new SerializedObject(_configSettings);
            _selectedTabIndex = EditorPrefs.GetInt(cEditorPrefSelectedTabKey, 0);

            if (_selectedTabIndex > _tabs.Count)
            {
                _selectedTabIndex = 0;
            }

            _selectedTab = _tabs[_selectedTabIndex];
        }

        /// <summary>
        /// Draw GUI for the window
        /// </summary>
        internal void OnGUI()
        {
            if (_tabStrings == null || _tabs == null || _selectedTabIndex >= _tabs.Count || _configSettings == null)
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
                SerializedProperty pathProperty = _serializedObject.FindProperty(nameof(ConfigurationSettings.ResourcesPath));
                string oldResourcesPath = pathProperty.stringValue;
                if (EditorGUIHelper.CheckChange(gui: () => pathProperty.stringValue = EditorGUILayout.DelayedTextField("Resources Path", pathProperty.stringValue)))
                {
                    string newResourcesPath = pathProperty.stringValue;
                    string error = "";

                    if (newResourcesPath.EndsWith("/Resources"))
                    {
                        newResourcesPath = $"{newResourcesPath}/";
                    }
                    else if (!newResourcesPath.EndsWith("/Resources/"))
                    {
                        // Add Resources folder with always one "/" before
                        newResourcesPath = newResourcesPath.EndsWith("/") ?
                                           $"{newResourcesPath}Resources/" :
                                           $"{newResourcesPath}/Resources/";
                    }

                    if (!Directory.Exists(newResourcesPath))
                    {
                        Directory.CreateDirectory(newResourcesPath);
                        Debug.Log($"Directory couldn't be found, created a new Resources directory at: {newResourcesPath}");
                    }

                    // Check for error
                    if (string.IsNullOrEmpty(error))
                    {
                        Debug.Log($"Resources Folder changed from \"{oldResourcesPath}\" to \"{newResourcesPath}\"");

                        string oldFolderPath = $"{oldResourcesPath}{ConfigurationSettings.cResourcesFolderPath}";
                        string newFolderPath = $"{newResourcesPath}{ConfigurationSettings.cResourcesFolderPath}";

                        // Move configuration settings file
                        AssetDatabaseHelper.MoveAsset($"{oldFolderPath}{ConfigurationSettings.cFileName}", $"{newFolderPath}{ConfigurationSettings.cFileName}");

                        // Move package configuration settings file
                        foreach (PackageConfigurationTab tab in _tabs)
                        {
                            AssetDatabaseHelper.MoveAsset($"{oldFolderPath}{tab.FileName}", $"{newFolderPath}{tab.FileName}");

                            foreach (string otherPath in tab.SettingsOtherPaths)
                            {
                                string path = otherPath.EndsWith("/") ?
                                              otherPath.Remove(otherPath.Length - 1) :
                                              otherPath;

                                AssetDatabaseHelper.MoveAsset($"{oldFolderPath}{path}", $"{newFolderPath}{path}");
                            }
                        }

                        AssetDatabase.Refresh();
                        pathProperty.stringValue = newResourcesPath;
                        _serializedObject.ApplyModifiedProperties();
                    }
                    // Has error
                    // Revert changes in path
                    else
                    {
                        Debug.LogError($"Failed to change Configuration path to: {newResourcesPath}");
                        Debug.LogError(error);
                        pathProperty.stringValue = oldResourcesPath;
                    }
                }
                EditorGUILayout.Space(5);

                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUILayout.LabelField("Select configuration: ", GUILayout.Width(140));

                    if (EditorGUIHelper.CheckChange(gui: () => _selectedTabIndex = EditorGUILayout.Popup(_selectedTabIndex, _tabStrings)))
                    {
                        EditorPrefs.SetInt(cEditorPrefSelectedTabKey, _selectedTabIndex);
                        _selectedTab = _tabs[_selectedTabIndex];
                    }
                });
                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField(_selectedTab.SettingsType.Name.SplitCamelCase(), EditorStyles.boldLabel);
                _selectedTab.DrawGUI();

                EditorGUILayout.Space(5);
            }, horizontalStyle: GUIStyle.none);
        }

        /// <summary>
        /// Try get a tab by its type <typeparamref name="T"/> in the list of tabs
        /// </summary>
        /// <typeparam name="T">Type of tab</typeparam>
        /// <param name="tab">Outputted tab</param>
        /// <returns>Found?</returns>
        internal bool TryGetTab<T>(out T tab) where T : PackageConfigurationTab
        {
            tab = (T)_tabs.FirstOrDefault(x => x.GetType() == typeof(T));

            if (tab == null)
            {
                Debug.LogError($"Unable to find tab of type: {typeof(T)}.");
            }

            return tab != null;
        }
    }
}
