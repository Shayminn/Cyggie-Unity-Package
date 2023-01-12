using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cyggie.Main.Runtime.Utils.Extensions
{
    /// <summary>
    /// Extension class to <see cref="String"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Add spaces to a Camelcase string. <br/>
        /// GTSTestTest => GTS Test Test
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SplitCamelCase(this string str)
        {
            // \P{Ll} = Any non lower-case letter (upper-case letter)
            // \p{Ll} = Any lower-case letter

            // First Replace checks for any Upper followed by an Upper and a Lower (i.e. GTSTest => S and Te), it'll then a space in between giving GTS Test
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
