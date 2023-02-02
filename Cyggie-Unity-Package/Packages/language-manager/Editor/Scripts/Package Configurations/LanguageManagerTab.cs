using Cyggie.LanguageManager.Runtime.Configurations;
using Cyggie.LanguageManager.Runtime.Serializations;
using Cyggie.LanguageManager.Runtime.Services;
using Cyggie.Main.Editor.Configurations;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Serializations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.LanguageManager.Editor.Configurations
{
    /// <summary>
    /// Tab for <see cref="LanguageService"/>
    /// </summary>
    internal class LanguageManagerTab : PackageConfigurationTab
    {
        #region Constants

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
        private const string cSelectedPackLabel = "Selected Pack:";
        private const string cDefaultPackLabel = "Default Pack:";
        private const string cLanguageCodeLabel = "Language Code:";
        private const string cKeyLabel = "Key:";
        private const string cValueLabel = "Value:";
        private const string cSearchLabel = "Search:";

        private const string cDefaultLanguagePack = "No existing language pack. Create a new one.";
        private const string cEmptyTranslations = "No translations has been created yet.";

        #endregion

        // Language pack fields
        private int _selectedLanguageIndex = 0;
        private string _selectedLanguageString = "";

        // Translation fields
        private Vector2 _scrollviewPos = Vector2.zero;
        private int _selectedTranslationIndex = -1;
        private string _editTranslationKey = "";
        private string _editTranslationValue = "";
        private string _searchTranslation = "";

        private bool _databaseRefresh = false;

        private LanguageManagerSettings Settings => (LanguageManagerSettings) _settings;

        /// <inheritdoc/>
        internal override Type SettingsType => typeof(LanguageManagerSettings);

        /// <inheritdoc/>
        internal override string[] SettingsOtherPaths => new string[]
        {
            LanguageManagerSettings.cLanguageFolderPath
        };

        /// <inheritdoc/>
        internal override void OnInitialized()
        {
            LoadFiles();

            if (Settings.LanguagePacks.Count > 0)
            {
                if (_databaseRefresh)
                {
                    _databaseRefresh = false;
                    return;
                }

                _selectedLanguageIndex = 0;
                _selectedLanguageString = Settings.LanguagePacks[_selectedLanguageIndex].LanguageCode;
            }
        }

        /// <inheritdoc/>
        internal override void DrawGUI()
        {
            EditorGUILayout.Space(5);

            // Editor fields
            EditorGUILayout.LabelField(cEditorLabel, EditorStyles.boldLabel);
            Settings.DebugLogs = EditorGUILayout.Toggle(cDebugLogsLabel, Settings.DebugLogs);
            EditorGUILayout.Space(10);

            // Language pack fields
            EditorGUILayout.LabelField(cLanguagePackLabel, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            LanguagePack selectedPack = null;

            // Draw language packs list
            EditorGUIHelper.DrawAsReadOnly(Settings.LanguagePacks.Count == 0, gui: (bool isEmpty) =>
            {
                string[] options = isEmpty ?
                                   new string[1] { cDefaultLanguagePack } :
                                   Settings.LanguagePacks.Select(x => x.LanguageCode).ToArray();

                if (!isEmpty)
                {
                    selectedPack = Settings.LanguagePacks[_selectedLanguageIndex];
                }

                // Draw and check for change in Selected Language pack popup
                if (EditorGUIHelper.CheckChange(gui: () => _selectedLanguageIndex = EditorGUILayout.Popup(cSelectedPackLabel, _selectedLanguageIndex, options)))
                {
                    // Update text field
                    selectedPack = Settings.LanguagePacks[_selectedLanguageIndex];
                    _selectedLanguageString = selectedPack.LanguageCode;

                    _selectedTranslationIndex = -1;
                    _editTranslationKey = "";
                    _editTranslationValue = "";
                }
            });

            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                EditorGUILayout.TextField(cDefaultPackLabel, (Settings.DefaultLanguagePack != null ? Settings.DefaultLanguagePack.LanguageCode : ""));
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
                    EditorGUIHelper.DrawAsReadOnly(Settings.LanguagePacks.Any(x => x.LanguageCode == _selectedLanguageString), gui: () =>
                    {
                        if (GUILayout.Button(cCreateButtonLabel, GUILayout.Width(100)))
                        {
                            _databaseRefresh = true;

                            LanguagePack newPack = new LanguagePack()
                            {
                                LanguageCode = _selectedLanguageString
                            };
                            Settings.LanguagePacks.Add(newPack);

                            SaveFile(Settings.LanguagePacks.Count - 1);

                            // Get the new pack reference due to it being shuffled from order by
                            newPack = Settings.LanguagePacks.FirstOrDefault(x => x.LanguageCode == _selectedLanguageString);
                            _selectedLanguageIndex = Settings.LanguagePacks.IndexOf(newPack);

                            if (!Settings.DefaultLanguagePack.IsAssigned)
                            {
                                Settings.DefaultLanguagePack = newPack;
                                _serializedObject = new SerializedObject(Settings);
                                EditorUtility.SetDirty(Settings);
                            }
                        }
                    });

                    if (selectedPack != null)
                    {
                        // Draw readonly if language code already exists
                        EditorGUIHelper.DrawAsReadOnly(Settings.LanguagePacks.Any(x => x.LanguageCode == _selectedLanguageString), gui: () =>
                        {
                            if (GUILayout.Button(cUpdateButtonLabel, GUILayout.Width(100)))
                            {
                                _databaseRefresh = true;

                                // Update the file
                                UpdateFile(_selectedLanguageIndex, _selectedLanguageString);

                                // Get the new index
                                selectedPack = Enumerable.FirstOrDefault<LanguagePack>(Settings.LanguagePacks, (Func<LanguagePack, bool>) (x => x.LanguageCode == _selectedLanguageString));
                                _selectedLanguageIndex = Settings.LanguagePacks.IndexOf(selectedPack);
                            }
                        });

                        EditorGUIHelper.DrawAsReadOnly(selectedPack.LanguageCode == Settings.DefaultLanguagePack.LanguageCode, gui: () =>
                        {
                            if (GUILayout.Button(cDefaultButtonLabel, GUILayout.Width(100)))
                            {
                                // Update the file
                                Settings.DefaultLanguagePack = selectedPack;
                                _serializedObject = new SerializedObject(Settings);
                                EditorUtility.SetDirty(Settings);
                            }
                        });
                    }
                });

                if (selectedPack != null)
                {
                    if (GUILayout.Button(cDeleteButtonLabel, GUILayout.Width(120)))
                    {
                        _databaseRefresh = true;
                        if (DeleteFile(_selectedLanguageIndex))
                        {
                            _selectedLanguageIndex = Math.Max(_selectedLanguageIndex - 1, 0);

                            if (Settings.LanguagePacks.Any())
                            {
                                selectedPack = Settings.LanguagePacks[_selectedLanguageIndex];
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

                    SaveFile(_selectedLanguageIndex);
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

                        SaveFile(_selectedLanguageIndex);
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

                        SaveFile(_selectedLanguageIndex);
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

        /// <summary>
        /// Load the json files from the <see cref="DataPath"/>
        /// </summary>
        internal void LoadFiles()
        {
            try
            {
                Settings.LanguagePacks.Clear();

                // Create directory if it doesn't exists
                if (!Directory.Exists(Settings.DataPath))
                {
                    Directory.CreateDirectory(Settings.DataPath);
                    return;
                }

                IEnumerable<string> files = Directory.EnumerateFiles(Settings.DataPath, "*.json");
                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string content = File.ReadAllText(filePath);
                    SerializedDictionary<string, string> translations = JsonConvert.DeserializeObject<SerializedDictionary<string, string>>(content);

                    LanguagePack languagePack = new LanguagePack()
                    {
                        LanguageCode = fileName,
                        Translations = translations
                    };

                    Settings.LanguagePacks.Add(languagePack);
                }

                // Check if default language pack is missing
                if (!Settings.DefaultLanguagePack.IsAssigned)
                {
                    // Assign first found
                    Settings.DefaultLanguagePack = Settings.LanguagePacks.FirstOrDefault();
                }
                else
                {
                    // Check if language pack still exists
                    Settings.DefaultLanguagePack = Settings.LanguagePacks.FirstOrDefault(x => x.LanguageCode == Settings.DefaultLanguagePack.LanguageCode);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load files, exception: {ex}, {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Save the json file at index <paramref name="index"/>
        /// </summary>
        /// <param name="index">Index of Language pack to save</param>
        internal void SaveFile(int index)
        {
            if (index >= Settings.LanguagePacks.Count) return;

            // Get language pack and path
            LanguagePack languagePack = Settings.LanguagePacks[index];
            string path = $"{Settings.DataPath}{languagePack.LanguageCode}.json";

            try
            {
                // Create directory to path if it doesn't exist
                FileInfo file = new FileInfo(path);
                file.Directory.Create();
                File.WriteAllText(file.FullName, JsonConvert.SerializeObject(languagePack.Translations, Formatting.Indented));

                if (Settings.DebugLogs)
                {
                    Debug.Log($"Saved file at: \"{path}\". Lang code: {languagePack.LanguageCode}, Count: {languagePack.Count}");
                }

                // Order language packs by language code
                Settings.LanguagePacks = Settings.LanguagePacks.OrderBy(x => x.LanguageCode).ToList();

                // Refresh assets folder
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save the file, exception: {ex}");

                // Revert changes
                Settings.LanguagePacks.Remove(languagePack);
            }
        }

        /// <summary>
        /// Update the json file with a new language code
        /// </summary>
        /// <param name="index">Index of Language pack to update</param>
        /// <param name="newLanguageCode">New language code</param>
        internal void UpdateFile(int index, string newLanguageCode)
        {
            if (index >= Settings.LanguagePacks.Count) return;

            // Get language pack and path
            LanguagePack languagePack = Settings.LanguagePacks[index];

            string oldPath = $"{Settings.DataPath}{languagePack.LanguageCode}.json";
            string path = $"{Settings.DataPath}{newLanguageCode}.json";

            try
            {
                // Update the default language pack if it's the same
                if (languagePack.LanguageCode == Settings.DefaultLanguagePack.LanguageCode)
                {
                    Settings.DefaultLanguagePack = languagePack;
                }

                languagePack.LanguageCode = newLanguageCode; // Update language code

                // Delete old path
                File.Delete(oldPath);

                // Write to new path
                FileInfo file = new FileInfo(path);
                file.Directory.Create();
                File.WriteAllText(file.FullName, JsonConvert.SerializeObject(languagePack.Translations, Formatting.Indented));

                if (Settings.DebugLogs)
                {
                    Debug.Log($"Update file from \"{oldPath}\" to \"{path}\". Lang code: {newLanguageCode}, Count: {languagePack.Count}");
                }

                // Order language packs by language code
                Settings.LanguagePacks = Settings.LanguagePacks.OrderBy(x => x.LanguageCode).ToList();

                // Refresh assets folder
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to update the file, exception: {ex}");
            }
        }

        /// <summary>
        /// Delete the json file
        /// </summary>
        /// <param name="index">Index of Language pack to delete</param>
        /// <returns></returns>
        internal bool DeleteFile(int index)
        {
            if (index >= Settings.LanguagePacks.Count) return false;

            // Get the language pack and the path
            LanguagePack languagePack = Settings.LanguagePacks[index];
            string path = $"{Settings.DataPath}{languagePack.LanguageCode}.json";

            try
            {
                // Delete file at path
                File.Delete(path);

                // Remove from list
                Settings.LanguagePacks.RemoveAt(index);

                if (Settings.DebugLogs)
                {
                    Debug.Log($"Deleting file at: \"{path}\". Lang code: {languagePack.LanguageCode}, Count: {languagePack.Count}");
                }

                // Reassign the default language pack
                if (languagePack.LanguageCode == Settings.DefaultLanguagePack.LanguageCode)
                {
                    Settings.DefaultLanguagePack = Settings.LanguagePacks.FirstOrDefault();
                }

                // Order language packs by language code
                Settings.LanguagePacks = Settings.LanguagePacks.OrderBy(x => x.LanguageCode).ToList();

                // Refresh assets folder
                AssetDatabase.Refresh();

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to update the file, exception: {ex}");
                return false;
            }
        }
    }
}
