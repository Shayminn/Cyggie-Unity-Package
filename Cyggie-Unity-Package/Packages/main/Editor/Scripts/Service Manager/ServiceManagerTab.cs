using Cyggie.Main.Editor.Utils.Constants;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Main.Runtime.Utils.Constants;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// Tab for <see cref="ServiceManager"/>
    /// </summary>
    internal class ServiceManagerTab : PackageConfigurationTab<Service, ServiceManagerSettings>
    {
        internal const string cSettingsAssetPath = FolderConstants.cAssets +
                                                   FolderConstants.cCyggieResources +
                                                   FolderConstants.cCyggie +
                                                   nameof(ServiceManagerSettings) + FileExtensionConstants.cAsset;

        // Serialized Properties
        private SerializedProperty _prefab = null;
        private SerializedProperty _serviceConfigurations = null;

        /// <inheritdoc/>
        internal ServiceManagerSettings Settings => (ServiceManagerSettings) _settings;

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            _settings = GetServiceManagerSettings();
            _serializedObject = new SerializedObject(_settings);

            _prefab = _serializedObject.FindProperty(nameof(ServiceManagerSettings.Prefab));
            _serviceConfigurations = _serializedObject.FindProperty(nameof(ServiceManagerSettings.ServiceConfigurations));
        }

        /// <inheritdoc/>
        protected override void DrawGUI()
        {
            // Draw settings properties
            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(_prefab);
            });
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField($"Service Configurations ({_serviceConfigurations.arraySize})", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 0;

                for (int i = 0; i < _serviceConfigurations.arraySize; i++)
                {
                    SerializedProperty _serviceConfigProperty = _serviceConfigurations.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(_serviceConfigProperty, new GUIContent(""));
                    EditorGUILayout.Space(1);
                }
                EditorGUIUtility.labelWidth = labelWidth;
            });

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Refresh", GUILayout.Width(100)))
            {
                PackageConfigurationEditorWindow.RefreshServiceConfigurations(Settings);

                // This is necessary the missing references in the window
                EditorUtility.SetDirty(Settings);

                _serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Add configuration through script from settings
        /// </summary>
        /// <param name="config">Configuration to add</param>
        internal void AddConfiguration(ServiceConfigurationSO config)
        {
            Settings.ServiceConfigurations.Add(config);

            // This will update the current window view
            _serializedObject = new SerializedObject(Settings);

            // This will make sure the changes are saved in the scriptable object
            EditorUtility.SetDirty(Settings);

            _serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Get the service manager settings <br/>
        /// Creating one if not found
        /// </summary>
        /// <returns>The service manager settings</returns>
        public static ServiceManagerSettings GetServiceManagerSettings()
        {
            ServiceManagerSettings settings = Resources.Load<ServiceManagerSettings>(ServiceManagerSettings.cResourcesPath);

            // Create settings object if not found
            if (settings == null)
            {
                Debug.Log($"[Cyggie.Main] {nameof(ServiceManagerSettings)} not found. Creating it...");
                settings = CreateServiceManagerSettings();
            }

            return settings;
        }

        private static ServiceManagerSettings CreateServiceManagerSettings()
        {
            ServiceManagerSettings settings = (ServiceManagerSettings) ScriptableObject.CreateInstance(typeof(ServiceManagerSettings));

            // Create asset
            if (!AssetDatabaseHelper.CreateAsset(settings, cSettingsAssetPath)) return null;

            settings.OnScriptableObjectCreated();
            return settings;
        }
    }
}
