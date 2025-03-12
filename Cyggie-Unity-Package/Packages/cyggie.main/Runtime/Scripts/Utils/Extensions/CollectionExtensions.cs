using Cyggie.Main.Runtime.Serializations;
using System;
using System.Collections.Generic;

namespace Cyggie.Main.Runtime.Utils.Extensions
{
    /// <summary>
    /// Extension class to Collections related
    /// </summary>
    public static class CollectionExtensions
    {
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
        public static TSource ToSerializedDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> iEnumerable, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector)
            where TSource : SerializedDictionary<TKey, TValue>, new()
        {
            TSource dict = new TSource();
            foreach (TSource source in iEnumerable)
            {
                dict.Add(keySelector.Invoke(source), elementSelector.Invoke(source));
            }

            return dict;
        }
    }
}
