using Cyggie.Main.Editor.Utils.Constants;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Plugins.Logs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Windows
{
    /// <summary>
    /// Editor window for managing editor configurations
    /// </summary>
    public class EditorConfigurationsEditorWindow : EditorWindow
    {
        private EditorConfigurations _configurations = null;
        private SerializedObject _serializedConfigurations = null;

        private SerializedProperty _forcePlayScene = null;
        private SerializedProperty _forcePlayScenePath = null;
        private SerializedProperty _forcePlaySceneName = null;
        private SerializedProperty _clearLogs = null;
        private SceneAsset _sceneAsset = null;

        private void OnEnable()
        {
            _configurations = AssetDatabase.LoadAssetAtPath<EditorConfigurations>(EditorConfigurations.cAssetPath);

            if (_configurations == null)
            {
                ScriptableObject config = ScriptableObject.CreateInstance<EditorConfigurations>();
                config.name = EditorConfigurations.cFileName;

                if (AssetDatabaseHelper.CreateAsset(config, EditorConfigurations.cAssetPath))
                {
                    _configurations = AssetDatabase.LoadAssetAtPath<EditorConfigurations>(EditorConfigurations.cAssetPath);
                }
                else
                {
                    Log.Error($"Failed to create editor configurations asset.", nameof(EditorConfigurationsEditorWindow));
                    return;
                }
            }

            _serializedConfigurations = new SerializedObject(_configurations);
            _forcePlayScene = _serializedConfigurations.FindProperty(nameof(EditorConfigurations.ForcePlayScene));
            _forcePlayScenePath = _serializedConfigurations.FindProperty(nameof(EditorConfigurations.ForcePlayScenePath));
            _forcePlaySceneName = _serializedConfigurations.FindProperty(nameof(EditorConfigurations.ForcePlaySceneName));
            _clearLogs = _serializedConfigurations.FindProperty(nameof(EditorConfigurations.ClearLogs));

            if (!string.IsNullOrEmpty(_forcePlayScenePath.stringValue))
            {
                _sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(_forcePlayScenePath.stringValue);
                CheckForceScenePath();
            }
        }

        private void OnGUI()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Close();
                return;
            }

            _serializedConfigurations.Update();

            EditorGUILayout.Space(5);
            EditorGUILayoutHelper.DrawHorizontal(gui: () =>
            {
                EditorGUILayoutHelper.DrawVertical(gui: () =>
                {
                    EditorGUILayout.LabelField("Scene settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(_forcePlayScene);
                    if (_forcePlayScene.boolValue)
                    {
                        EditorGUILayoutHelper.DrawHorizontal(gui: () =>
                        {
                            GUILayout.Space(10);
                            EditorGUILayoutHelper.DrawVertical(gui: () =>
                            {
                                _sceneAsset = (SceneAsset) EditorGUILayout.ObjectField("Scene to Play: ", _sceneAsset, typeof(SceneAsset), false);
                                CheckForceScenePath(_forcePlayScenePath);

                                EditorGUILayout.PropertyField(_clearLogs);
                            });

                            EditorGUILayout.Space(2);
                        });
                    }
                });
            });

            _serializedConfigurations.ApplyModifiedProperties();
        }

        private void CheckForceScenePath(SerializedProperty forceScenePathProperty = null)
        {
            string scenePath = "";
            if (_sceneAsset != null)
            {
                scenePath = AssetDatabase.GetAssetPath(_sceneAsset.GetInstanceID());
                List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList();

                // Check if build settings scenes have the currently selected force scene
                if (!buildScenes.Any(x => x.path == scenePath))
                {
                    buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                    EditorBuildSettings.scenes = buildScenes.ToArray();
                    Log.Debug($"Force scene is set to \"{scenePath}\", automatically adding it to build settings. To change this, open the window at {EditorMenuItemConstants.cEditorConfigurations}.", nameof(EditorConfigurationsEditorWindow));

                    // Open the build settings window
                    GetWindow<BuildPlayerWindow>();
                }
            }

            if (forceScenePathProperty != null)
            {
                forceScenePathProperty.stringValue = scenePath;
                _forcePlaySceneName.stringValue = Path.GetFileNameWithoutExtension(scenePath);
            }
        }
    }
}
