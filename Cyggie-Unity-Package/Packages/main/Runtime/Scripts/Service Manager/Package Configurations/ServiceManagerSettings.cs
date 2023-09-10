using Cyggie.Main.Runtime.ServicesNS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cyggie.Main.Runtime.Utils.Constants;
using Cyggie.Plugins.Logs;

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

        [SerializeField, Tooltip("Prefab instantiated at the start which contains this component.")]
        internal ServiceManager Prefab = null;

        [SerializeField, Tooltip("Empty prefab for the ObjectHelper.")]
        internal GameObject EmptyPrefab = null;

        [SerializeField, Tooltip("Determines which services part of the Main package should be enabled.")]
        internal MainServiceTypes EnabledServices = MainServiceTypes.Everything;

        [SerializeField, Tooltip("Whether logs using Cyggie's Logging system should be enabled.")]
        internal bool EnableLog = true;

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

#if UNITY_EDITOR

        private const string cPrefabPath = "Packages/cyggie.main/Runtime/Prefabs/Service Manager.prefab";
        private const string cEmptyPrefabPath = "Packages/cyggie.main/Runtime/Prefabs/GameObject.prefab";

        /// <inheritdoc/>
        internal override void OnScriptableObjectCreated()
        {
            base.OnScriptableObjectCreated();

            Prefab = AssetDatabase.LoadAssetAtPath<ServiceManager>(cPrefabPath);
            EmptyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(cEmptyPrefabPath);

            // This makes sure that the above reference is saved after closing the editor
            EditorUtility.SetDirty(this);
        }

        internal void Refresh()
        {
            if (Prefab == null)
            {
                Prefab = AssetDatabase.LoadAssetAtPath<ServiceManager>(cPrefabPath);
            }

            if (EmptyPrefab == null)
            {
                EmptyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(cEmptyPrefabPath);
            }

            // Remove null/missing configurations
            ServiceConfigurations.RemoveAll(config => config == null);

            // Remove null/missing profiles
            LogProfiles.RemoveAll(profile => profile == null);
        }

#endif
    }
}
