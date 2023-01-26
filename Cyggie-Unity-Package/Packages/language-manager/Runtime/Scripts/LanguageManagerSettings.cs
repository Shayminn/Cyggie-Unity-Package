using Cyggie.LanguageManager.Runtime.Serializations;
using Cyggie.LanguageManager.Runtime.Services;
using Cyggie.LanguageManager.Runtime.Utils;
using Cyggie.Main.Runtime.Serializations;
using Cyggie.Main.Runtime.Utils.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.LanguageManager.Runtime.Settings
{
    /// <summary>
    /// Scriptable object to hold settings for managing Languages and Localizations, used by <see cref="LanguageService"/>
    /// </summary>
    public class LanguageManagerSettings : ScriptableObject
    {
        internal const string cLanguageCodePrefKey = "LanguageManager/LanguageCode";

        [SerializeField, Tooltip("List of language packs, each having a language code and its associated translations.")]
        internal List<LanguagePack> LanguagePacks = new List<LanguagePack>();

        [SerializeField, Tooltip("The language pack that should be used by default.")]
        internal LanguagePack DefaultLanguagePack = null;

#if UNITY_EDITOR
        private const string cSettingsAssetPath = "Packages/cyggie.language-manager/Runtime/Resources/" + LanguageManagerConstants.cSettingsFileWithExtension;

        [SerializeField, Tooltip("Whether some debug logs should be displayed (Editor only).")]
        internal bool DebugLogs = true;

        [SerializeField, Tooltip("The folder path for language pack json files (Editor only).")]
        internal string DataPath = "Assets/Resources/LanguageManager/";

        internal static SerializedObject SerializedSettings => new SerializedObject(Settings);

        private static LanguageManagerSettings _settings = null;
        internal static LanguageManagerSettings Settings => _settings.AssignIfNull(GetOrCreateSettings());

        /// <summary>
        /// Get or create if not found new default settings
        /// </summary>
        /// <returns>SceneChangerSettings object</returns>
        public static LanguageManagerSettings GetOrCreateSettings()
        {
            // Try get the relative path to file
            LanguageManagerSettings settings = AssetDatabase.LoadAssetAtPath<LanguageManagerSettings>(cSettingsAssetPath);

            if (settings == null)
            {
                // Settings not found, create a new one
                Debug.Log($"Couldn't find default settings file, creating a new one...");

                settings = CreateInstance<LanguageManagerSettings>();

                AssetDatabase.CreateAsset(settings, cSettingsAssetPath);
                settings.DefaultLanguagePack = null; // for some reason, CreateAsset is assigning DefaultLanguagePack to default values

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"New settings ({nameof(LanguageManagerSettings)}) created at path: \"{cSettingsAssetPath}\".");
            }

            return settings;
        }

        /// <summary>
        /// Keywords for search in Project Settings
        /// </summary>
        /// <returns></returns>
        internal static string[] GetKeywords() => new string[]
        {
            nameof(DebugLogs),
            nameof(DataPath),
            nameof(LanguagePacks)
        };

        /// <summary>
        /// Move <see cref="LanguagePacks"/> from one folder <paramref name="oldDataPath"/> to <see cref="DataPath"/>
        /// </summary>
        /// <param name="oldDataPath">The old data path</param>
        internal void MoveDataPath(string oldDataPath)
        {
            // Check if the new path is a folder path
            if (!DataPath.EndsWith('/'))
            {
                DataPath = oldDataPath;
                Debug.LogError($"Data path is not a folder path, reverting changes...");
                return;
            }

            // Check if the directory exists
            if (!Directory.Exists(DataPath))
            {
                if (DebugLogs)
                {
                    Debug.Log($"Directory path \"{DataPath}\" doesn't exist yet, creating it...");
                }

                try
                {
                    // Create the directory to the data path
                    Directory.CreateDirectory(DataPath);
                }
                // Catch IO exceptions or any other exceptions, reverting the data path
                catch (Exception ex)
                {
                    DataPath = oldDataPath;
                    Debug.LogError(ex);
                    return;
                }
            }

            // Move language packs json file if any
            if (LanguagePacks.Count > 0)
            {
                if (DebugLogs)
                {
                    Debug.Log($"Moving files from \"{oldDataPath}\" to \"{DataPath}\"");
                    Debug.Log($"The old folder path is not automatically deleted. Path: {oldDataPath}");
                }

                foreach (LanguagePack languagePack in LanguagePacks)
                {
                    File.Move($"{oldDataPath}{languagePack.LanguageCode}.json", $"{DataPath}{languagePack.LanguageCode}.json");
                }

                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// Load the json files from the <see cref="DataPath"/>
        /// </summary>
        internal void LoadFiles()
        {
            try
            {
                LanguagePacks.Clear();

                IEnumerable<string> files = Directory.EnumerateFiles(DataPath, "*.json");
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

                    LanguagePacks.Add(languagePack);
                }

                // Check if default language pack is missing
                if (DefaultLanguagePack == null)
                {
                    // Assign first found
                    DefaultLanguagePack = LanguagePacks.FirstOrDefault();
                }
                else
                {
                    // Check if language pack still exists
                    DefaultLanguagePack = LanguagePacks.FirstOrDefault(x => x.LanguageCode == DefaultLanguagePack.LanguageCode);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load files, exception: {ex}");
            }
        }

        /// <summary>
        /// Save the json file at index <paramref name="index"/>
        /// </summary>
        /// <param name="index">Index of Language pack to save</param>
        internal void SaveFile(int index)
        {
            if (index >= LanguagePacks.Count) return;

            // Get language pack and path
            LanguagePack languagePack = LanguagePacks[index];
            string path = $"{DataPath}{languagePack.LanguageCode}.json";

            try
            {
                // Create directory to path if it doesn't exist
                FileInfo file = new FileInfo(path);
                file.Directory.Create();
                File.WriteAllText(file.FullName, JsonConvert.SerializeObject(languagePack.Translations, Formatting.Indented));

                if (DebugLogs)
                {
                    Debug.Log($"Saved file at: \"{path}\". Lang code: {languagePack.LanguageCode}, Count: {languagePack.Count}");
                }

                // Order language packs by language code
                LanguagePacks = LanguagePacks.OrderBy(x => x.LanguageCode).ToList();

                // Refresh assets folder
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save the file, exception: {ex}");

                // Revert changes
                LanguagePacks.Remove(languagePack);
            }
        }

        /// <summary>
        /// Update the json file with a new language code
        /// </summary>
        /// <param name="index">Index of Language pack to update</param>
        /// <param name="newLanguageCode">New language code</param>
        internal void UpdateFile(int index, string newLanguageCode)
        {
            if (index >= LanguagePacks.Count) return;

            // Get language pack and path
            LanguagePack languagePack = LanguagePacks[index];

            string oldPath = $"{DataPath}{languagePack.LanguageCode}.json";
            string path = $"{DataPath}{newLanguageCode}.json";

            try
            {
                // Update the default language pack if it's the same
                if (languagePack.LanguageCode == DefaultLanguagePack.LanguageCode)
                {
                    DefaultLanguagePack = languagePack;
                }

                languagePack.LanguageCode = newLanguageCode; // Update language code

                // Delete old path
                File.Delete(oldPath);

                // Write to new path
                FileInfo file = new FileInfo(path);
                file.Directory.Create();
                File.WriteAllText(file.FullName, JsonConvert.SerializeObject(languagePack.Translations, Formatting.Indented));

                if (DebugLogs)
                {
                    Debug.Log($"Update file from \"{oldPath}\" to \"{path}\". Lang code: {newLanguageCode}, Count: {languagePack.Count}");
                }

                // Order language packs by language code
                LanguagePacks = LanguagePacks.OrderBy(x => x.LanguageCode).ToList();

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
            if (index >= LanguagePacks.Count) return false;

            // Get the language pack and the path
            LanguagePack languagePack = LanguagePacks[index];
            string path = $"{DataPath}{languagePack.LanguageCode}.json";

            try
            {
                // Delete file at path
                File.Delete(path);

                // Remove from list
                LanguagePacks.RemoveAt(index);

                if (DebugLogs)
                {
                    Debug.Log($"Deleting file at: \"{path}\". Lang code: {languagePack.LanguageCode}, Count: {languagePack.Count}");
                }

                // Reassign the default language pack
                if (languagePack.LanguageCode == DefaultLanguagePack.LanguageCode)
                {
                    DefaultLanguagePack = LanguagePacks.FirstOrDefault();
                }

                // Order language packs by language code
                LanguagePacks = LanguagePacks.OrderBy(x => x.LanguageCode).ToList();

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
#endif
    }
}