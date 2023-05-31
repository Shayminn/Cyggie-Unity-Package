using System.IO;
using UnityEngine;

namespace Cyggie.Main.Editor.Utils.Helpers
{
    /// <summary>
    /// Helper class for File management
    /// </summary>
    public static class FileHelper
    {
        private static readonly string cAssetsPath = "Assets/";

        /// <summary>
        /// Get the relative file path to <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">File name to look up</param>
        /// <returns>Relative path</returns>
        public static bool TryGetRelativePath(string fileName, out string path, bool suppressError = false)
        {
            path = string.Empty;

            // Get the absolute path
            if (!TryGetAbsolutePath(fileName, out string absolutePath, suppressError)) return false;

            // Find the start index of the relative path
            int index = absolutePath.IndexOf(cAssetsPath);

            // Return the relative path based on the absolute path
            path = absolutePath[index..];

            return true;
        }

        /// <summary>
        /// Get the absolute file path to <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">File name to look up</param>
        /// <returns>Absolute path</returns>
        public static bool TryGetAbsolutePath(string fileName, out string path, bool suppressError = false)
        {
            // Get all existing paths to file name
            string[] paths = System.IO.Directory.GetFiles(Application.dataPath, fileName, SearchOption.AllDirectories);
            path = string.Empty;
            if (paths.Length == 0)
            {
                if (!suppressError)
                {
                    Debug.LogError($"[Cyggie.Main] File {fileName} not found in the project.");
                }

                return false;
            }

            // Return the absolute path
            path = paths[0].Replace("\\", "/");

            return true;
        }
    }
}
