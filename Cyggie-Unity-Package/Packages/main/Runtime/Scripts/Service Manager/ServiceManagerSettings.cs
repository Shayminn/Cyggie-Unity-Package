using UnityEngine;
using Cyggie.Main.Runtime.Utils.Extensions;
using System.Linq;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cyggie.Main.Runtime.Services
{
    /// <summary>
    /// Service manager settings
    /// </summary>
    internal class ServiceManagerSettings : ScriptableObject
    {
        private static readonly string cSettingsAssetPath = "Packages/cyggie.main/Runtime/Resources/ServiceManagerSettings.asset";
        private static readonly string cPrefabPath = "Packages/cyggie.main/Runtime/Prefabs/Service Manager.prefab";

        // Error strings
        private static readonly string cDuplicateConfiguration = $"Multiple of the same configuration has been assigned to {nameof(ServiceConfigurations)}";
        private static readonly string cHasNullConfiguration = $"A null configuration was found in {nameof(ServiceConfigurations)}";

        [SerializeField, Tooltip("Prefab object to instantiate on start.")]
        internal ServiceManager Prefab = null;

        [SerializeField, Tooltip("List of service configurations to apply.")]
        internal ServiceConfiguration[] ServiceConfigurations = { };

        /// <summary>
        /// Whether the settings are valid
        /// </summary>
        [SerializeField, HideInInspector]
        internal bool IsValid = true;

#if UNITY_EDITOR
        private static SerializedObject _serializedSettings = null;
        internal static SerializedObject SerializedSettings => _serializedSettings ??= new SerializedObject(Settings);

        private static ServiceManagerSettings _settings = null;
        internal static ServiceManagerSettings Settings => _settings.AssignIfNull(GetOrCreateSettings());

        /// <summary>
        /// Get or create if not found new default settings
        /// </summary>
        /// <returns>SceneChangerSettings object</returns>
        public static ServiceManagerSettings GetOrCreateSettings()
        {
            // Try get the relative path to file
            ServiceManagerSettings settings = AssetDatabase.LoadAssetAtPath<ServiceManagerSettings>(cSettingsAssetPath);

            if (settings == null)
            {
                // Settings not found, create a new one
                Debug.Log($"Couldn't find default settings file, creating a new one...");

                settings = CreateInstance<ServiceManagerSettings>();
                settings.Prefab = AssetDatabase.LoadAssetAtPath<ServiceManager>(cPrefabPath);
                AssetDatabase.CreateAsset(settings, cSettingsAssetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"New settings ({nameof(ServiceManagerSettings)}) created at path: \"{cSettingsAssetPath}\".");
            }

            return settings;
        }

        /// <summary>
        /// Keywords for search in Project Settings
        /// </summary>
        /// <returns></returns>
        internal static string[] GetKeywords() => new string[]
        {
            nameof(ServiceConfigurations)
        };

        /// <summary>
        /// Validates the Service Manager
        /// </summary>
        /// <param name="error">Outputs error message (if any)</param>
        /// <returns>Valid?</returns>
        internal bool Validate(out string error)
        {
            IsValid = true;
            error = "";

            int oldCount = ServiceConfigurations.Length;
            IEnumerable<ServiceConfiguration> uniqueConfigs = ServiceConfigurations.Distinct();

            if (oldCount != uniqueConfigs.Count())
            {
                error = cDuplicateConfiguration;
                IsValid = false;
            }

            if (uniqueConfigs.Any(x => x == null))
            {
                error = cHasNullConfiguration;
                IsValid = false;
            }

            return IsValid;
        }
#endif
    }
}
