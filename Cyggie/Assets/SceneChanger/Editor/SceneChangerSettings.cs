using Cyggie.Utils.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cyggie.SceneChanger.Editor
{
    // Create a new type of Settings Asset.
    public class SceneChangerSettings : ScriptableObject
    {
        private const string cSettingsFileName = "SceneChangerSettings.asset";
        private const string cSettingsScriptFileName = "SceneChangerSettingsIMGUI.cs";

        [SerializeField, Tooltip("")]
        private int m_Number;

        [SerializeField, Tooltip("")]
        private string m_SomeString;

        private SceneChangerSettings()
        {
            m_Number = 42;
            m_SomeString = "The answer to the universe";
        }

        private static SerializedObject _serializedSettings = null;
        public static SerializedObject SerializedSettings => _serializedSettings ??= new SerializedObject(GetOrCreateSettings());

        private static SceneChangerSettings GetOrCreateSettings()
        {
            // Try get the relative path to file
            if (!FileHelper.TryGetRelativePath(cSettingsFileName, out string path))
            {
                Debug.Log($"Couldn't find default settings file, creating a new one...");

                // Try get the relative path to the script file
                if (!FileHelper.TryGetRelativePath(cSettingsScriptFileName, out path))
                {
                    Debug.LogError($"Even the script file was not found! There's no way out of this one captain, we gotta do what we must... reimport." + path);
                    return null;
                }

                // Replace script file name by settings file name
                path = path.Replace(cSettingsScriptFileName, cSettingsFileName);

                // File not found, create new settings
                SceneChangerSettings settings = CreateInstance<SceneChangerSettings>();
                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.SaveAssets();

                Debug.Log($"New settings ({nameof(SceneChangerSettings)} created at path: {path}.");

                return settings;
            }

            // Return settings at file path
            return AssetDatabase.LoadAssetAtPath<SceneChangerSettings>(path);
        }
    }
}
