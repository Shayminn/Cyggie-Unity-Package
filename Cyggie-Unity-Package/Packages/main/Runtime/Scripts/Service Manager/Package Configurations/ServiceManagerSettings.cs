using Cyggie.Main.Runtime.ServicesNS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cyggie.Main.Runtime.Utils.Constants;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cyggie.Main.Runtime.Configurations
{
    /// <summary>
    /// Settings for <see cref="ServiceManager"/>
    /// </summary>
    internal class ServiceManagerSettings : PackageConfigurationSettings<Service>
    {
        internal const string cResourcesPath = FolderConstants.cCyggie +
                                               nameof(ServiceManagerSettings);

        // Error strings
        private const string cDuplicateConfiguration = "Multiple of the same configuration has been assigned to " + nameof(ServiceConfigurations);

        [SerializeField]
        internal ServiceManager Prefab = null;

        [SerializeField]
        internal List<ServiceConfigurationSO> ServiceConfigurations = new List<ServiceConfigurationSO>();

        /// <summary>
        /// Whether the settings are valid
        /// </summary>
        [SerializeField]
        internal bool IsValid = true;

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

#if UNITY_EDITOR

        private const string cPrefabPath = "Packages/cyggie.main/Runtime/Prefabs/Service Manager.prefab";

        /// <inheritdoc/>
        internal override void OnScriptableObjectCreated()
        {
            base.OnScriptableObjectCreated();

            Prefab = AssetDatabase.LoadAssetAtPath<ServiceManager>(cPrefabPath);
        }

#endif
    }
}
