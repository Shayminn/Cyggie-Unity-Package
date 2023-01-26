using Cyggie.Main.Runtime.Serializations;
using System;
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

        /// <summary>
        /// Converts an IEnumerable to a <see cref="SerializedDictionary{TKey, TValue}"/>
        /// </summary>
        /// <typeparam name="TSource">IEnumerable T source</typeparam>
        /// <typeparam name="TKey">Dictionary's Key type</typeparam>
        /// <typeparam name="TValue">Dictionary's Value type</typeparam>
        /// <param name="iEnumerable">Target IEnumerable</param>
        /// <param name="keySelector">Function selector for keys</param>
        /// <param name="elementSelector">Function selector for values</param>
        /// <returns>Serialized Dictionary of TKey and TElement</returns>
        public static SerializedDictionary<TKey, TValue> ToSerializedDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> iEnumerable, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector)
        {
            SerializedDictionary<TKey, TValue> dict = new SerializedDictionary<TKey, TValue>();
            foreach (TSource source in iEnumerable)
            {
                dict.Add(keySelector.Invoke(source), elementSelector.Invoke(source));
            }

            return dict;
        }
    }
}
