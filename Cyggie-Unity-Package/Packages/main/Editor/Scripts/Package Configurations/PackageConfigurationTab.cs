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
        // TODO
        // Put this as a setting for users (this will have its own scriptable object settings but as an editor file cause it's not needed)
        // Folder path of all configurations
        private const string cConfigurationsFolderPath = "Assets/Resources/Cyggie/Package Configurations/";

        protected PackageConfigurationSettings _settings = null;
        protected SerializedObject _serializedObject = null;

        internal string ClassName => GetType().Name.Replace("Tab", "").SplitCamelCase();

        /// <summary>
        /// Type of settings that inherits <see cref="PackageConfigurationSettings"/>
        /// </summary>
        internal abstract Type SettingsType { get; }

        /// <summary>
        /// Constructor for retrieving/creating this tab's settings based on <see cref="SettingType"/>
        /// </summary>
        protected PackageConfigurationTab()
        {
            // Get settings
            string assetPath = $"{cConfigurationsFolderPath}{SettingsType.Name}.asset";
            _settings = (PackageConfigurationSettings) AssetDatabase.LoadAssetAtPath(assetPath, SettingsType);

            if (_settings == null)
            {
                // Settings not found, create a new one
                Debug.Log($"Couldn't find settings file for tab: {GetType().Name}, creating a new one...");

                // Create object instance
                _settings = (PackageConfigurationSettings) ScriptableObject.CreateInstance(SettingsType);
                OnSettingsCreated();

                // Create asset
                Directory.CreateDirectory(assetPath);
                AssetDatabase.CreateAsset(_settings, assetPath);

                // Save asset
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"New settings ({nameof(_settings.name)}) created at path: \"{assetPath}\".");
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
