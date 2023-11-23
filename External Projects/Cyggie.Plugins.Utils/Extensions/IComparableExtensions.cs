using System;

namespace Cyggie.Plugins.Utils.Extensions
{
    /// <summary>
    /// Extension class for <see cref="IComparable"/> objects
    /// </summary>
    public static class IComparableExtensions
    {
        /// <summary>
        /// Checks if <paramref name="value"/> is between <paramref name="minInclusive"/> and <paramref name="maxInclusive"/>, both inclusively
        /// </summary>
        /// <typeparam name="T">Comparable Type</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="minInclusive">Min value inclusive</param>
        /// <param name="maxInclusive">Max value inclusive</param>
        /// <returns></returns>
        public static bool IsBetween<T>(this T value, T minInclusive, T maxInclusive) where T : IComparable<T>
            => value.CompareTo(minInclusive) >= 0 && value.CompareTo(maxInclusive) <= 0;
    }
}
