using Cyggie.Main.Editor.Utils.Constants;
using Cyggie.Main.Editor.Windows;
using UnityEditor;

namespace Cyggie.Main.Editor
{
    /// <summary>
    /// Static class for Editor Menu Items in toolbar for Cyggie
    /// </summary>
    internal static class EditorMenuItems
    {
        internal static bool ExpectPackageConfigurationsShortcut = false;

        /// <summary>
        /// Menu Item for managing Service Configurations
        /// </summary>
        [MenuItem(itemName: EditorMenuItemConstants.cServiceConfigurations + " &#c")]
        private static void OpenPackageConfigurations()
        {
            if (ExpectPackageConfigurationsShortcut)
            {
                ExpectPackageConfigurationsShortcut = false;
                return;
            }

            // Create window
            ServiceConfigurationsEditorWindow window = EditorWindow.GetWindow<ServiceConfigurationsEditorWindow>();
            window.titleContent = EditorWindowConstants.cServiceConfigurationWindowTitle;
            window.minSize = EditorWindowConstants.cServiceConfigurationWindowMinSize;

            window.Show();
        }

        /// <summary>
        /// Menu item for opening a window for creating service identifiers scriptable objects
        /// </summary>
        [MenuItem(itemName: EditorMenuItemConstants.cCreateServiceIdentifier)]
        private static void CreateServiceIdentifier()
        {
            // Create window
            ServiceCreatorEditorWindow window = EditorWindow.GetWindow<ServiceCreatorEditorWindow>();
            window.titleContent = EditorWindowConstants.cServiceCreatorWindowTitle;
            window.minSize = EditorWindowConstants.cServiceCreatorWindowMinSize;
            window.maxSize = EditorWindowConstants.cServiceCreatorWindowMaxSize;

            window.Show();
        }

        /// <summary>
        /// Menu item for opening a window for creating service configurations scriptable objects
        /// </summary>
        [MenuItem(itemName: EditorMenuItemConstants.cCreateServiceConfiguration)]
        private static void CreateServiceConfiguration()
        {
            // Create window
            ServiceConfigurationCreatorEditorWindow window = EditorWindow.GetWindow<ServiceConfigurationCreatorEditorWindow>();
            window.titleContent = EditorWindowConstants.cServiceConfigurationCreatorWindowTitle;
            window.minSize = EditorWindowConstants.cServiceConfigurationCreatorWindowMinSize;
            window.maxSize = EditorWindowConstants.cServiceConfigurationCreatorWindowMaxSize;

            window.Show();
        }

        /// <summary>
        /// Menu item for opening a window for managing configurations
        /// </summary>
        [MenuItem(itemName: EditorMenuItemConstants.cEditorConfigurations)]
        private static void OpenEditorConfigurations()
        {
            // Create window
            EditorConfigurationsEditorWindow window = EditorWindow.GetWindow<EditorConfigurationsEditorWindow>();
            window.titleContent = EditorWindowConstants.cEditorConfigurationsWindowTitle;
            window.minSize = EditorWindowConstants.cEditorConfigurationsWindowMinSize;

            window.Show();
        }
    }
}
