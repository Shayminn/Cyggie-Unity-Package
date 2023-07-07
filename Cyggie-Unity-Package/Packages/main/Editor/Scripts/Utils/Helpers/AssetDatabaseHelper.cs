using Cyggie.Plugins.Logs;
using System;
using System.IO;
using UnityEditor;

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
                    Log.Error($"Directories to path does not exist: {path}.", nameof(AssetDatabaseHelper));
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
                Log.Debug($"Created asset {asset} at path: \"{path}\".", nameof(AssetDatabaseHelper));

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Log.Error($"[Cyggie.Main] Unknown error occured, exception: {ex}.", nameof(AssetDatabaseHelper));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Delete an asset <br/>
        /// </summary>
        /// <param name="objectToDelete">Asset to delete</param>
        /// <returns></returns>
        public static bool DeleteAsset(UnityEngine.Object objectToDelete) => DeleteAsset(AssetDatabase.GetAssetPath(objectToDelete));

        /// <summary>
        /// Delete an asset at a specified path
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static bool DeleteAsset(string assetPath)
        {
            try
            {
                AssetDatabase.DeleteAsset(assetPath);
            }
            catch (Exception ex)
            {
                Log.Error($"Unknown error occured, exception: {ex}.", nameof(AssetDatabaseHelper));
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
                Log.Error($"Old path is equal to new path.", nameof(AssetDatabaseHelper));
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
                    Log.Error($"Directories to path does not exist: {newAssetPath}.", nameof(AssetDatabaseHelper));
                    return false;
                }
            }

            string errorMessage = AssetDatabase.MoveAsset(oldAssetPath, newAssetPath);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Log.Error($"Error: {errorMessage}.", nameof(AssetDatabaseHelper));
                return false;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return true;
        }
    }
}
