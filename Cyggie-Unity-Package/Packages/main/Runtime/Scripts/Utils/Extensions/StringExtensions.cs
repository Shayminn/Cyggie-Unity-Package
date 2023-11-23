using Cyggie.Main.Runtime.Utils.Constants;

namespace Cyggie.Main.Runtime.Utils.Extensions
{
    /// <summary>
    /// Extension class to <see cref="string"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts the <paramref name="path"/> to a relative path from the resources folder <br/>
        /// i.e. Assets/Resources/Cyggie will return Cyggie
        /// </summary>
        /// <param name="path">Path to convert</param>
        /// <returns><paramref name="path"/> if it doesn't contain <see cref="FolderConstants.cResources"/></returns>
        public static string ToResourcesRelativePath(this string path)
        {
            if (!path.Contains(FolderConstants.cResources)) return path;

            return path[(path.IndexOf(FolderConstants.cResources) + FolderConstants.cResources.Length + 1)..];
        }
    }
}
