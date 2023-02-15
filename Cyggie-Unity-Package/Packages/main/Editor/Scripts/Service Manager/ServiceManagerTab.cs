using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// Tab for <see cref="ServiceManager"/>
    /// </summary>
    internal class ServiceManagerTab : PackageConfigurationTab
    {
        // Const labels
        private const string cServiceConfigurationManagerLabel = "Service Configuration Manager";
        private const string cSearchLabel = "Search: ";
        private const string cFilterLabel = "Filter added configs";
        private const string cCreateButtonLabel = "Create new configuration";

        private int _selectedConfigIndex = -1;
        private bool _filterAdded = true;
        private string _logMessage = "";
        private string _search = "";

        private List<Type> _configTypes = null;

        /// <inheritdoc/>
        private ServiceManagerSettings Settings => (ServiceManagerSettings)_settings;

        /// <inheritdoc/>
        internal override Type SettingsType => typeof(ServiceManagerSettings);

        /// <inheritdoc/>
        internal override void OnInitialized()
        {
            // Retrieve all classes that implements ServiceConfiguration
            _configTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())

                // don't add package configuration settings as something instantiable, they are built-in packages and shouldn't be created outside
                .Where(t => !typeof(PackageConfigurationSettings).IsAssignableFrom(t) && typeof(ServiceConfiguration).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();
        }

        /// <inheritdoc/>
        internal override void DrawGUI()
        {
            // Draw settings properties
            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(ServiceManagerSettings.Prefab)));
            });
            EditorGUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(ServiceManagerSettings.ServiceConfigurations)));
            if (EditorGUI.EndChangeCheck())
            {
                // Reset the log message if the list of configurations has changed
                _logMessage = "";

                // Updates the settings object
                _serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space(10);

            // Draw service configuration class selection
            EditorGUILayout.LabelField(cServiceConfigurationManagerLabel, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // Draw filter toggle
            _filterAdded = EditorGUILayout.Toggle(cFilterLabel, _filterAdded);
            EditorGUILayout.Space(5);

            // Draw search bar
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(cSearchLabel, GUILayout.Width(50));
            _search = EditorGUILayout.TextField(_search);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);

            List<Type> filteredTypes = new List<Type>(_configTypes);

            // Filter by search
            if (!string.IsNullOrEmpty(_search))
            {
                filteredTypes.RemoveAll(type => !type.Name.Contains(_search, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by added configurations
            if (_filterAdded)
            {
                filteredTypes.RemoveAll(type => Settings.ServiceConfigurations.Any(x => x != null && x.GetType() == type));
            }

            // Draw selection grid
            _selectedConfigIndex = GUILayout.SelectionGrid(_selectedConfigIndex, filteredTypes.Select(x => x.FullName).ToArray(), 1);
            EditorGUILayout.Space(10);

            bool isError = false;

            // Draw create configuration button
            Type type = _selectedConfigIndex != -1 ?
                        filteredTypes.ElementAt(_selectedConfigIndex) :
                        null;

            ScriptableObject scriptableObj = null;
            if (type != null)
            {
                scriptableObj = ScriptableObject.CreateInstance(type);

                if (scriptableObj is ServiceConfiguration configuration)
                {
                    if (!configuration.Validate())
                    {
                        isError = true;
                        _logMessage = $"Selected configuration ({type}) is invalid. Make sure the {nameof(ServiceConfiguration.ServiceType)} derives from {typeof(Service)}";
                    }
                }
            }

            EditorGUIHelper.DrawAsReadOnly(_selectedConfigIndex == -1 || isError, gui: () =>
            {
                if (GUILayout.Button(cCreateButtonLabel, GUILayout.Width(160)))
                {
                    scriptableObj.name = type.Name;

                    if (scriptableObj is ServiceConfiguration configuration)
                    {
                        if (configuration.Validate())
                        {
                            string uniquePath = AssetDatabase.GenerateUniqueAssetPath($"Assets/{scriptableObj.name}.asset");
                            AssetDatabase.CreateAsset(scriptableObj, uniquePath);

                            _logMessage = $"Created new service configuration of type {type} at \"{uniquePath}\".";
                        }
                    }
                }
            });

            EditorGUILayout.Space(10);

            // Validate settings
            if (!isError)
            {
                if (!Settings.Validate(out string error))
                {
                    isError = true;
                    _logMessage = error;
                }
            }

            // Draw log message
            if (!string.IsNullOrEmpty(_logMessage))
            {
                EditorGUILayout.HelpBox(_logMessage, isError ? MessageType.Error : MessageType.Info);
            }

            EditorGUILayout.Space(10);
            _serializedObject.ApplyModifiedProperties();
        }

        internal void AddConfiguration(ServiceConfiguration config)
        {
            Settings.ServiceConfigurations.Add(config);

            // This will update the current window view
            _serializedObject = new SerializedObject(Settings);

            // This will make sure the changes are saved in the scriptable object
            EditorUtility.SetDirty(Settings);

            _serializedObject.ApplyModifiedProperties();
        }
    }
}
