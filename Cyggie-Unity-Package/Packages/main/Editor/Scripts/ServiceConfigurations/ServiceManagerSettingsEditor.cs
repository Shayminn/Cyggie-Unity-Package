using Cyggie.Main.Editor.Utils.Constants;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.Utils.Constants;
using Cyggie.Plugins.Editor.Helpers;
using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Utils.Constants;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// Inspector editor for <see cref="ServiceManagerSettings"/>
    /// </summary>
    [CustomEditor(typeof(ServiceManagerSettings))]
    internal class ServiceManagerSettingsEditor : UnityEditor.Editor
    {
        private const string cTestLogMessage = "This is a test log";
        private const string cTestLogTag = "Tag";

        private SerializedProperty _serviceManagerPrefab = null;
        private SerializedProperty _emptyPrefab = null;
        private SerializedProperty _serviceIdentifiers = null;
        private SerializedProperty _serviceConfigurations = null;
        private SerializedProperty _enableLog = null;

        private ServiceManagerSettings _settings = null;
        private SerializedObject _serializedLogProfile = null;
        private LogProfile _logProfile = null;

        private static RuntimePlatform _selectedPlatform;
        private static bool _deletingProfile = false;

        private void OnEnable()
        {
            _settings = target as ServiceManagerSettings;
            _serviceManagerPrefab = serializedObject.FindProperty(nameof(ServiceManagerSettings.Prefab));
            _emptyPrefab = serializedObject.FindProperty(nameof(ServiceManagerSettings.EmptyPrefab));

            _serviceConfigurations = serializedObject.FindProperty(nameof(ServiceManagerSettings.ServiceConfigurations));

            _enableLog = serializedObject.FindProperty(nameof(ServiceManagerSettings.EnableLog));
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            if (_settings == null) return;
            serializedObject.Update();

            EditorGUIHelper.DrawScriptReference(_settings);
            EditorGUILayout.Space(2);

            GUIHelper.DrawAsReadOnly(gui: () =>
            {
                EditorGUILayout.PropertyField(_serviceManagerPrefab);
                EditorGUILayout.PropertyField(_emptyPrefab);
                EditorGUILayout.Space(5);
            });

            _serviceIdentifiers = serializedObject.FindProperty(nameof(ServiceManagerSettings.ServiceIdentifiers));
            EditorGUILayout.PropertyField(_serviceIdentifiers);
            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(_serviceConfigurations);
            EditorGUILayout.Space(5);

            DrawLogProfiles();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawLogProfiles()
        {
            EditorGUILayout.LabelField("Logs", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_enableLog);

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

            _logProfile = _settings.LogProfiles.FirstOrDefault(x => x.Platform == _selectedPlatform);
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

                    _settings.LogProfiles.RemoveAll(x => x == null);
                    if (!_settings.LogProfiles.Any(x => x.Platform == _selectedPlatform)) // Just in case
                    {
                        _settings.LogProfiles.Add(_logProfile);

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
                EditorGUIHelper.DrawWithConfirm(ref _deletingProfile,
                    confirmLabel: $"Delete profile \"{_logProfile.Name}\"?",
                    onConfirm: () =>
                    {
                        AssetDatabaseHelper.DeleteAsset(_logProfile);
                        _settings.LogProfiles.Remove(_logProfile);
                        _logProfile = null;
                    },
                    onInactiveGUI: () =>
                    {
                        if (GUILayout.Button("Delete", GUILayout.Width(100)))
                        {
                            _deletingProfile = true;
                        }
                    }
                );

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
    }
}
