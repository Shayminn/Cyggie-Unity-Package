using Cyggie.Main.Runtime.Services;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cyggie.Main.Runtime.Configurations
{
    /// <summary>
    /// Settings for <see cref="ServiceManager"/>
    /// </summary>
    internal class ServiceManagerSettings : PackageConfigurationSettings
    {
        // Error strings
        private const string cDuplicateConfiguration = "Multiple of the same configuration has been assigned to " + nameof(ServiceConfigurations);
        private const string cHasNullConfiguration = "A null configuration was found in " + nameof(ServiceConfigurations);

        [SerializeField, Tooltip("Prefab object to instantiate on start.")]
        internal ServiceManager Prefab = null;

        [SerializeField, Tooltip("List of service configurations to apply.")]
        internal List<ServiceConfiguration> ServiceConfigurations = new List<ServiceConfiguration>();

        [SerializeField, HideInInspector]
        internal ConfigurationSettings ConfigurationSettings = null;

        /// <summary>
        /// Whether the settings are valid
        /// </summary>
        [SerializeField, HideInInspector]
        internal bool IsValid = true;

        /// <summary>
        /// The Service Manager settings don't have a ServiceType to reference to
        /// </summary>
        public override Type ServiceType => null;

#if UNITY_EDITOR

        private const string cPrefabPath = "Packages/cyggie.main/Runtime/Prefabs/Service Manager.prefab";

        internal override void Initialize(ConfigurationSettings configSettings)
        {
            ConfigurationSettings = configSettings;
            Prefab = AssetDatabase.LoadAssetAtPath<ServiceManager>(cPrefabPath);
        }

        /// <summary>
        /// Validates the Service Manager
        /// </summary>
        /// <param name="error">Outputs error message (if any)</param>
        /// <returns>Valid?</returns>
        internal bool Validate(out string error)
        {
            IsValid = true;
            error = "";

            int oldCount = ServiceConfigurations.Count;
            IEnumerable<ServiceConfiguration> uniqueConfigs = ServiceConfigurations.Distinct();

            // Verify that there are no dupes
            if (oldCount != uniqueConfigs.Count())
            {
                error = cDuplicateConfiguration;
                IsValid = false;
            }

            // Verify that there are no null configurations
            if (uniqueConfigs.Any(x => x == null))
            {
                error = cHasNullConfiguration;
                IsValid = false;
            }

            return IsValid;
        }
    }
#endif
}
