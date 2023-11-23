using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Main.Runtime.ServicesNS.ScriptableObjects;
using Cyggie.Main.Runtime.Utils.Constants;
using Cyggie.Plugins.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cyggie.Main.Runtime.Configurations
{
    /// <summary>
    /// Settings for <see cref="ServiceManagerMono"/>
    /// </summary>
    [Serializable]
    internal class ServiceManagerSettings : PackageServiceConfiguration
    {
        internal const string cResourcesPath = FolderConstants.cCyggie +
                                               nameof(ServiceManagerSettings);

        // Error strings
        private const string cDuplicateConfiguration = "Multiple of the same configuration has been assigned to " + nameof(ServiceConfigurations);

        [SerializeField, Tooltip("Prefab instantiated at the start which contains this component.")]
        internal ServiceManagerMono Prefab = null;

        [SerializeField, Tooltip("Empty prefab for the ObjectHelper.")]
        internal GameObject EmptyPrefab = null;

        [SerializeField, Tooltip("Whether logs using Cyggie's Logging system should be enabled.")]
        internal bool EnableLog = true;

        [SerializeField, Tooltip("")]
        internal List<ServiceIdentifier> ServiceIdentifiers = new List<ServiceIdentifier>();

        [SerializeField, Tooltip("List of service configurations. Automatically generated upon refresh.")]
        internal List<ServiceConfigurationSO> ServiceConfigurations = new List<ServiceConfigurationSO>();

        [SerializeField, Tooltip("List of log profiles. Created in the Configuration window.")]
        internal List<LogProfile> LogProfiles = new List<LogProfile>();

        /// <summary>
        /// Try get a service configuration from the list of assigned service configurations
        /// </summary>
        /// <param name="serviceConfiguration">Output service configuration (null if not found)</param>
        /// <returns>Found?</returns>
        internal bool TryGetServiceConfiguration<T>(out T serviceConfiguration) where T : ServiceConfigurationSO
        {
            serviceConfiguration = (T) ServiceConfigurations.FirstOrDefault(x => x.GetType() == typeof(T));
            return serviceConfiguration != null;
        }
    }
}
