using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.Utils.Extensions;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// Abstract class for creating a Package Configuration Tab used with <see cref="PackageConfigurationSettings"/> by <see cref="PackageConfigurationEditorWindow"/>
    /// </summary>
    internal abstract class PackageConfigurationTab
    {
        /// <summary>
        /// Settings object
        /// </summary>
        protected PackageConfigurationSettings _settings = null;

        /// <summary>
        /// Serialized object of settings
        /// </summary>
        protected SerializedObject _serializedObject = null;

        /// <summary>
        /// File name to the settings (includes extension .asset)
        /// </summary>
        internal string FileName => $"{SettingsType.Name}.asset";

        /// <summary>
        /// Class name to the tab (used to display in Window selection popup)
        /// </summary>
        internal string ClassName => GetType().Name.Replace("Tab", "").SplitCamelCase();

        /// <summary>
        /// Path to the asset from the resources folder
        /// </summary>
        internal abstract string ResourcesPath { get; }

        /// <summary>
        /// Type of settings that inherits <see cref="PackageConfigurationSettings"/>
        /// </summary>
        internal abstract Type SettingsType { get; }

        /// <summary>
        /// Any other file paths that the settings require <br/>
        /// The file paths referenced here will be moved altogether with <see cref="ConfigurationSettings.ResourcesPath"/> <br/>
        /// </summary>
        internal virtual string[] SettingsOtherPaths { get; } = { };

        /// <summary>
        /// Initialize the Tab by retrieve it's associated settings
        /// </summary>
        /// <param name="configSettings">Configuration Settings applied for all Package Configuration Settings</param>
        internal void Initialize(ConfigurationSettings configSettings)
        {
            // Get settings
            string assetPath = $"{configSettings.ResourcesPath}{ResourcesPath}.asset";
            _settings = (PackageConfigurationSettings) AssetDatabase.LoadAssetAtPath(assetPath, SettingsType);

            if (_settings == null)
            {
                // Settings not found, create a new one
                Debug.Log($"Couldn't find settings file for tab: {GetType().Name}, creating a new one...");

                // Create object instance
                _settings = (PackageConfigurationSettings) ScriptableObject.CreateInstance(SettingsType);
                _settings.Initialize(configSettings);

                // Add Package Configuration Settings as a ServiceConfiguration
                // Only for non-service manager settings, no point for it to be referencing itself
                if (_settings.GetType() != typeof(ServiceManagerSettings))
                {
                    if (PackageConfigurationEditorWindow.Window.TryGetTab(out ServiceManagerTab tab))
                    {
                        tab.AddConfiguration(_settings);
                    }
                }

                OnSettingsCreated();

                // Create asset
                AssetDatabaseHelper.CreateAsset(_settings, assetPath);

                // Save asset
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"New settings ({_settings.GetType().Name}) created at path: \"{assetPath}\".");
            }

            _serializedObject = new SerializedObject(_settings);
            OnInitialized();
        }

        /// <summary>
        /// Called when the settings object is created right before it is saved in the AssetDatabase
        /// </summary>
        internal virtual void OnSettingsCreated() { }

        /// <summary>
        /// Called after the settings object has been retrieved sucessfully
        /// </summary>
        internal virtual void OnInitialized() { }

        /// <summary>
        /// Called when drawing the GUI in the <see cref="PackageConfigurationEditorWindow"/>
        /// </summary>
        internal abstract void DrawGUI();
    }
}
