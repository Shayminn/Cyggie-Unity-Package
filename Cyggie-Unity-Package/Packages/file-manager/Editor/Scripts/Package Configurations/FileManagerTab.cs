using Cyggie.FileManager.Runtime.Services;
using Cyggie.Main.Editor.Configurations;
using UnityEditor;

namespace Cyggie.SceneChanger.Editor.Configurations
{
    /// <summary>
    /// Package tab for <see cref="FileManagerSettings"/> <br/>
    /// Accessible through Cyggie/Package Configurations
    /// </summary>
    internal class FileManagerTab : PackageConfigurationTab
    {
        // GUI Labels
        private const string cSaveLocationLabel = "Save Location Settings";
        private const string cOtherLabel = "Other Settings";

        /// <inheritdoc/>
        internal override System.Type SettingsType => typeof(FileManagerSettings);

        /// <inheritdoc/>
        internal override string ResourcesPath => FileManagerSettings.cResourcesPath;

        /// <inheritdoc/>
        internal override void DrawGUI()
        {
            EditorGUIUtility.labelWidth = 150;
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField(cSaveLocationLabel, EditorStyles.boldLabel);
            SerializedProperty usePersistent = _serializedObject.FindProperty(nameof(FileManagerSettings.UsePersistentDataPath));
            EditorGUILayout.PropertyField(usePersistent);

            if (!usePersistent.boolValue)
            {
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(FileManagerSettings.LocalSavePath)));
            }
            EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(FileManagerSettings.DefaultFileExtension)));
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField(cOtherLabel, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(FileManagerSettings.Encrypted)));
            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(FileManagerSettings.FilesToIgnore)));

            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
