using Cyggie.LanguageManager.Runtime.Services;
using Cyggie.LanguageManager.Runtime.Settings;
using Cyggie.Main.Editor.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cyggie.LanguageManager.Editor.SettingsProviders
{
    /// <summary>
    /// IMGUI to <see cref="LanguageManagerSettings"/>
    /// </summary>
    static class LanguageManagerSettingsIMGUI
    {
        // Settings strings
        private static readonly string cSettingsPath = "Cyggie/LanguageManager";
        private static readonly string cSettingsLabel = "Language Manager";

        private static LanguageManagerSettings _settings = null;

        private static int _selectedLanguageIndex = 0;
        private static string _selectedLanguageString = "";

        private static Vector2 _scrollviewPos = Vector2.zero;
        private static int _selectedTranslationIndex = -1;
        private static LanguageEntry _editTranslationEntry = new LanguageEntry();
        private static string _searchTranslation = "";

        private static bool _databaseRefresh = false;

        /// <summary>
        /// Create a settings provider at Project Settings/Cyggie/LanguageManager
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider(cSettingsPath, SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = cSettingsLabel,

                activateHandler = OnActiveHandler,

                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = OnSettingsGUI,

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = LanguageManagerSettings.GetKeywords()
            };

            return provider;
        }

        /// <summary>
        /// Event handler to active settings (when it's first opened)
        /// </summary>
        private static void OnActiveHandler(string _, VisualElement __)
        {
            _settings = LanguageManagerSettings.Settings;
            _settings.LoadFiles();

            if (_settings.LanguagePacks.Count > 0)
            {
                if (_databaseRefresh)
                {
                    _databaseRefresh = false;
                    return;
                }

                _selectedLanguageIndex = 0;
                _selectedLanguageString = _settings.LanguagePacks[_selectedLanguageIndex].LanguageCode;
            }
        }

        /// <summary>
        /// GUI Handler for creating the settings UI
        /// </summary>
        private static void OnSettingsGUI(string _)
        {
            EditorGUILayout.Space(5);

            // Editor fields
            EditorGUILayout.LabelField("Editor", EditorStyles.boldLabel);
            _settings.DebugLogs = EditorGUILayout.Toggle("Debug Logs", _settings.DebugLogs);

            string oldDataPath = _settings.DataPath;
            if (EditorGUIHelper.CheckChange(() => _settings.DataPath = EditorGUILayout.DelayedTextField("Data Path:", _settings.DataPath)))
            {
                _settings.MoveDataPath(oldDataPath);
            }
            EditorGUILayout.Space(10);

            // Language pack fields
            EditorGUILayout.LabelField("Language Pack", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            LanguagePack selectedPack = null;

            // Draw language packs list
            EditorGUIHelper.DrawAsReadOnly(_settings.LanguagePacks.Count == 0, gui: (bool isEmpty) =>
            {
                string[] options = isEmpty ?
                                   new string[1] { "No existing language pack. Create a new one." } :
                                   _settings.LanguagePacks.Select(x => x.LanguageCode).ToArray();

                if (!isEmpty)
                {
                    selectedPack = _settings.LanguagePacks[_selectedLanguageIndex];
                }

                // Draw and check for change in Selected Language pack popup
                if (EditorGUIHelper.CheckChange(gui: () => _selectedLanguageIndex = EditorGUILayout.Popup("Selected Pack:", _selectedLanguageIndex, options)))
                {
                    // Update text field
                    selectedPack = _settings.LanguagePacks[_selectedLanguageIndex];
                    _selectedLanguageString = selectedPack.LanguageCode;

                    _selectedTranslationIndex = -1;
                    _editTranslationEntry = new LanguageEntry();
                }

                EditorGUILayout.Space(5);
            });

            // Draw input fields for language code
            EditorGUIHelper.DrawHorizontal(gui: () =>
            {
                //EditorGUILayout.LabelField("Language Code:", GUILayout.Width(120));
                _selectedLanguageString = EditorGUILayout.TextField("Language Code:", _selectedLanguageString);
            });
            EditorGUILayout.Space(5);

            // Draw buttons for language pack
            EditorGUIHelper.DrawHorizontal(gui: () =>
            {
                EditorGUIHelper.DrawAsReadOnly(string.IsNullOrEmpty(_selectedLanguageString), gui: () =>
                {
                    // Draw readonly if language code already exists
                    EditorGUIHelper.DrawAsReadOnly(_settings.LanguagePacks.Any(x => x.LanguageCode == _selectedLanguageString), gui: () =>
                    {
                        if (GUILayout.Button("Create", GUILayout.Width(100)))
                        {
                            _databaseRefresh = true;

                            LanguagePack newPack = new LanguagePack(_selectedLanguageString);
                            _settings.LanguagePacks.Add(newPack);

                            _selectedLanguageIndex = _settings.LanguagePacks.IndexOf(newPack);
                            _settings.SaveFile(_selectedLanguageIndex);
                        }
                    });

                    if (selectedPack != null)
                    {
                        // Draw readonly if language code already exists
                        EditorGUIHelper.DrawAsReadOnly(_settings.LanguagePacks.Any(x => x.LanguageCode == _selectedLanguageString), gui: () =>
                        {
                            if (GUILayout.Button("Update", GUILayout.Width(100)))
                            {
                                _databaseRefresh = true;

                                // Update the file
                                _settings.UpdateFile(_selectedLanguageIndex, _selectedLanguageString);

                                // Get the new index
                                selectedPack = _settings.LanguagePacks.FirstOrDefault(x => x.LanguageCode == _selectedLanguageString);
                                _selectedLanguageIndex = _settings.LanguagePacks.IndexOf(selectedPack);
                            }
                        });
                    }
                });

                if (selectedPack != null)
                {
                    if (GUILayout.Button("Delete", GUILayout.Width(120)))
                    {
                        _databaseRefresh = true;
                        if (_settings.DeleteFile(_selectedLanguageIndex))
                        {
                            _selectedLanguageIndex = Math.Max(_selectedLanguageIndex - 1, 0);

                            if (_settings.LanguagePacks.Any())
                            {
                                selectedPack = _settings.LanguagePacks[_selectedLanguageIndex];
                                _selectedLanguageString = selectedPack.LanguageCode;
                            }
                            else
                            {
                                selectedPack = null;
                                _selectedLanguageString = "";
                                GUI.FocusControl(null);
                            }
                        }
                    }
                }
            });
            EditorGUILayout.Space(10);

            // Draw translations 
            EditorGUILayout.LabelField("Translations" + (selectedPack == null ? "" : $" ({selectedPack.Count})"), EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUIHelper.DrawAsReadOnly(false, gui: () =>
            {
                EditorGUIHelper.DrawAsReadOnly(_selectedTranslationIndex != -1, gui: () =>
                {
                    _editTranslationEntry.Key = EditorGUILayout.TextField("Key:", _editTranslationEntry.Key);
                });
                _editTranslationEntry.Value = EditorGUILayout.TextField("Value:", _editTranslationEntry.Value);
                EditorGUILayout.Space(5);

                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUIHelper.DrawAsReadOnly(string.IsNullOrEmpty(_editTranslationEntry.Key) || selectedPack.ContainsKey(_editTranslationEntry.Key), gui: () =>
                    {
                        if (GUILayout.Button("Create", GUILayout.Width(100)))
                        {
                            selectedPack.Add(_editTranslationEntry);
                            _editTranslationEntry = new LanguageEntry();
                            GUI.FocusControl(null);

                            _settings.SaveFile(_selectedLanguageIndex);
                        }
                    });

                    EditorGUIHelper.DrawAsReadOnly(_selectedTranslationIndex == -1, gui: () =>
                    {
                        if (GUILayout.Button("Delete", GUILayout.Width(100)))
                        {
                            selectedPack.Delete(_editTranslationEntry);

                            if (selectedPack.Any)
                            {
                                _selectedTranslationIndex = Math.Max(--_selectedTranslationIndex, 0);
                                _editTranslationEntry = selectedPack.Translations[_selectedTranslationIndex];
                            }
                            else
                            {
                                _selectedTranslationIndex = -1;
                                _editTranslationEntry = new LanguageEntry();
                                GUI.FocusControl(null);
                            }

                            _settings.SaveFile(_selectedLanguageIndex);
                        }

                        if (GUILayout.Button("Deselect", GUILayout.Width(100)))
                        {
                            _selectedTranslationIndex = -1;
                            _editTranslationEntry = new LanguageEntry();
                        }
                    });
                });
                EditorGUILayout.Space(10);
            });

            if (_selectedTranslationIndex == -1)
            {
                if (!string.IsNullOrEmpty(_editTranslationEntry.Key))
                {
                    if (selectedPack.ContainsKey(_editTranslationEntry.Key))
                    {
                        EditorGUILayout.HelpBox($"Language Pack already contains key: {_editTranslationEntry.Key}, value: {_editTranslationEntry.Value}", MessageType.Error);
                        EditorGUILayout.Space(10);
                    }
                }
            }

            if (selectedPack == null || !selectedPack.Any)
            {
                EditorGUILayout.LabelField("No translations has been created yet.");
            }
            else
            {
                _searchTranslation = EditorGUILayout.TextField("Search:", _searchTranslation);
                EditorGUIHelper.DrawWithScrollview(ref _scrollviewPos, gui: () =>
                {
                    List<LanguageEntry> entries = selectedPack.GetTranslations(_searchTranslation);
                    if (EditorGUIHelper.CheckChange(gui: () => _selectedTranslationIndex = GUILayout.SelectionGrid(_selectedTranslationIndex, entries.Select(x => $"{x.Key}: {x.Value}").ToArray(), 1)))
                    {
                        _editTranslationEntry = entries[_selectedTranslationIndex];
                        GUI.FocusControl(null);
                    }
                });
            }
        }
    }
}
