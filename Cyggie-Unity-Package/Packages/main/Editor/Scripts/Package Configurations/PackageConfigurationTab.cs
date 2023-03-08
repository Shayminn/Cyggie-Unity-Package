using Cyggie.Main.Editor.Utils.Constants;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.Utils.Extensions;
using System;
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
        /// Path to the asset from the resources folder
        /// </summary>
        internal abstract string ResourcesPath { get; }

        /// <summary>
        /// Type of settings that inherits <see cref="PackageConfigurationSettings"/>
        /// </summary>
        internal abstract Type SettingsType { get; }

        /// <summary>
        /// Dropdown name for the tab (used to display in Window selection popup)
        /// </summary>
        internal virtual string DropdownName { get => GetType().Name.Replace("Tab", "").SplitCamelCase(); }

        /// <summary>
        /// Title for the Settings Tab that appears in the Package Configuration Window
        /// </summary>
        internal virtual string Title { get => SettingsType.Name.SplitCamelCase(); }

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
            string assetPath = $"{configSettings.ResourcesPath}{ResourcesPath}".InsertEndsWith(FileExtensionConstants.cAsset);
            _settings = (PackageConfigurationSettings) AssetDatabase.LoadAssetAtPath(assetPath, SettingsType);

            if (_settings == null)
            {
                // Settings not found, create a new one
                Debug.Log($"Couldn't find settings file for tab: {GetType().Name}, creating a new one...");

                _settings = GetOrCreateSettings(SettingsType, ResourcesPath, OnSettingsCreated);

                // Add Package Configuration Settings as a ServiceConfiguration
                // Only for non-service manager settings, no point for it to be referencing itself
                if (_settings.GetType() != typeof(ServiceManagerSettings))
                {
                    if (PackageConfigurationEditorWindow.Window.TryGetTab(out ServiceManagerTab tab))
                    {
                        tab.AddConfiguration(_settings);
                    }
                }

                Debug.Log($"New settings ({_settings.GetType().Name}) created at path: \"{assetPath}\".");
            }

            _serializedObject = new SerializedObject(_settings);
            OnInitialized();
        }

        /// <summary>
        /// Draw the package configuration settings tab
        /// </summary>
        internal void DrawTab()
        {
            if (_serializedObject == null) return;

            DrawGUI();
            _serializedObject.ApplyModifiedProperties();
        }

        #region Overloadable methods

        /// <summary>
        /// Called when the settings object is created right before it is saved in the AssetDatabase
        /// </summary>
        protected virtual void OnSettingsCreated() { }

        /// <summary>
        /// Called after the settings object has been retrieved sucessfully
        /// </summary>
        protected virtual void OnInitialized() { }

        /// <summary>
        /// Called when drawing the GUI in the <see cref="PackageConfigurationEditorWindow"/>
        /// </summary>
        protected abstract void DrawGUI();

        #endregion

        #region Static methods

        /// <summary>
        /// Create the object settings of type <typeparamref name="T"/> <br/>
        /// This is necessary when used in windows independent from the <see cref="PackageConfigurationEditorWindow"/>
        /// </summary>
        /// <typeparam name="T">Configuration Settings type to create</typeparam>
        /// <param name="resourcesPath">Relative path to resources folder</param>
        /// <param name="onSettingsCreated">Callback when new settings are created</param>
        /// <returns>Settings object</returns>
        internal static T GetOrCreateSettings<T>(string resourcesPath, Action onSettingsCreated = null) where T : PackageConfigurationSettings
        {
            return (T) GetOrCreateSettings(typeof(T), resourcesPath, onSettingsCreated);
        }

        /// <summary>
        /// Create the object settings of type <typeparamref name="T"/> <br/>
        /// This is necessary when used in windows independent from the <see cref="PackageConfigurationEditorWindow"/>
        /// </summary>
        /// <param name="type">Configuration Settings type to create</param>
        /// <param name="resourcesPath">Relative path to resources folder</param>
        /// <param name="onSettingsCreated">Callback when settings are created</param>
        /// <returns>Settings object</returns>
        internal static PackageConfigurationSettings GetOrCreateSettings(Type type, string resourcesPath, Action onSettingsCreated = null)
        {
            // Get the configuration settings
            ConfigurationSettings configSettings = Resources.Load<ConfigurationSettings>(ConfigurationSettings.cResourcesPath);

            // Load existing settings
            string assetPath = $"{configSettings.ResourcesPath}{resourcesPath}".InsertEndsWith(FileExtensionConstants.cAsset);
            PackageConfigurationSettings settings = (PackageConfigurationSettings) AssetDatabase.LoadAssetAtPath(assetPath, type);

            // Create settings object if not found
            if (settings == null)
            {
                settings = (PackageConfigurationSettings) ScriptableObject.CreateInstance(type);
                settings.Initialize(configSettings);

                onSettingsCreated?.Invoke();

                // Create asset
                AssetDatabaseHelper.CreateAsset(settings, assetPath);

                // Save asset
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return settings;
        }

        #endregion
    }
}
