using Codice.Client.BaseCommands;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Utils.Helpers
{
    /// <summary>
    /// Helper class for extending the <see cref="UnityEditor.AssetDatabase"/>
    /// </summary>
    public static class AssetDatabaseHelper
    {
        /// <summary>
        /// Create an asset at a specified path <br/>
        /// Automatically create directories to path if <paramref name="autoCreateDirectories"/> is true
        /// </summary>
        /// <param name="asset">Asset to create</param>
        /// <param name="path">Path to create asset at (include file extension)</param>
        /// <param name="autoCreateDirectories">Automatically create new directory (if necessary)</param>
        /// <param name="uniqueAssetPath">Create the asset at an unique path</param>
        /// <returns>Success?</returns>
        public static bool CreateAsset(UnityEngine.Object asset, string path, bool autoCreateDirectories = true, bool uniqueAssetPath = true)
        {
            if (!Directory.Exists(path))
            {
                if (autoCreateDirectories)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError($"[Cyggie.Main] directories to path does not exist: {path}");
                    return false;
                }
            }

            try
            {
                if (uniqueAssetPath)
                {
                    path = AssetDatabase.GenerateUniqueAssetPath(path);
                }

                AssetDatabase.CreateAsset(asset, path);
                Debug.Log($"[Cyggie.Main] Created asset {asset} at path: \"{path}\".");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Cyggie.Main] Unknown error occured, exception: {ex}.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Move an asset (file or folder) from <paramref name="oldAssetPath"/> to <paramref name="newAssetPath"/> <br/>
        /// Automatically create directories to path if <paramref name="autoCreateDirectories"/> is true
        /// </summary>
        /// <param name="oldAssetPath">Old asset path (must not end with "/" and include file extension)</param>
        /// <param name="newAssetPath">New asset path (must not end with "/" and include file extension)</param>
        /// <param name="autoCreateDirectories">Automatically create new directory (if necessary)?</param>
        /// <returns>Success?</returns>
        public static bool MoveAsset(string oldAssetPath, string newAssetPath, bool autoCreateDirectories = true)
        {
            if (oldAssetPath == newAssetPath)
            {
                Debug.LogError($"[Cyggie.Main] Old path is equal to new path.");
                return false;
            }

            if (!Directory.Exists(newAssetPath))
            {
                if (autoCreateDirectories)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newAssetPath));
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError($"[Cyggie.Main] Directories to path does not exist: {newAssetPath}");
                    return false;
                }
            }

            string errorMessage = AssetDatabase.MoveAsset(oldAssetPath, newAssetPath);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Debug.LogError($"[Cyggie.Main] Error: {errorMessage}");
                return false;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return true;
        }
    }
}
