using Cyggie.Main.Editor.Utils.Constants;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Main.Runtime.Utils.Constants;
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
        private const string cEditorPrefSelectedTabKey = "CyggiePackageConfigurationSelectedTabIndex";

        private List<AbstractPackageConfigurationTab> _tabs = null;
        private AbstractPackageConfigurationTab _selectedTab = null;
        private string[] _tabStrings = null;

        private int _selectedTabIndex = 0;
        private Vector2 _tabScrollViewPos = Vector2.zero;

        internal static PackageConfigurationEditorWindow Window = null;
        internal ServiceManagerTab ServiceManagerTab = null;

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

            _selectedTabIndex = EditorPrefs.GetInt(cEditorPrefSelectedTabKey, 0);
            if (_selectedTabIndex >= _tabs.Count)
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
            if (_tabStrings == null || _tabs == null || _selectedTabIndex >= _tabs.Count)
            {
                Close();
                return;
            }

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
                            EditorPrefs.SetInt(cEditorPrefSelectedTabKey, _selectedTabIndex);
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
            // Remove all null references
            settings.ServiceConfigurations.RemoveAll(config => config == null);

            // Retrieve all classes that implements ServiceConfiguration
            // that are not present in the Service Manager Settings
            List<Type> _missingTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(ServiceConfigurationSO).IsAssignableFrom(type) && !type.IsAbstract && type != typeof(ServiceManagerSettings))
                .Where(type => !settings.ServiceConfigurations.Any(x => x.GetType() == type))
                .ToList();

            if (_missingTypes.Count > 0)
            {
                foreach (Type type in _missingTypes)
                {
                    Debug.Log($"[Cyggie.Main] Service configuration ({type.Name}) not found. Creating it...");
                    ScriptableObject scriptableObj = ScriptableObject.CreateInstance(type);

                    if (scriptableObj is ServiceConfigurationSO configuration)
                    {
                        string folderName = configuration.IsPackageSettings ? FolderConstants.cPackageConfigurations : FolderConstants.cServiceConfigurations;
                        string assetPath = FolderConstants.cAssets +
                                           FolderConstants.cCyggie +
                                           folderName +
                                           configuration.GetType().Name + FileExtensionConstants.cAsset;

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
