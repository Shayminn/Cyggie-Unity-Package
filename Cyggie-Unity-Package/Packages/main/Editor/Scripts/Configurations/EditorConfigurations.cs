using Cyggie.Main.Editor.Utils.Constants;
using Cyggie.Plugins.Utils.Constants;
using UnityEngine;

namespace Cyggie.Main.Editor
{
    /// <summary>
    /// Scriptable object that contains editor related configurations
    /// </summary>
    [CreateAssetMenu(menuName = "Cyggie/Test")]
    internal class EditorConfigurations : ScriptableObject
    {
        public const string cFileName = "EditorConfigurations";
        public const string cAssetPath = Runtime.Utils.Constants.FolderConstants.cAssets + FolderConstants.cCyggieEditor + cFileName + FileExtensionConstants.cAsset;

        [Tooltip("Whether the editor should force play a specific scene every time play mode is started, returning to the original scene when play is stopped.")]
        public bool ForcePlayScene = false;

        [Tooltip("The path to the scene the editor should force start on.")]
        public string ForcePlayScenePath = string.Empty;

        [Tooltip("The scene name the editor should force start on.")]
        public string ForcePlaySceneName = string.Empty;

        [Tooltip("Whether the logs should be cleared before forcing another scene.")]
        public bool ClearLogs = false;
    }
}
