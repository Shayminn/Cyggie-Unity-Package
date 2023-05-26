using Cyggie.Main.Runtime.ServicesNS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Cyggie.Main.Runtime.Utils.Extensions;

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
        internal const string cResourcesPath = ConfigurationSettings.cResourcesFolderPath + nameof(ServiceManagerSettings);

        // Error strings
        private const string cDuplicateConfiguration = "Multiple of the same configuration has been assigned to " + nameof(ServiceConfigurations);

        [SerializeField]
        internal ServiceManager Prefab = null;

        [SerializeField]
        internal List<ServiceConfiguration> ServiceConfigurations = new List<ServiceConfiguration>();

        /// <summary>
        /// Whether the settings are valid
        /// </summary>
        [SerializeField]
        internal bool IsValid = true;

        /// <summary>
        /// The Service Manager settings don't have a ServiceType to reference to
        /// </summary>
        public override Type ServiceType => null;

#if UNITY_EDITOR

        private const string cPrefabPath = "Packages/cyggie.main/Runtime/Prefabs/Service Manager.prefab";

        /// <inheritdoc/>
        internal override void Initialize(ConfigurationSettings configSettings)
        {
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
                return false;
            }

            // Verify that there are no null configurations
            if (uniqueConfigs.Any(x => x == null))
            {
                error = $"The service configuration at index {uniqueConfigs.IndexOf(uniqueConfigs.First(x => x == null))} is null.";
                IsValid = false;
                return false;
            }

            ServiceConfiguration config = uniqueConfigs.FirstOrDefault(x => !x.Validate());
            if (config != null)
            {
                error = $"The service configuration {config.GetType()}'s is invalid. Make sure the {nameof(ServiceConfiguration.ServiceType)} derives from {typeof(Service)}.";
                IsValid = false;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Override the on validate to validate itself within <see cref="Validate"/> <br/>
        /// No need to validate using the ServiceType because this settings doesn't have a service associated to it
        /// </summary>
        protected override void OnValidate()
        {
            if (!Validate(out string error))
            {
                Debug.LogError($"{error} Fix this at Cyggie/Package Configurations");
            }
        }
#endif
    }
}
