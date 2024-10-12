using Cyggie.Main.Editor.Utils.Constants;
using Cyggie.Plugins.Logs;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// Class that starts editor play mode in a specific scene
    /// </summary>
    public class StartingSceneOnPlay
    {
        private static string _sceneBeforePlay = string.Empty;
        private static string _sceneToPlayPath = string.Empty;

        /// <summary>
        /// Called when editor enters play mode
        /// </summary>
        [InitializeOnEnterPlayMode]
        public static void OnEnterPlayMode()
        {
            // Find editor configurations to apply
            EditorConfigurations configurations = AssetDatabase.LoadAssetAtPath<EditorConfigurations>(EditorConfigurations.cAssetPath);
            if (configurations == null || !configurations.ForcePlayScene) return;

            // Check if force scene to play is valid
            _sceneToPlayPath = configurations.ForcePlayScenePath;
            if (string.IsNullOrEmpty(_sceneToPlayPath))
            {
                Log.Error($"Trying to force a starting scene on editor play, but the scene path is not specified. Please specify it in the window at {EditorMenuItemConstants.cEditorConfigurations}.", nameof(StartingSceneOnPlay));
                return;
            }

            // Check if current scene is not already the scene to play
            if (SceneManager.GetActiveScene().name == _sceneToPlayPath) return;

            // Check if build settings scenes have the currently selected force scene
            List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList();
            if (!buildScenes.Any(x => x.path == _sceneToPlayPath))
            {
                // Check if scene asset exists at path
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(_sceneToPlayPath);
                if (sceneAsset == null)
                {
                    configurations.ForcePlayScenePath = string.Empty;
                    Log.Error($"Force scene path \"{_sceneToPlayPath}\" no longer exists. Please assign a new scene in the window at {EditorMenuItemConstants.cEditorConfigurations}.", nameof(StartingSceneOnPlay));
                    return;
                }

                buildScenes.Add(new EditorBuildSettingsScene(_sceneToPlayPath, true));
                EditorBuildSettings.scenes = buildScenes.ToArray();
                Log.Debug($"Force scene is set to \"{_sceneToPlayPath}\", automatically adding it to build settings. To change this, open the window at {EditorMenuItemConstants.cEditorConfigurations}.", nameof(StartingSceneOnPlay));
                Log.Debug($"Play mode stopped forcefully, now that the force scene is in the build settings, you can click on play again.", nameof(StartingSceneOnPlay));
                EditorApplication.isPlaying = false;
                return;
            }

            EditorApplication.playModeStateChanged += StateChange;
        }

        private static void StateChange(PlayModeStateChange state)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.playModeStateChanged -= StateChange;

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    // We're in playmode, just about to change playmode to Editor
                    EditorSceneManager.OpenScene(_sceneBeforePlay);
                }
                else
                {
                    // We're in playmode, right after having pressed Play
                    _sceneBeforePlay = SceneManager.GetActiveScene().name;

                    ClearLog();
                    SceneManager.LoadScene(_sceneToPlayPath);
                }
            }
        }

        /// <summary>
        /// Clears the Console Log
        /// </summary>
        private static void ClearLog()
        {
            var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            var method = logEntries.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
    }
}
