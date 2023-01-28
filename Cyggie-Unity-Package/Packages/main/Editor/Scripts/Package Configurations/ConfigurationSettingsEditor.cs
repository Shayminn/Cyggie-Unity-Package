using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using UnityEditor;

namespace Cyggie.Main.Editor.Configurations
{
    [CustomEditor(typeof(ConfigurationSettings))]
    internal class ConfigurationSettingsEditor : UnityEditor.Editor
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
