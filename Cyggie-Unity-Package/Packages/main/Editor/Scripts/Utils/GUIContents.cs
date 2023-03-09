using UnityEngine;

namespace Cyggie.Main.Editor.Utils.Styles
{
    /// <summary>
    /// Styles for all EditorGUI drawings
    /// </summary>
    internal class GUIContents
    {
        //
        //  Configuration Settings
        //
        internal static readonly GUIContent cResourcesPath = new GUIContent("Resources Path", "Folder Path for all files related to Cyggie's Package Configurations");

        // 
        //  Service Manager Tab
        //
        internal static readonly GUIContent cPrefab = new GUIContent("Prefab", "Prefab object to instantiate on start.");
        internal static readonly GUIContent cServiceConfigurations = new GUIContent("Service Configurations", "List of service configurations to apply.");
    }
}
