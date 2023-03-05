using Cyggie.FileManager.Runtime.Services;
using Cyggie.Main.Editor.Utils.Helpers;
using UnityEditor;

namespace Cyggie.FileManager.Editor.Configurations
{
    /// <summary>
    /// File Manager Settings inspector editor
    /// </summary>
    [CustomEditor(typeof(FileManagerSettings))]
    internal class FileManagerSettingsEditor : UnityEditor.Editor
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
