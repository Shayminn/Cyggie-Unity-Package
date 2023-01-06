using System.Collections.Generic;
using System.Linq;

namespace Cyggie.Main.Runtime.Utils.Extensions
{
    /// <summary>
    /// Extension class to Collections related
    /// </summary>
    public static class CollectionExtensions
    {
        private static System.Random _rand = new System.Random();

        /// <summary>
        /// Shuffle the collection <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">IEnumerable to shuffle</param>
        /// <returns>Randomly shuffled IEnumerable</returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            return collection.OrderBy(x => _rand.Next());
        }
    }
}
