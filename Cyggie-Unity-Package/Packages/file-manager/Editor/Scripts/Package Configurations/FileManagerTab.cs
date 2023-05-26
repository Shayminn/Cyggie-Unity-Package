using Cyggie.FileManager.Editor.Utils.Styles;
using Cyggie.FileManager.Runtime.ServicesNS;
using Cyggie.Main.Editor.Configurations;
using Cyggie.Main.Editor.Utils.Helpers;
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

        // Serialized Properties
        private SerializedProperty _usePersistentDataPath = null;
        private SerializedProperty _localSavePath = null;
        private SerializedProperty _defaultFileExtension = null;
        private SerializedProperty _encrypted = null;
        private SerializedProperty _filesToIgnore = null;

        /// <inheritdoc/>
        internal override System.Type SettingsType => typeof(FileManagerSettings);

        /// <inheritdoc/>
        internal override string ResourcesPath => FileManagerSettings.cResourcesPath;

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            _usePersistentDataPath = _serializedObject.FindProperty(nameof(FileManagerSettings.UsePersistentDataPath));
            _localSavePath = _serializedObject.FindProperty(nameof(FileManagerSettings.LocalSavePath));
            _defaultFileExtension = _serializedObject.FindProperty(nameof(FileManagerSettings.DefaultFileExtension));
            _encrypted = _serializedObject.FindProperty(nameof(FileManagerSettings.Encrypted));
            _filesToIgnore = _serializedObject.FindProperty(nameof(FileManagerSettings.FilesToIgnore));
        }

        /// <inheritdoc/>
        protected override void DrawGUI()
        {
            EditorGUIUtility.labelWidth = 150;
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField(cSaveLocationLabel, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_usePersistentDataPath, GUIContents.cUsePersistentDataPath);

            EditorGUIHelper.DrawAsReadOnly(_usePersistentDataPath.boolValue, gui: () =>
            {
                EditorGUILayout.PropertyField(_localSavePath, GUIContents.cLocalSavePath);
            });
            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(_defaultFileExtension, GUIContents.cDefaultFileExtension);
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField(cOtherLabel, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_encrypted, GUIContents.cEncrypted);
            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(_filesToIgnore, GUIContents.cFilesToIgnore);
        }
    }
}
