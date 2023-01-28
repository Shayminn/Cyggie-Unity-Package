using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using UnityEditor;

namespace Cyggie.Main.Editor.Configurations
{

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
