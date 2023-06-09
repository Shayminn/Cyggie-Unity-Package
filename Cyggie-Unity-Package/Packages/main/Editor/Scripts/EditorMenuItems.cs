using Cyggie.Main.Editor.Configurations;
using Cyggie.Main.Runtime;
using Cyggie.Main.Runtime.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor
{
    /// <summary>
    /// Static class for Editor Menu Items in toolbar for Cyggie
    /// </summary>
    internal static class EditorMenuItems
    {
        internal static bool ExpectPackageConfigurationsShortcut = false;

        /// <summary>
        /// Menu Item for managing Package Configurations
        /// </summary>
        [MenuItem(itemName: "Cyggie/Package Configurations &c")]
        private static void PackageConfiguration()
        {
            if (ExpectPackageConfigurationsShortcut)
            {
                ExpectPackageConfigurationsShortcut = false;
                return;
            }

            // Get all PackageConfigurationTab in project
            List<Type> tabTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(AbstractPackageConfigurationTab).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();

            List<AbstractPackageConfigurationTab> tabs = tabTypes.Select(type => (AbstractPackageConfigurationTab) Activator.CreateInstance(type)).OrderBy(tab => tab.GetType().Name).ToList();

            if (tabs == null || tabs.Count == 0)
            {
                Log.Debug($"Failed to open Package Configuration window: No tab of type {typeof(AbstractPackageConfigurationTab)} was found.", nameof(EditorMenuItems));
                return;
            }

            AbstractPackageConfigurationTab serviceManagerTab = tabs.FirstOrDefault(x => x.GetType() == typeof(ServiceManagerTab));
            tabs = tabs.MoveFirst(serviceManagerTab).ToList();

            // Create window
            PackageConfigurationEditorWindow window = EditorWindow.GetWindow<PackageConfigurationEditorWindow>();
            window.titleContent = new GUIContent("Cyggie's Configurations");
            window.minSize = new Vector2(715, 600); // set window size

            window.ServiceManagerTab = serviceManagerTab as ServiceManagerTab;
            window.Initialize(tabs); // initialize window

            window.Show();
        }
    }
}
