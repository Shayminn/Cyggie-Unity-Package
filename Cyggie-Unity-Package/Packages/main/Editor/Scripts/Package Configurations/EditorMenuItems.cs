using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Cyggie.Main.Editor.Configurations;
using UnityEngine.Tilemaps;
using Cyggie.Main.Runtime.Utils.Extensions;

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
            List<PackageConfigurationTab> tabs = tabTypes.Select(type => (PackageConfigurationTab) Activator.CreateInstance(type)).OrderBy(tab => tab.GetType().Name).ToList();

            if (tabs == null || tabs.Count == 0)
            {
                Debug.Log($"Failed to open Package Configuration window: No tab of type {typeof(PackageConfigurationTab)} was found.");
                return;
            }

            PackageConfigurationTab serviceManagerTab = tabs.FirstOrDefault(x => x.GetType() == typeof(ServiceManagerTab));
            tabs = tabs.MoveFirst(serviceManagerTab).ToList();

            // Create window
            PackageConfigurationEditorWindow window = EditorWindow.GetWindow<PackageConfigurationEditorWindow>();
            window.titleContent = new GUIContent("Cyggie's Configurations");
            window.minSize = new Vector2(715, 600); // set window size
            window.Initialize(tabs); // initialize window

            window.Show();
        }
    }
}
