using Cyggie.LanguageManager.Runtime.Serializations;
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
        #region Constants

        // Settings strings
        private const string cSettingsPath = "Cyggie/LanguageManager";
        private const string cSettingsLabel = "Language Manager";

        // Header labels
        private const string cEditorLabel = "Editor";
        private const string cLanguagePackLabel = "Language Pack";

        // Button labels
        private const string cCreateButtonLabel = "Create";
        private const string cUpdateButtonLabel = "Update";
        private const string cDefaultButtonLabel = "Default";
        private const string cDeleteButtonLabel = "Delete";
        private const string cDeselectButtonLabel = "Deselect";

        // Text fields labels
        private const string cDebugLogsLabel = "Debug Logs";
        private const string cDataPathLabel = "Data Path:";
        private const string cSelectedPackLabel = "Selected Pack:";
        private const string cDefaultPackLabel = "Default Pack:";
        private const string cLanguageCodeLabel = "Language Code:";
        private const string cKeyLabel = "Key:";
        private const string cValueLabel = "Value:";
        private const string cSearchLabel = "Search:";

        private const string cDefaultLanguagePack = "No existing language pack. Create a new one.";
        private const string cEmptyTranslations = "No translations has been created yet.";

        #endregion

        private static SerializedObject _serializedObject = null;
        private static LanguageManagerSettings _settings = null;

        // Language pack fields
        private static int _selectedLanguageIndex = 0;
        private static string _selectedLanguageString = "";

        // Translation fields
        private static Vector2 _scrollviewPos = Vector2.zero;
        private static int _selectedTranslationIndex = -1;
        private static string _editTranslationKey = "";
        private static string _editTranslationValue = "";
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
            _serializedObject = LanguageManagerSettings.SerializedSettings;
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
            EditorGUILayout.LabelField(cEditorLabel, EditorStyles.boldLabel);
            _settings.DebugLogs = EditorGUILayout.Toggle(cDebugLogsLabel, _settings.DebugLogs);

            string oldDataPath = _settings.DataPath;
            if (EditorGUIHelper.CheckChange(() => _settings.DataPath = EditorGUILayout.DelayedTextField(cDataPathLabel, _settings.DataPath)))
            {
                _settings.MoveDataPath(oldDataPath);
            }
            EditorGUILayout.Space(10);

            // Language pack fields
            EditorGUILayout.LabelField(cLanguagePackLabel, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            LanguagePack selectedPack = null;

            // Draw language packs list
            EditorGUIHelper.DrawAsReadOnly(_settings.LanguagePacks.Count == 0, gui: (bool isEmpty) =>
            {
                string[] options = isEmpty ?
                                   new string[1] { cDefaultLanguagePack } :
                                   _settings.LanguagePacks.Select(x => x.LanguageCode).ToArray();

                if (!isEmpty)
                {
                    selectedPack = _settings.LanguagePacks[_selectedLanguageIndex];
                }

                // Draw and check for change in Selected Language pack popup
                if (EditorGUIHelper.CheckChange(gui: () => _selectedLanguageIndex = EditorGUILayout.Popup(cSelectedPackLabel, _selectedLanguageIndex, options)))
                {
                    // Update text field
                    selectedPack = _settings.LanguagePacks[_selectedLanguageIndex];
                    _selectedLanguageString = selectedPack.LanguageCode;

                    _selectedTranslationIndex = -1;
                    _editTranslationKey = "";
                    _editTranslationValue = "";
                }
            });

            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                EditorGUILayout.TextField(cDefaultPackLabel, _settings.DefaultLanguagePack != null ? _settings.DefaultLanguagePack.LanguageCode : "");
                EditorGUILayout.Space(5);
            });

            // Draw input fields for language code
            EditorGUIHelper.DrawHorizontal(gui: () =>
            {
                _selectedLanguageString = EditorGUILayout.TextField(cLanguageCodeLabel, _selectedLanguageString);
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
                        if (GUILayout.Button(cCreateButtonLabel, GUILayout.Width(100)))
                        {
                            _databaseRefresh = true;

                            LanguagePack newPack = new LanguagePack()
                            {
                                LanguageCode = _selectedLanguageString
                            };
                            _settings.LanguagePacks.Add(newPack);

                            _settings.SaveFile(_settings.LanguagePacks.Count - 1);

                            // Get the new pack reference due to it being shuffled from order by
                            newPack = _settings.LanguagePacks.FirstOrDefault(x => x.LanguageCode == _selectedLanguageString);
                            _selectedLanguageIndex = _settings.LanguagePacks.IndexOf(newPack);

                            _settings.DefaultLanguagePack ??= newPack;
                        }
                    });

                    if (selectedPack != null)
                    {
                        // Draw readonly if language code already exists
                        EditorGUIHelper.DrawAsReadOnly(_settings.LanguagePacks.Any(x => x.LanguageCode == _selectedLanguageString), gui: () =>
                        {
                            if (GUILayout.Button(cUpdateButtonLabel, GUILayout.Width(100)))
                            {
                                _databaseRefresh = true;

                                // Update the file
                                _settings.UpdateFile(_selectedLanguageIndex, _selectedLanguageString);

                                // Get the new index
                                selectedPack = _settings.LanguagePacks.FirstOrDefault(x => x.LanguageCode == _selectedLanguageString);
                                _selectedLanguageIndex = _settings.LanguagePacks.IndexOf(selectedPack);
                            }
                        });

                        EditorGUIHelper.DrawAsReadOnly(selectedPack.LanguageCode == _settings.DefaultLanguagePack.LanguageCode, gui: () =>
                        {
                            if (GUILayout.Button(cDefaultButtonLabel, GUILayout.Width(100)))
                            {
                                // Update the file
                                _settings.DefaultLanguagePack = selectedPack;
                            }
                        });
                    }
                });

                if (selectedPack != null)
                {
                    if (GUILayout.Button(cDeleteButtonLabel, GUILayout.Width(120)))
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

            // Draw input values
            EditorGUIHelper.DrawAsReadOnly(_selectedTranslationIndex != -1, gui: () =>
            {
                _editTranslationKey = EditorGUILayout.TextField(cKeyLabel, _editTranslationKey);
            });

            // Draw text field for creating new key-value pair
            if (_selectedTranslationIndex == -1)
            {
                _editTranslationValue = EditorGUILayout.TextField(cValueLabel, _editTranslationValue);
            }
            // Draw delayed text field for updating existing key-value pair
            else
            {
                if (EditorGUIHelper.CheckChange(() => _editTranslationValue = EditorGUILayout.DelayedTextField(cValueLabel, _editTranslationValue)))
                {
                    selectedPack.Translations[_editTranslationKey] = _editTranslationValue;

                    _settings.SaveFile(_selectedLanguageIndex);
                }
            }
            EditorGUILayout.Space(5);

            // Draw translation control buttons
            EditorGUIHelper.DrawHorizontal(gui: () =>
            {
                EditorGUIHelper.DrawAsReadOnly(string.IsNullOrEmpty(_editTranslationKey) || selectedPack.ContainsKey(_editTranslationKey), gui: () =>
                {
                    if (GUILayout.Button(cCreateButtonLabel, GUILayout.Width(100)))
                    {
                        // Unfocus input control
                        GUI.FocusControl(null);

                        selectedPack.Add(_editTranslationKey, _editTranslationValue);

                        // Reset input values
                        _editTranslationKey = "";
                        _editTranslationValue = "";

                        _settings.SaveFile(_selectedLanguageIndex);
                    }
                });

                EditorGUIHelper.DrawAsReadOnly(_selectedTranslationIndex == -1, gui: () =>
                {
                    if (GUILayout.Button(cDeselectButtonLabel, GUILayout.Width(100)))
                    {
                        // Reset input values
                        _selectedTranslationIndex = -1;
                        _editTranslationKey = "";
                        _editTranslationValue = "";
                        GUI.FocusControl(null);
                    }

                    if (GUILayout.Button(cDeleteButtonLabel, GUILayout.Width(100)))
                    {
                        selectedPack.Delete(_editTranslationKey);

                        // Reset input values
                        _selectedTranslationIndex = -1;
                        _editTranslationKey = "";
                        _editTranslationValue = "";
                        GUI.FocusControl(null);

                        _settings.SaveFile(_selectedLanguageIndex);
                    }
                });
            });
            EditorGUILayout.Space(10);

            if (_selectedTranslationIndex == -1)
            {
                if (!string.IsNullOrEmpty(_editTranslationKey))
                {
                    if (selectedPack.ContainsKey(_editTranslationKey))
                    {
                        EditorGUILayout.HelpBox($"Language Pack already contains key: {_editTranslationKey}, value: {_editTranslationValue}", MessageType.Error);
                        EditorGUILayout.Space(10);
                    }
                }
            }

            if (selectedPack == null || !selectedPack.Any)
            {
                EditorGUILayout.LabelField(cEmptyTranslations);
            }
            else
            {
                _searchTranslation = EditorGUILayout.TextField(cSearchLabel, _searchTranslation);
                EditorGUIHelper.DrawWithScrollview(ref _scrollviewPos, gui: () =>
                {
                    Dictionary<string, string> entries = selectedPack.GetTranslations(_searchTranslation);
                    if (EditorGUIHelper.CheckChange(gui: () => _selectedTranslationIndex = GUILayout.SelectionGrid(_selectedTranslationIndex, entries.Select(x => $"{x.Key}: {x.Value}").ToArray(), 1)))
                    {
                        string key = entries.Keys.ToList()[_selectedTranslationIndex];
                        _editTranslationKey = key;
                        _editTranslationValue = entries[key];
                        GUI.FocusControl(null);
                    }
                });
            }

            _serializedObject.ApplyModifiedProperties();
        }
    }
}
