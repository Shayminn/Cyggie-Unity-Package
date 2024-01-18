using Cyggie.Main.Editor.Utils.Constants;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Main.Runtime.ServicesNS.ScriptableObjects;
using Cyggie.Main.Runtime.Utils.Constants;
using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Interfaces;
using Cyggie.Plugins.Utils.Constants;
using Cyggie.Plugins.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Windows
{
    /// <summary>
    /// Editor window for Package Configurations, managing all existing tabs in project <see cref="PackageConfigurationTab"/>
    /// </summary>
    internal class ServiceConfigurationsEditorWindow : EditorWindow
    {
        // For shortcut
        private bool _cIsPressed = false;
        private bool _altIsPressed = false;

        private List<ServiceConfigurationSO> _configs = new List<ServiceConfigurationSO>();
        private ServiceConfigurationSO _selectedConfig = null;
        private UnityEditor.Editor _selectedConfigEditor = null;

        private Vector2 _tabScrollViewPos = Vector2.zero;
        private int _numServiceConfigs = 0;
        private int _selectedTabIndex = 0;

        internal ServiceManagerSettings ServiceManagerSettings { get; set; } = null;

        /// <summary>
        /// Initialize the window
        /// </summary>
        private void OnEnable()
        {
            LoadServiceManagerSettings();
            if (ServiceManagerSettings == null) return;

            SetConfigs(initialize: true);

            // Load last tab index
            int savedIndex = EditorPrefs.GetInt(EditorPrefsConstants.cPackageWindowSelectedTabIndex, 0);
            SelectTab(savedIndex);
        }

        /// <summary>
        /// Draw GUI for the window
        /// </summary>
        private void OnGUI()
        {
            if (ServiceManagerSettings == null ||
                _configs.Count == 0 ||
                HandleCloseWindowShortcut())
            {
                Close();
                return;
            }

            if (_numServiceConfigs != ServiceManagerSettings.ServiceConfigurations.Count)
            {
                // Update the configs based on the changes in Service configurations
                // This will update the number of tabs as well
                SetConfigs(initialize: false);
            }

            // Draw the whole window with a vertical scroll view
            EditorGUIHelper.DrawWithScrollview(ref _tabScrollViewPos, gui: () =>
            {
                EditorGUILayout.Space(5);
                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    if (_configs.Count > 1)
                    {
                        EditorGUILayout.LabelField("Select configuration: ", GUILayout.Width(140));

                        int newIndex = 0;
                        if (EditorGUIHelper.CheckChange(gui: () => newIndex = EditorGUILayout.Popup(_selectedTabIndex, _configs.Select(x => x == null ? "Null" : x.GetType().Name.SplitCamelCase()).ToArray())))
                        {
                            SelectTab(newIndex);
                        }
                    }
                });
                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField(_selectedConfig.name.SplitCamelCase(), EditorStyles.boldLabel);

                // Draw config editor GUI
                _selectedConfigEditor.OnInspectorGUI();

                // Check if there are any changes when editing the Service Manager Settings
                if (_selectedTabIndex == 0 && GUI.changed)
                {
                    // Update the configs based on the changes in Service configurations
                    // This will update the number of tabs as well
                    SetConfigs(initialize: false);
                }

                EditorGUILayout.Space(5);
            }, horizontalStyle: GUIStyle.none);

            int newNumServiceConfigs = ServiceManagerSettings.ServiceConfigurations.Count;
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

        private void LoadServiceManagerSettings()
        {
            ServiceManagerSettings = Resources.Load<ServiceManagerSettings>(ServiceManagerSettings.cResourcesPath);

            // Create settings object if not found
            if (ServiceManagerSettings == null)
            {
                Log.Debug($"{nameof(ServiceManagerSettings)} not found. Creating it...", nameof(ServiceConfigurationsEditorWindow));
                ServiceManagerSettings = (ServiceManagerSettings) ScriptableObject.CreateInstance(typeof(ServiceManagerSettings));

                string cSettingsAssetPath = FolderConstants.cAssets +
                                                   FolderConstants.cCyggieResources +
                                                   FolderConstants.cCyggie +
                                                   nameof(ServiceManagerSettings) + FileExtensionConstants.cAsset;

                // Create asset
                if (!AssetDatabaseHelper.CreateAsset(ServiceManagerSettings, cSettingsAssetPath))
                {
                    Log.Error($"Unable to load service manager settings.", nameof(ServiceConfigurationsEditorWindow));
                }
            }

            if (ServiceManagerSettings.Prefab == null)
            {
                ServiceManagerSettings.Prefab = AssetDatabase.LoadAssetAtPath<ServiceManagerMono>(ServiceManagerSettings.cPrefabPath);
                AssetDatabase.SaveAssets();
            }

            if (ServiceManagerSettings.EmptyPrefab == null)
            {
                ServiceManagerSettings.EmptyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ServiceManagerSettings.cEmptyPrefabPath);
                AssetDatabase.SaveAssets();
            }
        }

        private void SetConfigs(bool initialize)
        {
            List<Type> serviceConfigTypes = new List<Type>();
            // Verify that all Package Service Configs exist
            foreach (ServiceIdentifier identifier in ServiceManagerSettings.ServiceIdentifiers)
            {
                // Check if identifier exists
                if (identifier == null) continue;

                // Get type of identifier
                Type type = Type.GetType(identifier.AssemblyName);
                if (type == null) continue;

                // Check if type is of PackageServiceMono
                if (type.IsSubclassOfGenericType(typeof(PackageServiceMono<>)))
                {
                    // Check if PackageServiceMono's config is assigned
                    IServiceWithConfiguration serviceWithConfig = (IServiceWithConfiguration) Activator.CreateInstance(type);
                    serviceConfigTypes.Add(serviceWithConfig.ConfigurationType);

                    if (!ServiceManagerSettings.ServiceConfigurations.Any(x => x != null && serviceWithConfig.ConfigurationType == x.GetType()))
                    {
                        string path = $"{FolderConstants.cAssets}{FolderConstants.cCyggieServiceConfigurations}";

                        // Try and load existing package service config if any
                        PackageServiceConfiguration packageServiceConfig = (PackageServiceConfiguration) AssetDatabase.LoadAssetAtPath($"{path}{serviceWithConfig.ConfigurationType.Name}{FileExtensionConstants.cAsset}", serviceWithConfig.ConfigurationType);

                        // Create scriptable object if not found
                        if (packageServiceConfig == null)
                        {
                            packageServiceConfig = (PackageServiceConfiguration) ScriptableObject.CreateInstance(serviceWithConfig.ConfigurationType);

                            // Create asset
                            AssetDatabaseHelper.CreateAsset(packageServiceConfig, $"{path}{serviceWithConfig.ConfigurationType.Name}{FileExtensionConstants.cAsset}");
                        }

                        ServiceManagerSettings.ServiceConfigurations.Add(packageServiceConfig);
                    }
                }
            }

            // Verify that all Package Service Configs have their associated service
            ServiceManagerSettings.ServiceConfigurations.RemoveAll((serviceConfig) =>
            {
                if (serviceConfig == null) return true;

                bool toRemove = !serviceConfigTypes.Any(x => x == serviceConfig.GetType());
                if (toRemove)
                {
                    string path = $"{FolderConstants.cAssets}{FolderConstants.cCyggieServiceConfigurations}";
                    string assetPath = $"{path}{serviceConfig.name}{FileExtensionConstants.cAsset}";
                    if (File.Exists(path))
                    {
                        AssetDatabaseHelper.DeleteAsset(assetPath);
                    }
                }

                return toRemove;
            });

            _configs = new List<ServiceConfigurationSO>() { ServiceManagerSettings };
            _configs.AddRange(ServiceManagerSettings.ServiceConfigurations);
            _numServiceConfigs = ServiceManagerSettings.ServiceConfigurations.Count;

            // Validate data only when initializing
            if (!initialize) return;

            int removed = _configs.RemoveAll(x => x == null);
            if (removed > 0)
            {
                Log.Error("Found null/missing reference to a service configuration from the Service Manager settings. Please remove or reassign it.", nameof(ServiceConfigurationsEditorWindow));
            }

            int count = _configs.Count;
            _configs = _configs.Distinct().ToList();
            if (count != _configs.Count)
            {
                Log.Error("Found duplicate reference of service configuration from the Service Manager settings. Please remove it.", nameof(ServiceConfigurationsEditorWindow));
            }
        }

        private void SelectTab(int index)
        {
            if (_configs.Count == 0) return;

            if (index >= _configs.Count)
            {
                index = 0;
            }

            _selectedTabIndex = index;
            _selectedConfig = _configs[index];
            _selectedConfigEditor = UnityEditor.Editor.CreateEditor(_selectedConfig);

            // Save it to editor prefs for the next time this is opened
            EditorPrefs.SetInt(EditorPrefsConstants.cPackageWindowSelectedTabIndex, _selectedTabIndex);
        }
    }
}
