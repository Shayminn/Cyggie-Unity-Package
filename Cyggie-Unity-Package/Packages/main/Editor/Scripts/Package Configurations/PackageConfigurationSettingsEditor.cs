using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using UnityEditor;

namespace Cyggie.Main.Editor
{
    /// <summary>
    /// Inspector Editor for <see cref="PackageConfigurationSettings"/>
    /// </summary>
    public class PackageConfigurationSettingsEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Override the default inspector GUI to make it readonly
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Edit these values in the Toolbar at \"Cyggie/Package Configurations\".");
            EditorGUILayout.Space(10);

            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                base.OnInspectorGUI();
            });
        }
    }
}
