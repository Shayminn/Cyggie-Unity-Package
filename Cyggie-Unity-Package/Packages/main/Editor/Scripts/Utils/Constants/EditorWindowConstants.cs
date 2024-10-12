using UnityEngine;

namespace Cyggie.Main.Editor.Utils.Constants
{
    /// <summary>
    /// Constants of Editor Prefs keys
    /// </summary>
    internal struct EditorWindowConstants
    {
        //
        // Editor configurations Editor window
        //
        internal static readonly GUIContent cEditorConfigurationsWindowTitle = new GUIContent("Editor Configurations");
        internal static readonly Vector2 cEditorConfigurationsWindowMinSize = new Vector2(500, 250);

        // 
        // Service Configurations Editor Window
        //
        internal static readonly GUIContent cServiceConfigurationWindowTitle = new GUIContent("Service Configurations");
        internal static readonly Vector2 cServiceConfigurationWindowMinSize = new Vector2(715, 600);

        // 
        // Service Creator Editor Window
        //
        internal static readonly GUIContent cServiceCreatorWindowTitle = new GUIContent("Service Creator");
        internal static readonly Vector2 cServiceCreatorWindowMinSize = new Vector2(600, 500);
        internal static readonly Vector2 cServiceCreatorWindowMaxSize = new Vector2(600, 500);

        // 
        // Service Configuration Creator Editor Window
        //
        internal static readonly GUIContent cServiceConfigurationCreatorWindowTitle = new GUIContent("Service Configuration Creator");
        internal static readonly Vector2 cServiceConfigurationCreatorWindowMinSize = new Vector2(600, 500);
        internal static readonly Vector2 cServiceConfigurationCreatorWindowMaxSize = new Vector2(600, 500);

        internal const string cPackageWindowSelectedTabIndex = "Cyggie/PackageConfiguration/SelectedTabIndex";
        internal const string cServiceManagerSelectedPlatformIndex = "Cyggie/ServiceManager/SelectedPlatformIndex";
    }
}
