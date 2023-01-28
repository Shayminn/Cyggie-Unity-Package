using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Services;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Configurations
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
        internal ServiceConfiguration[] ServiceConfigurations = { };

        /// <summary>
        /// Whether the settings are valid
        /// </summary>
        [SerializeField, HideInInspector]
        internal bool IsValid = true;

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

    /// <summary>
    /// Service Manager Settings inspector editor
    /// </summary>
    [CustomEditor(typeof(ServiceManagerSettings))]
    internal class ServiceManagerSettingsEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Override the default inspector GUI to make it readonly
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                base.OnInspectorGUI();
            });
        }
    }
}
