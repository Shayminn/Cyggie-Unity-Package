using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.Services;
using System;
using UnityEditor;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// Tab for <see cref="ServiceManager"/>
    /// </summary>
    internal class ServiceManagerTab : PackageConfigurationTab
    {
        private string _logMessage = "";

        /// <inheritdoc/>
        private ServiceManagerSettings Settings => (ServiceManagerSettings)_settings;

        /// <inheritdoc/>
        internal override Type SettingsType => typeof(ServiceManagerSettings);

        /// <inheritdoc/>
        internal override void DrawGUI()
        {
            // Draw settings properties
            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(ServiceManagerSettings.Prefab)));
            });
            EditorGUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(ServiceManagerSettings.ServiceConfigurations)));
            if (EditorGUI.EndChangeCheck())
            {
                // Reset the log message if the list of configurations has changed
                _logMessage = "";

                // Updates the settings object
                _serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space(10);

            // Validate settings
            bool isError = false;
            if (!Settings.Validate(out string error))
            {
                isError = true;
                _logMessage = error;
            }

            // Draw log message
            if (!string.IsNullOrEmpty(_logMessage))
            {
                EditorGUILayout.HelpBox(_logMessage, isError ? MessageType.Error : MessageType.Info);
            }

            EditorGUILayout.Space(10);
            _serializedObject.ApplyModifiedProperties();
        }

        internal void AddConfiguration(ServiceConfiguration config)
        {
            Settings.ServiceConfigurations.Add(config);

            // This will update the current window view
            _serializedObject = new SerializedObject(Settings);

            // This will make sure the changes are saved in the scriptable object
            EditorUtility.SetDirty(Settings);

            _serializedObject.ApplyModifiedProperties();
        }
    }
}
