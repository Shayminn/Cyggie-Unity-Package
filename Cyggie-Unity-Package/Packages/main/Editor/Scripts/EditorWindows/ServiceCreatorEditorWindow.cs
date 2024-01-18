using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Main.Runtime.ServicesNS.ScriptableObjects;
using Cyggie.Plugins.Editor.Helpers;
using Cyggie.Plugins.Services.Interfaces;
using Cyggie.Plugins.Utils.Constants;
using Cyggie.Plugins.Utils.Extensions;
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
    /// Editor window to create a <see cref="ServiceIdentifier"/>
    /// </summary>
    public class ServiceCreatorEditorWindow : EditorWindow
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
            _serviceTypes = TypeHelper.GetAllIsAssignableFrom<IService>(
                // Comment out to create PackageServiceMono
                x =>!typeof(PackageServiceMono).IsAssignableFrom(x) && !x.IsSubclassOfGenericType(typeof(PackageServiceMono<>))
            ).ToList();
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

            EditorGUILayoutHelper.DrawHorizontal(gui: () =>
            {
                EditorGUILayout.Space(2);
                EditorGUILayoutHelper.DrawVertical(gui: () =>
                {
                    // Instructions
                    EditorGUILayout.LabelField("Select a service type and create a new identifier that you can assign in the Service Configuration");
                    EditorGUILayout.LabelField("window (ALT + C).");
                    EditorGUILayout.Space(5);

                    // Search bar
                    EditorGUILayoutHelper.DrawHorizontal(gui: () =>
                    {
                        EditorGUILayout.LabelField("Search: ", GUILayout.Width(50));
                        _searchText = EditorGUILayout.TextField(_searchText);
                    });
                    EditorGUILayout.Space(2);

                    // Scroll view list
                    EditorGUILayoutHelper.DrawWithScrollview(
                        ref _scrollbarPosition,
                        gui: () =>
                        {
                            // Filter types by search text
                            IEnumerable<Type> filteredTypes = _serviceTypes.Where(x => x.FullName.Contains(_searchText, StringComparison.InvariantCultureIgnoreCase));

                            if (filteredTypes.Count() == 0)
                            {
                                EditorGUILayout.LabelField("No services found.");
                            }
                            else
                            {
                                if (EditorGUILayoutHelper.CheckChange(gui: () => _selectedTypeIndex = GUILayout.SelectionGrid(_selectedTypeIndex, filteredTypes.Select(x => new GUIContent(x.Name, x.FullName)).ToArray(), 1)))
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
                        string path = $"{Runtime.Utils.Constants.FolderConstants.cAssets}{Runtime.Utils.Constants.FolderConstants.cCyggieServiceIdentifiers}";

                        // Make sure directory exists
                        Directory.CreateDirectory(path);

                        string[] files = Directory.GetFiles(path, $"*{FileExtensionConstants.cAsset}");
                        bool identifierExists = false;
                        foreach (string file in files)
                        {
                            ServiceIdentifier identifier = AssetDatabase.LoadAssetAtPath<ServiceIdentifier>(file);
                            if (identifier == null) continue;
                            if (identifier.AssemblyName == _selectedType.AssemblyQualifiedName)
                            {
                                _configFilePath = file;
                                identifierExists = true;
                                break;
                            }
                        }

                        // Check if it already exists at path
                        if (identifierExists)
                        {
                            GUIHelper.DrawWithColor(Color.yellow, gui: () =>
                            {
                                EditorGUILayout.LabelField(new GUIContent("Service identifier already exists! But no worries, I'm not stopping you =D"));
                            });
                        }
                        else
                        {
                            selectedText = _selectedType.FullName;

                            EditorGUILayoutHelper.DrawHorizontal(gui: () =>
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
                            ServiceIdentifier identifier = ScriptableObject.CreateInstance<ServiceIdentifier>();
                            identifier.AssemblyName = _selectedType.AssemblyQualifiedName;
                            identifier.name = $"{_selectedType.Name}Identifier";

                            AssetDatabaseHelper.CreateAsset(identifier, $"{path}{identifier.name}{FileExtensionConstants.cAsset}");
                        }

                        if (identifierExists)
                        {
                            EditorGUILayoutHelper.DrawWithConfirm(ref _confirmDelete,
                                onInactiveGUI: () =>
                                {
                                    if (GUILayout.Button("Delete", GUILayout.Width(50)))
                                    {
                                        _confirmDelete = true;
                                    }
                                },
                                confirmLabel: "Are you sure you want to delete?",
                                onConfirm: () =>
                                {
                                    AssetDatabaseHelper.DeleteAsset(_configFilePath);
                                    _confirmDelete = false;
                                }
                            );
                        }
                    }
                });
                EditorGUILayout.Space(2);
            });
        }
    }
}
