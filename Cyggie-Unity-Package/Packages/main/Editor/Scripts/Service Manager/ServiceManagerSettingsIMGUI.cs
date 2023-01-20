using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Services
{
    /// <summary>
    /// IMGUI to <see cref="SceneChangerSettings"/>
    /// </summary>
    static class ServiceManagerSettingsIMGUI
    {
        // Settings strings
        private static readonly string cSettingsPath = "Cyggie/ServiceManager";
        private static readonly string cSettingsLabel = "Service Manager";

        private static readonly string cServiceConfigurationManagerLabel = "Service Configuration Manager";
        private static readonly string cSearchLabel = "Search: ";
        private static readonly string cFilterLabel = "Filter added configs";
        private static readonly string cCreateButtonLabel = "Create new configuration";

        private static int _selectedConfigIndex = -1;
        private static bool _filterAdded = true;
        private static string _logMessage = "";
        private static string _search = "";

        private static List<Type> _configTypes = null;

        /// <summary>
        /// Create a settings provider at Project Settings/Cyggie/SceneChanger
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            // Retrieve all classes that implements ServiceConfiguration
            _configTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(ServiceConfiguration)) && !t.IsAbstract).ToList();

            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider(cSettingsPath, SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = cSettingsLabel,

                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = OnSettingsGUI,

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = ServiceManagerSettings.GetKeywords()
            };

            return provider;
        }

        /// <summary>
        /// GUI Handler for creating the settings UI
        /// </summary>
        private static void OnSettingsGUI(string _)
        {
            // Get settings
            SerializedObject serializedSettings = ServiceManagerSettings.SerializedSettings;
            ServiceManagerSettings settings = ServiceManagerSettings.Settings;

            // Draw settings properties
            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(ServiceManagerSettings.Prefab)));
            });
            EditorGUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedSettings.FindProperty(nameof(ServiceManagerSettings.ServiceConfigurations)));
            if (EditorGUI.EndChangeCheck())
            {
                // Reset the log message if the list of configurations has changed
                _logMessage = "";

                // Updates the settings object
                serializedSettings.ApplyModifiedProperties();
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
                filteredTypes.RemoveAll(type => settings.ServiceConfigurations.Any(x => x != null && x.GetType() == type));
            }

            // Draw selection grid
            _selectedConfigIndex = GUILayout.SelectionGrid(_selectedConfigIndex, filteredTypes.Select(x => x.AssemblyQualifiedName).ToArray(), 1);
            EditorGUILayout.Space(10);

            // Draw create configuration button
            EditorGUIHelper.DrawAsReadOnly(_selectedConfigIndex == -1, gui: () =>
            {
                if (GUILayout.Button(cCreateButtonLabel, GUILayout.Width(160)))
                {
                    Type type = filteredTypes.ElementAt(_selectedConfigIndex);
                    ScriptableObject scriptableObj = ScriptableObject.CreateInstance(type);
                    scriptableObj.name = type.Name;

                    string uniquePath = AssetDatabase.GenerateUniqueAssetPath($"Assets/{scriptableObj.name}.asset");
                    AssetDatabase.CreateAsset(scriptableObj, uniquePath);

                    _logMessage = $"Created new service configuration of type {type} at \"{uniquePath}\".";
                }
                EditorGUILayout.Space(10);
            });

            // Validate settings
            bool isError = false;
            if (!settings.Validate(out string error))
            {
                isError = true;
                _logMessage = error;
            }

            // Draw log message
            if (!string.IsNullOrEmpty(_logMessage))
            {
                EditorGUILayout.HelpBox(_logMessage, isError ? MessageType.Error : MessageType.Info);
            }

            serializedSettings.ApplyModifiedProperties();
        }

        //private static void OnInspectorGUI()
        //{

        //}
    }
}
