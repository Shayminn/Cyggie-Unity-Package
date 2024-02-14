using Cyggie.Main.Runtime.Serializations;
using Cyggie.Plugins.Logs;
using System.Collections.Generic;
using UnityEngine;

namespace Cyggie.Main.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper class for serialization
    /// </summary>
    public static class MonoBehaviourExtensions
    {
        /// <summary>
        /// Verify the Inspector Reference of one or many <paramref name="objs"/> <br/>
        /// </summary>
        /// <param name="mono">Mono object that holds the reference</param>
        /// <param name="objs">Array of objects to check</param>
        /// <returns>True if any <paramref name="objs"/> has a null reference</returns>
        public static bool HasMissingReference(this MonoBehaviour mono, params UnityEngine.Object[] objs)
        {
            foreach (UnityEngine.Object obj in objs)
            {
                if (obj == null)
                {
                    Log.Error($"Object {obj.name} in {mono.name} has a missing reference, assign it in the Inspector.", nameof(MonoBehaviourExtensions), mono);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Verify the Inspector Reference of one or many <paramref name="objs"/> <br/>
        /// </summary>
        /// <param name="mono">Mono object that holds the reference</param>
        /// <param name="objs">Array of objects to check</param>
        /// <returns>True if any <paramref name="objs"/> has a null reference</returns>
        public static bool HasMissingReference(this MonoBehaviour mono, params object[] objs)
        {
            foreach (object obj in objs)
            {
                if (obj == null)
                {
                    Log.Error($"Object in {mono.name} has a missing reference, assign it in the Inspector.", nameof(MonoBehaviourExtensions), mono);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Verify the Inspector References of all values in a <see cref="SerializedDictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="mono">Mono object that holds the reference</param>
        /// <param name="dict">Dictionary's values to verify</param>
        /// <returns>True if any of the <paramref name="dict"/>'s value has a null reference</returns>
        public static bool HasMissingReference<TKey, TValue>(this MonoBehaviour mono, SerializedDictionary<TKey, TValue> dict)
        {
            foreach (KeyValuePair<TKey, TValue> kv in dict)
            {
                if (mono.HasMissingReference(kv.Value)) return true;
            }

            return false;
        }
    }
}
