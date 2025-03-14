using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.ServicesNS.ScriptableObjects;
using Cyggie.Plugins.Editor.Helpers;
using Cyggie.Plugins.Services.Interfaces;
using Cyggie.Plugins.UnityServices.Models;
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
        private const string cServiceConfigurationsPath = Runtime.Utils.Constants.FolderConstants.cAssets + Runtime.Utils.Constants.FolderConstants.cCyggieServiceConfigurations;

        private List<Type> _serviceTypes = null;
        private Type _selectedType = null;
        private int _selectedTypeIndex = -1;
        private string _configFilePath = "";
        private ServiceConfigurationSO _foundObject = null;

        private string _searchText = "";
        private Vector2 _scrollbarPosition = Vector2.zero;

        private bool _showCreated = false;
        private bool _confirmDelete = false;

        private int _previousTypeCount = 0;

        private void OnEnable()
        {
            _serviceTypes = ReflectionHelper.GetAllIsAssignableFrom<IServiceConfiguration>(type =>
            {
                // Type must derive from ScriptableObject to be created through the window
                return typeof(ScriptableObject).IsAssignableFrom(type) &&
                       !type.IsAbstract && // Configuration must not be abstract or it can't be created
                       !typeof(PackageServiceConfiguration).IsAssignableFrom(type) && // Configuration must not be part of PackageServiceConfiguration, those are automatically created
                       type != typeof(ServiceManagerSettings); // Service manager settings are always auto-created
            }).ToList();
        }

        private void OnGUI()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Close();
                return;
            }

            // Reset values when it's null
            if (_selectedType == null)
            {
                _selectedTypeIndex = -1;
                _confirmDelete = false;
            }

            EditorGUILayout.Space(5);

            EditorGUILayoutHelper.DrawHorizontal(gui: (Action) (() =>
            {
                EditorGUILayout.Space(2);
                EditorGUILayoutHelper.DrawVertical(gui: (Action) (() =>
                {
                    // Instructions
                    EditorGUILayout.LabelField("Select a service configuration type and create a new identifier that you can assign in");
                    EditorGUILayout.LabelField("the Service Configuration window (ALT + C).");
                    EditorGUILayout.Space(5);

                    // Search bar
                    EditorGUILayoutHelper.DrawHorizontal(gui: () =>
                    {
                        EditorGUILayout.LabelField("Search: ", GUILayout.Width(50));
                        _searchText = EditorGUILayout.TextField(_searchText);
                    });
                    EditorGUILayout.Space(2);

                    // Extra filters
                    float oldWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 95;
                    _showCreated = EditorGUILayout.Toggle("Display created", _showCreated, GUILayout.Width(50));
                    EditorGUIUtility.labelWidth = oldWidth;
                    EditorGUILayout.Space(2);

                    // Scroll view list
                    EditorGUILayoutHelper.DrawWithScrollview(
                        ref _scrollbarPosition,
                        gui: () =>
                        {
                            // Filter types by search text
                            IEnumerable<Type> filteredTypes = _serviceTypes.Where(x => x.FullName.Contains(_searchText, StringComparison.InvariantCultureIgnoreCase));
                            if (!_showCreated)
                            {
                                filteredTypes = filteredTypes.Where(x => !CheckTypeExists(x));
                            }

                            _previousTypeCount = filteredTypes.Count();
                            if (_selectedTypeIndex >= _previousTypeCount)
                            {
                                _selectedTypeIndex = -1;
                                _selectedType = null;
                            }

                            if (_previousTypeCount == 0)
                            {
                                EditorGUILayout.LabelField("No service configurations found.");
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
                        bool configExists = CheckTypeExists(_selectedType);

                        // Check if it already exists at path
                        if (configExists)
                        {
                            GUIHelper.DrawWithColor(Color.yellow, gui: () =>
                            {
                                EditorGUILayout.LabelField(new GUIContent("Service Configuration already exists! But no worries, I'm not stopping you =D"));
                            });

                            // Draw reference to the scriptable object
                            GUIHelper.DrawAsReadOnly(gui: () =>
                            {
                                float oldWidth = EditorGUIUtility.labelWidth;
                                EditorGUIUtility.labelWidth = 100;
                                EditorGUILayout.ObjectField("Scriptable Object", _foundObject, typeof(ServiceConfigurationSO), allowSceneObjects: false);
                                EditorGUIUtility.labelWidth = oldWidth;
                            });
                            EditorGUILayout.Space(3);
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
                            ScriptableObject config = ScriptableObject.CreateInstance(_selectedType);
                            config.name = _selectedType.Name;

                            AssetDatabaseHelper.CreateAsset(config, $"{cServiceConfigurationsPath}{config.name}{FileExtensionConstants.cAsset}");
                        }

                        if (configExists)
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

        private bool CheckTypeExists(Type type)
        {
            // Make sure directory exists
            Directory.CreateDirectory(cServiceConfigurationsPath);

            string[] files = Directory.GetFiles(cServiceConfigurationsPath, $"*{FileExtensionConstants.cAsset}");
            foreach (string file in files)
            {
                _foundObject = AssetDatabase.LoadAssetAtPath<ServiceConfigurationSO>(file);
                if (_foundObject == null) continue;
                if (_foundObject.GetType() == type)
                {
                    _configFilePath = file;
                    return true;
                }
            }

            return false;
        }
    }
}
