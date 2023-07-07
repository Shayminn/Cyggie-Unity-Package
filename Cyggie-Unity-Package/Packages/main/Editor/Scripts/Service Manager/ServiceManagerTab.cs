using Cyggie.Main.Editor.Utils.Constants;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Main.Runtime.Utils.Constants;
using Cyggie.Plugins.Logs;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// Tab for <see cref="ServiceManager"/>
    /// </summary>
    internal class ServiceManagerTab : PackageConfigurationTab<Service, ServiceManagerSettings>
    {
        private const string cTestLogMessage = "This is a test log";
        private const string cTestLogTag = "Tag";
        internal const string cSettingsAssetPath = FolderConstants.cAssets +
                                                   FolderConstants.cCyggieResources +
                                                   FolderConstants.cCyggie +
                                                   nameof(ServiceManagerSettings) + FileExtensionConstants.cAsset;

        // Serialized Properties
        private SerializedProperty _prefab = null;
        private SerializedProperty _enabledServices = null;
        private SerializedProperty _enableLog = null;

        private SerializedProperty _serviceConfigurations = null;
        private SerializedProperty _logProfiles = null;


        private Vector2 _scrollviewPos = Vector2.zero;
        private RuntimePlatform _selectedPlatform;

        private LogProfile _logProfile = null;
        private SerializedObject _serializedLogProfile = null;

        private bool _deletingProfile = false;

        /// <inheritdoc/>
        internal ServiceManagerSettings Settings => (ServiceManagerSettings) _settings;

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            _settings = GetServiceManagerSettings();
            _serializedObject = new SerializedObject(_settings);

            _prefab = _serializedObject.FindProperty(nameof(ServiceManagerSettings.Prefab));
            _enabledServices = _serializedObject.FindProperty(nameof(ServiceManagerSettings.EnabledServices));
            _enableLog = _serializedObject.FindProperty(nameof(ServiceManagerSettings.EnableLog));

            _serviceConfigurations = _serializedObject.FindProperty(nameof(ServiceManagerSettings.ServiceConfigurations));
            _logProfiles = _serializedObject.FindProperty(nameof(ServiceManagerSettings.LogProfiles));

            // Auto select current platform
            _selectedPlatform = (RuntimePlatform) EditorPrefs.GetInt(EditorPrefsConstants.cServiceManagerSelectedPlatformIndex, (int) Application.platform);
        }

        /// <inheritdoc/>
        protected override void DrawGUI()
        {
            DrawServiceManagerSettings();
            EditorGUILayout.Space(10);

            DrawServiceConfigurations();
            EditorGUILayout.Space(10);

            DrawLogProfiles();
            EditorGUILayout.Space(10);
        }

        private void DrawServiceManagerSettings()
        {
            // Draw settings properties
            GUIHelper.DrawAsReadOnly(gui: () =>
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(_prefab);
                EditorGUILayout.Space(5);
            });

            // Draw Service Manager toggles
            EditorGUILayout.PropertyField(_enabledServices);
            EditorGUILayout.PropertyField(_enableLog);
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

        private void DrawServiceConfigurations()
        {
            EditorGUILayout.LabelField($"Service Configurations ({_serviceConfigurations.arraySize})", EditorStyles.boldLabel);

            GUIHelper.DrawAsReadOnly(gui: () =>
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

        private void DrawLogProfiles()
        {
            EditorGUILayout.LabelField("Logs", EditorStyles.boldLabel);

            bool changed = EditorGUIHelper.CheckChange(gui: () =>
            {
                EditorGUIUtility.labelWidth = 75;
                _selectedPlatform = (RuntimePlatform) EditorGUILayout.EnumPopup("Platform:", _selectedPlatform, GUILayout.Width(250));
                EditorGUILayout.Space(5);
            });

            if (changed)
            {
                EditorPrefs.SetInt(EditorPrefsConstants.cServiceManagerSelectedPlatformIndex, (int) _selectedPlatform);
            }

            _logProfile = Settings.LogProfiles.FirstOrDefault(x => x.Platform == _selectedPlatform);
            if (_logProfile == null)
            {
                EditorGUILayout.LabelField($"No log profile found for {_selectedPlatform}.");
                EditorGUILayout.LabelField("By default: Debugs, warnings and errors with timestamp and stack trace will be shown.");
                EditorGUILayout.Space(5);

                if (GUILayout.Button("Create profile", GUILayout.Width(150)))
                {
                    _logProfile = ScriptableObject.CreateInstance<LogProfile>();
                    _logProfile.Platform = _selectedPlatform;
                    _logProfile.Name = _selectedPlatform.ToString();

                    Settings.LogProfiles.RemoveAll(x => x == null);
                    if (!Settings.LogProfiles.Any(x => x.Platform == _selectedPlatform)) // Just in case
                    {
                        Settings.LogProfiles.Add(_logProfile);

                        AssetDatabaseHelper.CreateAsset(_logProfile, FolderConstants.cAssets + FolderConstants.cCyggieScriptableObjects + FolderConstants.cLogProfiles + _selectedPlatform + FileExtensionConstants.cAsset);
                    }
                }
            }
            else
            {
                _serializedLogProfile = new SerializedObject(_logProfile);
                EditorGUILayout.LabelField("Displayed Logs");
                ToggleLogTypesFlag("Debug", LogTypes.Debug);
                ToggleLogTypesFlag("Warning", LogTypes.Warning);
                ToggleLogTypesFlag("Error", LogTypes.Error);

                EditorGUILayout.Space(10);
                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUIUtility.labelWidth = 110;
                    SerializedProperty showTimestamp = _serializedLogProfile.FindProperty(nameof(LogProfile.ShowTimestamp));
                    EditorGUILayout.PropertyField(showTimestamp, GUILayout.Width(150));

                    if (_logProfile.ShowTimestamp)
                    {
                        EditorGUIUtility.labelWidth = 50;
                        SerializedProperty timestampFormat = _serializedLogProfile.FindProperty(nameof(LogProfile.TimestampFormat));
                        EditorGUILayout.PropertyField(timestampFormat, new GUIContent("Format:"), GUILayout.Width(200));

                        GUIHelper.DrawAsReadOnly(gui: () =>
                        {
                            EditorGUILayout.TextField($"[{DateTime.Now.ToString(timestampFormat.stringValue)}]", GUILayout.Width(200));
                        });
                    }
                });

                EditorGUILayout.Space(2);
                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUIUtility.labelWidth = 110;
                    SerializedProperty showStackTrace = _serializedLogProfile.FindProperty(nameof(LogProfile.ShowStackTrace));
                    EditorGUILayout.PropertyField(showStackTrace, GUILayout.Width(150));

                    if (_logProfile.ShowStackTrace)
                    {
                        EditorGUIUtility.labelWidth = 75;
                        SerializedProperty stackTraceScriptOnly = _serializedLogProfile.FindProperty(nameof(LogProfile.StackTraceScriptOnly));
                        EditorGUILayout.PropertyField(stackTraceScriptOnly, new GUIContent("Script only"), GUILayout.Width(150));
                    }
                });

                EditorGUILayout.Space(10);
                if (_logProfile.Types != LogTypes.None)
                {
                    if (GUILayout.Button("Test log", GUILayout.Width(100)))
                    {
                        Log.SetProfile(_logProfile);

                        if (_logProfile.Types.HasFlag(LogTypes.Debug))
                        {
                            Log.Debug(cTestLogMessage, cTestLogTag);
                        }

                        if (_logProfile.Types.HasFlag(LogTypes.Warning))
                        {
                            Log.Warning(cTestLogMessage, cTestLogTag);
                        }

                        if (_logProfile.Types.HasFlag(LogTypes.Error))
                        {
                            Log.Error(cTestLogMessage, cTestLogTag);
                        }

                        Log.SetProfile(null);
                    }
                }

                EditorGUILayout.Space(2);
                if (_deletingProfile)
                {
                    EditorGUIHelper.DrawHorizontal(gui: () =>
                    {
                        GUIHelper.DrawWithBackgroundColor(Color.green, gui: () =>
                        {
                            // Confirm delete
                            if (GUILayout.Button("Confirm", GUILayout.Width(100)))
                            {
                                AssetDatabaseHelper.DeleteAsset(_logProfile);
                                Settings.LogProfiles.Remove(_logProfile);
                                _logProfile = null;

                                _deletingProfile = false;
                                return;
                            }
                        });

                        GUIHelper.DrawWithBackgroundColor(Color.red, gui: () =>
                        {
                            // Cancel delete
                            if (GUILayout.Button("Cancel", GUILayout.Width(100)))
                            {
                                _deletingProfile = false;
                            }
                        });
                    });
                }
                else
                {
                    GUIHelper.DrawWithBackgroundColor(Color.red, gui: () =>
                    {
                        if (GUILayout.Button("Delete", GUILayout.Width(100)))
                        {
                            _deletingProfile = true;
                        }
                    });
                }

                if (_logProfile != null)
                {
                    _serializedLogProfile.ApplyModifiedProperties();
                }
            }
        }

        private void ToggleLogTypesFlag(string label, LogTypes type)
        {
            bool toggle = EditorGUILayout.Toggle(new GUIContent(label, $"Whether logs of type {type} should be displayed"), _logProfile.Types.HasFlag(type));
            if (EditorGUI.EndChangeCheck())
            {
                if (toggle)
                {
                    _logProfile.Types |= type;
                }
                else
                {
                    _logProfile.Types &= ~type;
                }
            }
        }

        #region Static methods

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
                Log.Debug($"{nameof(ServiceManagerSettings)} not found. Creating it...", nameof(ServiceManagerTab));
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

        #endregion
    }
}
