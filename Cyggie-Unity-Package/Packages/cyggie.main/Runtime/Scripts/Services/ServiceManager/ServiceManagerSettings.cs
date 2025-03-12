using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Main.Runtime.ServicesNS.ScriptableObjects;
using Cyggie.Main.Runtime.Utils.Constants;
using Cyggie.Plugins.Logs;
using Cyggie.Plugins.UnityServices.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cyggie.Main.Runtime.Configurations
{
    /// <summary>
    /// Settings for <see cref="ServiceManagerMono"/>
    /// </summary>
    [Serializable]
    internal class ServiceManagerSettings : PackageServiceConfiguration
    {
#if UNITY_EDITOR
        internal const string cPrefabPath = "Packages/cyggie.main/Runtime/Prefabs/Service Manager.prefab";
        internal const string cEmptyPrefabPath = "Packages/cyggie.main/Runtime/Prefabs/Empty.prefab";
#endif

        internal const string cResourcesPath = FolderConstants.cCyggie +
                                               nameof(ServiceManagerSettings);

        // Error strings
        private const string cDuplicateConfiguration = "Multiple of the same configuration has been assigned to " + nameof(ServiceConfigurations);

        [SerializeField, Tooltip("Prefab instantiated at the start which contains this component.")]
        internal ServiceManagerMono Prefab = null;

        [SerializeField, Tooltip("Empty prefab for the ObjectHelper.")]
        internal GameObject EmptyPrefab = null;

        [SerializeField, Tooltip("The list of service identifiers to which services are created and initialized.")]
        internal List<ServiceIdentifier> ServiceIdentifiers = new List<ServiceIdentifier>();

        [SerializeField, Tooltip("List of service configurations. Automatically added upon opening the Service Configuration editor window.")]
        internal List<ServiceConfigurationSO> ServiceConfigurations = new List<ServiceConfigurationSO>();

        [SerializeField, Tooltip("The type of logs that are enabled when using the Log system.")]
        internal LogTypes EnabledLogs = LogTypes.Debug | LogTypes.Warning | LogTypes.Error;
    }
}
