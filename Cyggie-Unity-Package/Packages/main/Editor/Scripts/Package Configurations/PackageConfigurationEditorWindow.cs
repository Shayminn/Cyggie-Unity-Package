using Cyggie.Main.Editor.Utils.Constants;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Main.Runtime.Utils.Constants;
using Cyggie.Main.Runtime.Utils.Helpers;
using Cyggie.Plugins.Logs;
using System;
using System.Collections.Generic;
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
        private List<AbstractPackageConfigurationTab> _tabs = null;
        private AbstractPackageConfigurationTab _selectedTab = null;
        private string[] _tabStrings = null;

        private int _selectedTabIndex = 0;
        private Vector2 _tabScrollViewPos = Vector2.zero;

        private bool _cIsPressed = false;
        private bool _altIsPressed = false;

        internal static PackageConfigurationEditorWindow Window = null;
        internal ServiceManagerTab ServiceManagerTab = null;
        internal bool ClosedByShortcut = false;

        /// <summary>
        /// Initialize the window with tabs
        /// </summary>
        /// <param name="tabs">List of tabs in projects</param>
        internal void Initialize(List<AbstractPackageConfigurationTab> tabs)
        {
            Window = this;
            _tabs = tabs;
            _selectedTab = _tabs.FirstOrDefault();

            _tabStrings = _tabs.Select(x => x.DropdownName).ToArray();

            // Initialize all tabs
            _tabs.ForEach(x =>
            {
                // Initialize the tab with configuration settings
                x.CallInitialized();
            });

            _selectedTabIndex = EditorPrefs.GetInt(EditorPrefsConstants.cPackageWindowSelectedTabIndex, 0);
            if (_selectedTabIndex >= _tabs.Count)
            {
                _selectedTabIndex = 0;
            }

            _selectedTab = _tabs[_selectedTabIndex];
        }

        /// <summary>
        /// Draw GUI for the window
        /// </summary>
        private void OnGUI()
        {
            if (_tabStrings == null || _tabs == null || _selectedTabIndex >= _tabs.Count)
            {
                Close();
                return;
            }

            if (HandleCloseWindowShortcut()) return;

            // Draw the whole window with a vertical scroll view
            EditorGUIHelper.DrawWithScrollview(ref _tabScrollViewPos, gui: () =>
            {
                EditorGUILayout.Space(5);

                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    if (_tabs.Count > 1)
                    {
                        EditorGUILayout.LabelField("Select configuration: ", GUILayout.Width(140));

                        if (EditorGUIHelper.CheckChange(gui: () => _selectedTabIndex = EditorGUILayout.Popup(_selectedTabIndex, _tabStrings)))
                        {
                            EditorPrefs.SetInt(EditorPrefsConstants.cPackageWindowSelectedTabIndex, _selectedTabIndex);
                            _selectedTab = _tabs[_selectedTabIndex];
                        }
                    }
                });
                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField(_selectedTab.Title, EditorStyles.boldLabel);
                _selectedTab.DrawTab();

                EditorGUILayout.Space(5);
            }, horizontalStyle: GUIStyle.none);
        }

        private void OnDestroy()
        {
            Window = null;
        }

        /// <summary>
        /// Try get settings of <typeparamref name="T"/>
        /// </summary>
        /// <param name="settings">Output settings (null if not found)</param>
        /// <returns>Found?</returns>
        internal bool TryGetSettings<T>(out ServiceConfigurationSO settings) where T : Service
        {
            settings = ServiceManagerTab.Settings.ServiceConfigurations.FirstOrDefault(x => x.ServiceType == typeof(T));
            return settings != null;
        }

        private bool HandleCloseWindowShortcut()
        {
            Event e = Event.current;
            if (e != null)
            {
                switch (e.type)
                {
                    case EventType.KeyDown:
                        {
                            switch (Event.current.keyCode)
                            {
                                case KeyCode.C:
                                    _cIsPressed = true;
                                    break;

                                case KeyCode.LeftAlt:
                                case KeyCode.RightAlt:
                                    _altIsPressed = true;
                                    break;

                            }

                            if (_cIsPressed && _altIsPressed)
                            {
                                EditorMenuItems.ExpectPackageConfigurationsShortcut = true;
                                Close();
                                return true;
                            }
                            break;
                        }

                    case EventType.KeyUp:
                        {
                            switch (Event.current.keyCode)
                            {
                                case KeyCode.C:
                                    _cIsPressed = false;
                                    break;

                                case KeyCode.LeftAlt:
                                case KeyCode.RightAlt:
                                    _altIsPressed = false;
                                    break;

                            }
                            break;
                        }
                }
            }

            return false;
        }

        #region Static methods

        /// <summary>
        /// Automatically adds service configurations on every assembly refresh
        /// </summary>
        [InitializeOnLoadMethod]
        internal static void AutoAddMissingServiceConfigurations()
        {
            // Don't refresh if application is playing
            if (Application.isPlaying) return;

            ServiceManagerSettings settings = ServiceManagerTab.GetServiceManagerSettings();
            RefreshServiceConfigurations(settings);
        }

        /// <summary>
        /// Refresh the list of service configurations <br/>
        /// Automatically creating missing ones
        /// </summary>
        /// <param name="settings">The service manager settings to check the list of configurations</param>
        internal static void RefreshServiceConfigurations(ServiceManagerSettings settings)
        {
            // Refresh the settings to remove any incorrect data
            settings.Refresh();

            // Retrieve all classes that implements ServiceConfiguration
            // that are not present in the Service Manager Settings
            List<Type> missingTypes = TypeHelper.GetAllIsAssignableFrom<ServiceConfigurationSO>(type =>
            {
                return type != typeof(ServiceManagerSettings) && // don't add ServiceManagerSettings
                       !settings.ServiceConfigurations.Any(x => x.GetType() == type); // make sure the service configuration doesn't already exists
            }).ToList();

            if (missingTypes.Count > 0)
            {
                foreach (Type type in missingTypes)
                {
                    ScriptableObject scriptableObj = ScriptableObject.CreateInstance(type);
                    if (scriptableObj is ServiceConfigurationSO configuration)
                    {
                        Log.Debug($"{type.Name}) not found in the Service Manager Settings. Searching for it...", nameof(PackageConfigurationEditorWindow));

                        string folderName = configuration.IsPackageSettings ? FolderConstants.cPackageConfigurations : FolderConstants.cServiceConfigurations;
                        string assetPath = FolderConstants.cAssets +
                                           FolderConstants.cCyggie +
                                           folderName +
                                           configuration.GetType().Name + FileExtensionConstants.cAsset;

                        // Check if the missing type is in the usual folder
                        ServiceConfigurationSO foundConfig = AssetDatabase.LoadAssetAtPath<ServiceConfigurationSO>(assetPath);
                        if (foundConfig != null)
                        {
                            Log.Debug($"{type.Name} found in {assetPath}. Adding it to the Service Manager Settings.");
                            settings.ServiceConfigurations.Add(foundConfig);
                            continue;
                        }

                        // Not found, create a new one
                        Log.Debug($"{type.Name} not found in {assetPath}. Creating a new one...");
                        if (!AssetDatabaseHelper.CreateAsset(scriptableObj, assetPath)) continue;

                        configuration.OnScriptableObjectCreated();
                        settings.ServiceConfigurations.Add(configuration);
                    }
                }

                EditorUtility.SetDirty(settings);
            }
        }

        #endregion
    }
}
