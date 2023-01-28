using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Cyggie.Main.Editor.Configurations;

namespace Cyggie.Main.Editor
{
    /// <summary>
    /// Static class for Editor Menu Items in toolbar for Cyggie
    /// </summary>
    internal static class EditorMenuItems
    {
        /// <summary>
        /// Menu Item for managing Package Configurations
        /// </summary>
        [MenuItem(itemName: "Cyggie/Package Configurations")]
        private static void PackageConfiguration()
        {
            // Get all PackageConfigurationTab in project
            List<Type> tabTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(PackageConfigurationTab)) && !t.IsAbstract).ToList();
            List<PackageConfigurationTab> tabs = tabTypes.Select(type => (PackageConfigurationTab) Activator.CreateInstance(type)).ToList();

            if (tabs == null || tabs.Count == 0)
            {
                Debug.Log($"Failed to open Package Configuration window: No tab of type {typeof(PackageConfigurationTab)} was found.");
                return;
            }

            // Create window
            PackageConfigurationEditorWindow window = EditorWindow.GetWindow<PackageConfigurationEditorWindow>();
            window.titleContent = new GUIContent("Cyggie's Configurations");
            window.minSize = new Vector2(510, 600); // set window size
            window.Initialize(tabs); // initialize window

            window.Show();
        }
    }
}
