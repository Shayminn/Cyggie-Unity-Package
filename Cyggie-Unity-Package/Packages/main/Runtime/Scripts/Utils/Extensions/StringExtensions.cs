using System;
using System.Text.RegularExpressions;

namespace Cyggie.Main.Runtime.Utils.Extensions
{
    /// <summary>
    /// Extension class to <see cref="String"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Add spaces to a Camelcase string. <br/>
        /// ABCTestTest => ABC Test Test
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SplitCamelCase(this string str)
        {
            // \P{Ll} = Any non lower-case letter (upper-case letter)
            // \p{Ll} = Any lower-case letter

            // First Replace checks for any Upper followed by an Upper and a Lower (i.e. ABCTest => C and Te), it'll then add a space in between giving ABC Test
            // Second Replace checks for any Lower followed by an Upper (i.e. TestTest => t and T), it'll then add a space in between giving Test Test

            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

        /// <summary>
        /// Converts the <paramref name="path"/> to a relative path from the resources folder <br/>
        /// i.e. Assets/Resources/Cyggie will return Cyggie
        /// </summary>
        /// <param name="path">Path to convert</param>
        /// <returns><paramref name="path"/> if it doesn't contain <see cref="StringConstants.cResources"/></returns>
        public static string ToResourcesRelativePath(this string path)
        {
            if (!path.Contains(StringConstants.cResources)) return path;

            return path[(path.IndexOf(StringConstants.cResources) + StringConstants.cResources.Length + 1)..];
        }
    }
}
