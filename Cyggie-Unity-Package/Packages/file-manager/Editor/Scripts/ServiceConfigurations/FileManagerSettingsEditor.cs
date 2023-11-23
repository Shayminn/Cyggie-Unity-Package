using Cyggie.FileManager.Editor.Utils.Styles;
using Cyggie.FileManager.Runtime.ServicesNS;
using Cyggie.Plugins.Editor.Helpers;
using UnityEditor;

namespace Cyggie.FileManager.Editor.Configurations
{
    /// <summary>
    /// Inspector editor for <see cref="FileManagerServiceConfiguration"/>
    /// </summary>
    [CustomEditor(typeof(FileManagerServiceConfiguration))]
    internal class FileManagerSettingsEditor : UnityEditor.Editor
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

        private void OnEnable()
        {
            _usePersistentDataPath = serializedObject.FindProperty(nameof(FileManagerServiceConfiguration.UsePersistentDataPath));
            _localSavePath = serializedObject.FindProperty(nameof(FileManagerServiceConfiguration.LocalSavePath));
            _defaultFileExtension = serializedObject.FindProperty(nameof(FileManagerServiceConfiguration.DefaultFileExtension));
            _encrypted = serializedObject.FindProperty(nameof(FileManagerServiceConfiguration.Encrypted));
            _filesToIgnore = serializedObject.FindProperty(nameof(FileManagerServiceConfiguration.FilesToIgnore));
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            EditorGUIUtility.labelWidth = 150;
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField(cSaveLocationLabel, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_usePersistentDataPath, GUIContents.cUsePersistentDataPath);

            GUIHelper.DrawAsReadOnly(_usePersistentDataPath.boolValue, gui: () =>
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
