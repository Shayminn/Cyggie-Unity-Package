using System;
using System.Linq;
using UnityEngine;

namespace Cyggie.Main.Runtime.Utils.Extensions
{
    /// <summary>
    /// Extension class for <see cref="Enum"/>
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get the number of flag count on an enum with attribute <see cref="FlagsAttribute"/> <br/>
        /// This will send a debug error if the enum does not have a <see cref="FlagsAttribute"/>
        /// </summary>
        /// <param name="enum">Enum value to check</param>
        /// <returns>Flag count (0 if attribute is not found)</returns>
        public static int GetFlagsCount(this Enum @enum)
        {
            if (!@enum.GetType().GetCustomAttributes(typeof(FlagsAttribute), false).Any())
            {
                Debug.LogError($"Failed in {nameof(GetFlagsCount)}, type {@enum.GetType()} does not have the attribute {nameof(FlagsAttribute)}.");
                return 0;
            }

            int iCount = 0;
            long lValue = Convert.ToInt64(@enum);

            // Loop the value while there are still bits
            while (lValue != 0)
            {
                // Remove the end bit
                lValue &= (lValue - 1);

                // Increment the count
                iCount++;
            }

            // Return the count
            return iCount;
        }
    }
}
