using System.IO;

namespace Cyggie.Main.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper class for managing File related stuff
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Generate an unique file path adding "(index)" if it exists
        /// </summary>
        /// <param name="originalPath">Original file path</param>
        /// <param name="fileExtension">File extension to the generated path</param>
        /// <returns>Unique file path</returns>
        public static string GenerateUniquePath(string originalPath)
        {
            int index = 1;
            string uniqueDbPath = originalPath;
            string fileExtension = Path.GetExtension(originalPath);

            while (File.Exists(uniqueDbPath))
            {
                uniqueDbPath = $"{originalPath[..^fileExtension.Length]} ({index}){fileExtension}";
                ++index;
            }

            return uniqueDbPath;
        }
    }
}
