using Cyggie.Plugins.Utils.Constants;
using System.Text.RegularExpressions;

namespace Cyggie.Plugins.Utils.Extensions
{
    /// <summary>
    /// Extension class to <see cref="string"/>
    /// </summary>
    public static class StringExtensions
    {

        /// <summary>
        /// Insert <paramref name="startString"/> if <paramref name="str"/> doesn't start with it
        /// </summary>
        /// <param name="str">Target string</param>
        /// <param name="startString">Start string to check</param>
        /// <returns>String that starts with <paramref name="startString"/></returns>
        public static string InsertStartsWith(this string str, string startString)
            => InsertStartsWith(str, startString, startString);

        /// <summary>
        /// Insert <paramref name="startString"/> if <paramref name="str"/> doesn't start with <paramref name="stringToCheck"/>
        /// </summary>
        /// <param name="str">Target string</param>
        /// <param name="stringToCheck">String to check</param>
        /// <param name="startString">Start string to input</param>
        /// <returns>String that starts with <paramref name="startString"/></returns>
        public static string InsertStartsWith(this string str, string stringToCheck, string startString)
        {
            return str.StartsWith(stringToCheck) ?
                   str :
                   startString + str;
        }

        /// <summary>
        /// Insert <paramref name="endString"/> if <paramref name="str"/> doesn't end with <paramref name="endString"/>
        /// </summary>
        /// <param name="str">Target string</param>
        /// <param name="endString">End string to check</param>
        /// <returns>String that ends with <paramref name="endString"/></returns>
        public static string InsertEndsWith(this string str, string endString)
            => InsertEndsWith(str, endString, endString);

        /// <summary>
        /// Insert <paramref name="endString"/> if <paramref name="str"/> doesn't end with <paramref name="stringToCheck"/>
        /// </summary>
        /// <param name="str">Target string</param>
        /// <param name="stringToCheck">End string to check</param>
        /// <param name="endString">End string to input</param>
        /// <returns>String that ends with <paramref name="endString"/></returns>
        public static string InsertEndsWith(this string str, string stringToCheck, string endString)
        {
            return str.EndsWith(stringToCheck) ?
                   str :
                   str + endString;
        }

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
    }
}
