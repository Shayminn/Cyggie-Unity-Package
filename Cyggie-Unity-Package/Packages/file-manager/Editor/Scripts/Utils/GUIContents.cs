using UnityEngine;

namespace Cyggie.FileManager.Editor.Utils.Styles
{
    /// <summary>
    /// Styles for all EditorGUI drawings
    /// </summary>
    internal class GUIContents
    {
        // 
        //  File Manager Tab
        //
        internal static readonly GUIContent cUsePersistentDataPath = new GUIContent("Use Persistent Data Path", "\"Whether to use Unity's persistent data path (\"C:\\Users\\[User]\\AppData\\LocalLow\\[CompanyName]\\[GameName]\") or a local path");
        internal static readonly GUIContent cLocalSavePath = new GUIContent("Local Save Path", "The local save path to use (when UsePersistentDataPath is not enabled)");
        internal static readonly GUIContent cDefaultFileExtension = new GUIContent("Default File Extension", "The default file extension to use for all saved files");
        internal static readonly GUIContent cEncrypted = new GUIContent("Encrypted", "Whether saved files are encrypted");
        internal static readonly GUIContent cFilesToIgnore = new GUIContent("Files to Ignore", "List of files to ignore from being read in the folder path");
    }
}
