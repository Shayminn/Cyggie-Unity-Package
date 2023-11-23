using Cyggie.Plugins.Logs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cyggie.Plugins.Utils.Extensions
{
    /// <summary>
    /// Extension class to Collections related
    /// </summary>
    public static class CollectionExtensions
    {
        private readonly static Random _rand = new Random();

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

        #region Move methods

        /// <summary>
        /// Move the element T <paramref name="fromIndex"/> to <paramref name="toIndex"/> affecting all the other elements' indexes
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="fromIndex">Index to move from</param>
        /// <param name="toIndex">Index to move to</param>
        /// <returns>Collection with moved elements (<paramref name="collection"/> if invalid)</returns>
        public static IEnumerable<T> Move<T>(this IEnumerable<T> collection, uint fromIndex, uint toIndex)
        {
            if (fromIndex == toIndex)
            {
                Log.Debug($"Both indexes are the same (From: {fromIndex}, To: {toIndex}).", nameof(CollectionExtensions));
                return collection;
            }

            List<T> list = collection.ToList();

            if (fromIndex >= list.Count || toIndex >= list.Count)
            {
                Log.Debug($"Index out of range (From: {fromIndex}, To: {toIndex}).", nameof(CollectionExtensions));
                return collection;
            }

            T element = list[(int) fromIndex];
            list.RemoveAt((int) fromIndex);
            list.Insert((int) toIndex, element);

            return list;
        }

        /// <summary>
        /// Move the <paramref name="element"/> to <paramref name="toIndex"/> affecting all the other elements' indexes
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="element">Target element in the collection</param>
        /// <param name="toIndex">Index to move to</param>
        /// <returns>Collection with moved elements (<paramref name="collection"/> if invalid)</returns>
        public static IEnumerable<T> Move<T>(this IEnumerable<T> collection, T element, uint toIndex)
        {
            if (element == null)
            {
                Log.Error($"Element is null.", nameof(CollectionExtensions));
                return collection;
            }

            List<T> list = collection.ToList();

            int elementIndex = list.IndexOf(element);
            if (elementIndex <= -1)
            {
                Log.Error($"Element {element} not found in {collection} (Count: {collection.Count()}).", nameof(CollectionExtensions));
                return collection;
            }

            if (elementIndex >= list.Count || toIndex >= list.Count)
            {
                Log.Error($"Index out of range (From: {elementIndex}, To: {toIndex}).", nameof(CollectionExtensions));
                return collection;
            }

            list.RemoveAt(elementIndex);
            list.Insert((int) toIndex, element);

            return list;
        }

        /// <summary>
        /// Move the <paramref name="element"/> up by 1 affecting all the other elements' indexes
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="element">Target element in the collection</param>
        /// <returns>Collection with moved elements (<paramref name="collection"/> if invalid)</returns>
        public static IEnumerable<T> MoveUp<T>(this IEnumerable<T> collection, T element)
        {
            int index = collection.IndexOf(element);
            if (index <= -1)
            {
                Log.Error($"Element {element} not found in {collection} (Count: {collection.Count()}).", nameof(CollectionExtensions));
                return collection;
            }

            return collection.Move((uint) index, (uint) index + 1);
        }

        /// <summary>
        /// Move the element at <paramref name="index"/> up by 1 affecting all the other elements' indexes
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="index">Index of element to move</param>
        /// <returns>Collection with moved elements (<paramref name="collection"/> if invalid)</returns>
        public static IEnumerable<T> MoveUp<T>(this IEnumerable<T> collection, uint index)
        {
            return collection.Move(index, index + 1);
        }

        /// <summary>
        /// Move the <paramref name="element"/> down by 1 affecting all the other elements' indexes
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="element">Target element in the collection</param>
        /// <returns>Collection with moved elements (<paramref name="collection"/> if invalid)</returns>
        public static IEnumerable<T> MoveDown<T>(this IEnumerable<T> collection, T element)
        {
            int index = collection.IndexOf(element);
            if (index <= -1)
            {
                Log.Error($"Element {element} not found in {collection} (Count: {collection.Count()}).", nameof(CollectionExtensions));
                return collection;
            }

            return collection.Move((uint) index, (uint) index - 1);
        }

        /// <summary>
        /// Move the element at <paramref name="index"/> down by 1 affecting all the other elements' indexes
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="index">Index of element to move</param>
        /// <returns>Collection with moved elements (<paramref name="collection"/> if invalid)</returns>
        public static IEnumerable<T> MoveDown<T>(this IEnumerable<T> collection, uint index)
        {
            return collection.Move(index, index - 1);
        }

        /// <summary>
        /// Move <paramref name="element"/> to the first element of <paramref name="collection"/> affecting all other elements' indexes
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="element">Element to move</param>
        /// <returns>Collection with moved elements (<paramref name="collection"/> if invalid)</returns>
        public static IEnumerable<T> MoveFirst<T>(this IEnumerable<T> collection, T element)
        {
            return collection.Move(element, 0);
        }

        /// <summary>
        /// Move the element at <paramref name="index"/> to the first element of <paramref name="collection"/> affecting all other elements' indexes
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="index">Element index to move</param>
        /// <returns>Collection with moved elements (<paramref name="collection"/> if invalid)</returns>
        public static IEnumerable<T> MoveFirst<T>(this IEnumerable<T> collection, uint index)
        {
            return collection.Move(index, 0);
        }

        /// <summary>
        /// Move <paramref name="element"/> to the last element of <paramref name="collection"/> affecting all other elements' indexes
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="element">Element to move</param>
        /// <returns>Collection with moved elements (<paramref name="collection"/> if invalid)</returns>
        public static IEnumerable<T> MoveLast<T>(this IEnumerable<T> collection, T element)
        {
            return collection.Move(element, (uint) collection.Count() - 1);
        }

        /// <summary>
        /// Move the element at <paramref name="index"/> to the last element of <paramref name="collection"/> affecting all other elements' indexes
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="index">Element index to move</param>
        /// <returns>Collection with moved elements (<paramref name="collection"/> if invalid)</returns>
        public static IEnumerable<T> MoveLast<T>(this IEnumerable<T> collection, uint index)
        {
            return collection.Move(index, (uint) collection.Count() - 1);
        }

        #endregion

        #region Swap methods

        /// <summary>
        /// Swap the position of <paramref name="fromElement"/> and <paramref name="toElement"/> unaffecting the other elements' indexes
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="fromElement">Element to swap from</param>
        /// <param name="toElement">Element to swap with</param>
        /// <returns>Collection with swapped elements (<paramref name="collection"/> if invalid)</returns>
        public static IEnumerable<T> Swap<T>(this IEnumerable<T> collection, T fromElement, T toElement)
        {
            int fromIndex = collection.IndexOf(fromElement);
            if (fromIndex <= -1)
            {
                Log.Error($"From Element {fromElement} not found in {collection} (Count: {collection.Count()}).", nameof(CollectionExtensions));
                return collection;
            }

            int toIndex = collection.IndexOf(toElement);
            if (toIndex <= -1)
            {
                Log.Error($"To Element {toElement} not found in {collection} (Count: {collection.Count()}).", nameof(CollectionExtensions));
                return collection;
            }

            return collection.Swap((uint) fromIndex, (uint) toIndex);
        }

        /// <summary>
        /// Swap the position of <paramref name="element"/> and the element at <paramref name="toIndex"/> unaffecting the other elements' indexes
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="element">Element to swap from</param>
        /// <param name="toIndex">Index to swap with</param>
        /// <returns>Collection with swapped elements (<paramref name="collection"/> if invalid)</returns>
        public static IEnumerable<T> Swap<T>(this IEnumerable<T> collection, T element, uint toIndex)
        {
            int fromIndex = collection.IndexOf(element);

            if (fromIndex <= -1)
            {
                Log.Error($"Element {element} not found in {collection} (Count: {collection.Count()}).", nameof(CollectionExtensions));
                return collection;
            }

            return collection.Swap((uint) fromIndex, toIndex);
        }

        /// <summary>
        /// Swap the position with elements at indexes <paramref name="fromIndex"/> and <paramref name="toIndex"/> unaffecting the other elements indexes
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="fromIndex">Index to swap from</param>
        /// <param name="toIndex">Index to swap with</param>
        /// <returns>Collection with swapped elements (<paramref name="collection"/> if invalid)</returns>
        public static IEnumerable<T> Swap<T>(this IEnumerable<T> collection, uint fromIndex, uint toIndex)
        {
            if (fromIndex == toIndex) return collection;

            int count = collection.Count();
            if (fromIndex > count || toIndex > count) return collection;

            T[] array = collection.ToArray();
            (array[toIndex], array[fromIndex]) = (array[fromIndex], array[toIndex]);

            return array;
        }

        #endregion

        /// <summary>
        /// Get the index of <paramref name="element"/> in <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="element">Element to find the index of</param>
        /// <returns>Index of element in collection (-1 if not found)</returns>
        public static int IndexOf<T>(this IEnumerable<T> collection, T element)
        {
            return Array.IndexOf(collection.ToArray(), element);
        }
    }
}
