using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Plugins.Editor.Helpers;
using UnityEditor;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// Inspector editor for <see cref="ServiceManagerSettings"/>
    /// </summary>
    [CustomEditor(typeof(ServiceManagerSettings))]
    internal class ServiceManagerSettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty _serviceManagerPrefab = null;
        private SerializedProperty _emptyPrefab = null;
        private SerializedProperty _serviceIdentifiers = null;
        private SerializedProperty _serviceConfigurations = null;
        private SerializedProperty _enabledLogs = null;

        private ServiceManagerSettings _settings = null;

        private void OnEnable()
        {
            _settings = target as ServiceManagerSettings;
            _serviceManagerPrefab = serializedObject.FindProperty(nameof(ServiceManagerSettings.Prefab));
            _emptyPrefab = serializedObject.FindProperty(nameof(ServiceManagerSettings.EmptyPrefab));

            _serviceConfigurations = serializedObject.FindProperty(nameof(ServiceManagerSettings.ServiceConfigurations));
            _enabledLogs = serializedObject.FindProperty(nameof(ServiceManagerSettings.EnabledLogs));
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

            DrawLogSettings();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawLogSettings()
        {
            EditorGUILayout.LabelField("Logs", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_enabledLogs);
        }
    }
}
