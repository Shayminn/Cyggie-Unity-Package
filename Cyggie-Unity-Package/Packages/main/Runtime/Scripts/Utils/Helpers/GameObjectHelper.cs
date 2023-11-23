using System;
using UnityEngine;

namespace Cyggie.Main.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper class for <see cref="UnityEngine.Object"/>
    /// </summary>
    public static class GameObjectHelper
    {
        /// <summary>
        /// Prefab to an empty game object prefab
        /// </summary>
        internal static GameObject EmptyPrefab { get; set; }

        /// <summary>
        /// Instantiate an empty game object to the scene
        /// </summary>
        /// <param name="parent">Parent of the empty game object</param>
        /// <param name="worldPositionStays">Whether the world position should stay</param>
        /// <returns>Created empty game object</returns>
        public static GameObject InstantiateEmptyPrefab(Transform parent = null, bool worldPositionStays = false)
        {
            return UnityEngine.Object.Instantiate(EmptyPrefab, parent, worldPositionStays);
        }
    }
}
