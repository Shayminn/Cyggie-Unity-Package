using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.ServicesNS.ScriptableObjects;
using Cyggie.Plugins.Editor.Helpers;
using Cyggie.Plugins.Utils.Constants;
using Cyggie.Plugins.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Windows
{
    /// <summary>
    /// Editor window to create a <see cref="ServiceConfigurationMono"/>
    /// </summary>
    public class ServiceConfigurationCreatorEditorWindow : EditorWindow
    {
        private List<Type> _serviceTypes = null;
        private Type _selectedType = null;
        private int _selectedTypeIndex = -1;
        private string _configFilePath = "";

        private string _searchText = "";
        private Vector2 _scrollbarPosition = Vector2.zero;

        private bool _confirmDelete = false;

        private void OnEnable()
        {
            _serviceTypes = TypeHelper.GetAllIsAssignableFrom<ServiceConfigurationSO>().ToList();
        }

        private void OnGUI()
        {
            // Reset values when it's null
            if (_selectedType == null)
            {
                _selectedTypeIndex = -1;
                _confirmDelete = false;
            }

            EditorGUILayout.Space(5);

            EditorGUIHelper.DrawHorizontal(gui: (Action) (() =>
            {
                EditorGUILayout.Space(2);
                EditorGUIHelper.DrawVertical(gui: (Action) (() =>
                {
                    // Instructions
                    EditorGUILayout.LabelField("Select a service configuration type and create a new identifier that you can assign in");
                    EditorGUILayout.LabelField("the Service Configuration window (ALT + C).");
                    EditorGUILayout.Space(5);

                    // Search bar
                    EditorGUIHelper.DrawHorizontal(gui: () =>
                    {
                        EditorGUILayout.LabelField("Search: ", GUILayout.Width(50));
                        _searchText = EditorGUILayout.TextField(_searchText);
                    });
                    EditorGUILayout.Space(2);

                    // Scroll view list
                    EditorGUIHelper.DrawWithScrollview(
                        ref _scrollbarPosition,
                        gui: () =>
                        {
                            // Filter types by search text
                            IEnumerable<Type> filteredTypes = _serviceTypes.Where(x => x.FullName.Contains(_searchText, StringComparison.InvariantCultureIgnoreCase));

                            if (filteredTypes.Count() == 0)
                            {
                                EditorGUILayout.LabelField("No service configurations found.");
                            }
                            else
                            {
                                if (EditorGUIHelper.CheckChange(gui: () => _selectedTypeIndex = GUILayout.SelectionGrid(_selectedTypeIndex, filteredTypes.Select(x => new GUIContent(x.Name, x.FullName)).ToArray(), 1)))
                                {
                                    _selectedType = filteredTypes.ElementAt(_selectedTypeIndex);

                                    // Reset the delete confirmation
                                    _confirmDelete = false;
                                }
                            }
                        },
                        alwaysShowVertical: true,
                        horizontalStyle: GUIStyle.none,
                        backgroundStyle: EditorStyles.helpBox,
                        guiLayoutOptions: new GUILayoutOption[] { GUILayout.MaxHeight(200) }
                    );
                    EditorGUILayout.Space(5);

                    string selectedText = "";
                    if (_selectedType != null)
                    {
                        string path = $"{Runtime.Utils.Constants.FolderConstants.cAssets}{Runtime.Utils.Constants.FolderConstants.cCyggieServiceConfigurations}";

                        // Make sure directory exists
                        Directory.CreateDirectory(path);

                        string[] files = Directory.GetFiles(path, $"*{FileExtensionConstants.cAsset}");
                        bool configExists = false;
                        foreach (string file in files)
                        {
                            ServiceConfigurationSO config = AssetDatabase.LoadAssetAtPath<ServiceConfigurationSO>(file);
                            if (config == null) continue;
                            if (config.GetType() == _selectedType)
                            {
                                _configFilePath = file;
                                configExists = true;
                                break;
                            }
                        }

                        // Check if it already exists at path
                        if (configExists)
                        {
                            GUIHelper.DrawWithColor(Color.yellow, gui: () =>
                            {
                                EditorGUILayout.LabelField(new GUIContent("Service Configuration already exists! But no worries, I'm not stopping you =D"));
                            });
                        }
                        else
                        {
                            selectedText = _selectedType.FullName;

                            EditorGUIHelper.DrawHorizontal(gui: () =>
                            {
                                EditorGUILayout.LabelField("Selected ", GUILayout.Width(53));
                                GUIHelper.DrawWithColor(Color.green, gui: () =>
                                {
                                    EditorGUILayout.LabelField(new GUIContent(selectedText, selectedText));
                                });
                            });
                        }

                        if (GUILayout.Button("Create", GUILayout.Width(50)))
                        {
                            ServiceConfigurationSO config = (ServiceConfigurationSO) ScriptableObject.CreateInstance(_selectedType);
                            config.name = _selectedType.Name;

                            AssetDatabaseHelper.CreateAsset(config, $"{path}{config.name}{FileExtensionConstants.cAsset}");
                        }

                        if (configExists)
                        {
                            EditorGUIHelper.DrawWithConfirm(ref _confirmDelete,
                                onInactiveGUI: () =>
                                {
                                    if (GUILayout.Button("Delete", GUILayout.Width(50)))
                                    {
                                        _confirmDelete = true;
                                    }
                                },
                                confirmLabel: "Are you sure you want to delete?",
                                onConfirm: (Action) (() =>
                                {
                                    AssetDatabaseHelper.DeleteAsset(_configFilePath);
                                    _confirmDelete = false;
                                })
                            );
                        }
                    }
                }));
                EditorGUILayout.Space(2);
            }));
        }
    }
}
