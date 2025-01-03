﻿using Cyggie.Main.Runtime.Serializations;
using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cyggie.Main.Runtime.Utils.Extensions
{
    /// <summary>
    /// Extension class to <see cref="MonoBehaviour"/> objects
    /// </summary>
    public static class MonoBehaviourExtensions
    {
        private const float cDefaultDelay = 0.01f;

        #region Extension methods

        /// <summary>
        /// Starts an <paramref name="action"/> after a <paramref name="delay"/> (in seconds)
        /// </summary>
        /// <param name="mono">Monobehaviour that triggers the action</param>
        /// <param name="action">Action to invoke after delay</param>
        /// <param name="delay">Delay before action is invoked (in seconds)</param>
        public static void DelayedAction(this MonoBehaviour mono, UnityAction action, float delay = cDefaultDelay)
        {
            mono.StartCoroutine(DelayedCoroutine(action, delay));
        }

        /// <summary>
        /// Starta an <paramref name="action"/> after <paramref name="condition"/> evaluates to false <br/>
        /// Set an optional max time to timeout and automatically proceed into the <paramref name="enumerator"/>
        /// </summary>
        /// <param name="mono">MonoBehaviour object to start the coroutine</param>
        /// <param name="condition">Condition to satisfy</param>
        /// <param name="action">Action to start</param>
        /// <param name="maxTime">Max time before timeout (defaults to -1, any negative value = infinite)</param>"
        public static void ConditionalAction(this MonoBehaviour mono, Predicate condition, UnityAction action, float maxTime = -1)
        {
            mono.StartCoroutine(ConditionalCoroutine(condition, action, maxTime));
        }

        /// <summary>
        /// Verify the Inspector Reference of one or many <paramref name="objs"/> <br/>
        /// </summary>
        /// <param name="mono">Mono object that holds the reference</param>
        /// <param name="objs">Array of objects to check</param>
        /// <returns>True if any <paramref name="objs"/> has a null reference</returns>
        public static bool HasMissingReference(this MonoBehaviour mono, params UnityEngine.Object[] objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] == null)
                {
                    Log.Error($"Object ({i}) in {mono.name} has a missing reference, assign it in the Inspector.", nameof(MonoBehaviourExtensions), mono);
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
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] == null)
                {
                    Log.Error($"Object {i} in {mono.name} has a missing reference, assign it in the Inspector.", nameof(MonoBehaviourExtensions), mono);
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
                if (kv.Value is UnityEngine.Object unityObj && mono.HasMissingReference(unityObj)) return true;
                else if (mono.HasMissingReference(kv.Value)) return true;
            }

            return false;
        }

        #endregion

        #region Coroutine extension methods

        /// <summary>
        /// Safely (re)starts a coroutine by first stopping it if it's not null before starting it.
        /// </summary>
        /// <param name="mono">MonoBehaviour object to start the coroutine on</param>
        /// <param name="coroutine">Coroutine object to reference</param>
        /// <param name="enumerator">Enumerator to run</param>
        public static void SafeStartCoroutine(this MonoBehaviour mono, ref Coroutine coroutine, IEnumerator enumerator)
        {
            if (coroutine != null)
            {
                mono.StopCoroutine(coroutine);
            }

            coroutine = mono.StartCoroutine(enumerator);
        }

        /// <summary>
        /// Safely stops a coroutine by checking if it's not null.
        /// </summary>
        /// <param name="mono">MonoBehaviour object to stop the coroutine on</param>
        /// <param name="coroutine">Coroutine object to stop</param>
        public static void SafeStopCoroutine(this MonoBehaviour mono, ref Coroutine coroutine)
        {
            if (coroutine == null) return;

            mono.StopCoroutine(coroutine);
        }

        /// <summary>
        /// Start a coroutine with a delay of <paramref name="delay"/> <br/>
        /// This will automatically wait for <paramref name="delay"/> (seconds) before starting the <paramref name="enumerator"/>
        /// </summary>
        /// <param name="mono">MonoBehaviour object to start the coroutine</param>
        /// <param name="delay">Delay before starting the enumerator</param>
        /// <param name="enumerator">Enumerator method to start</param>
        /// <returns>Coroutine from starting the enumerator</returns>
        public static Coroutine StartDelayedCoroutine(this MonoBehaviour mono, IEnumerator enumerator, float delay)
        {
            return mono.StartCoroutine(DelayedCoroutine(enumerator, delay));
        }

        /// <summary>
        /// Safely start a coroutine with a delay of <paramref name="delay"/> <br/>
        /// This will automatically stop the coroutine if it's not null and automatically wait for <paramref name="delay"/> (seconds) before starting the <paramref name="enumerator"/>
        /// </summary>
        /// <param name="mono">MonoBehaviour object to start the coroutine</param>
        /// <param name="coroutine">Referenced coroutine to stop</param>
        /// <param name="delay">Delay before starting the enumerator</param>
        /// <param name="enumerator">Enumerator method to start</param>
        public static void SafeStartDelayedCoroutine(this MonoBehaviour mono, ref Coroutine coroutine, IEnumerator enumerator, float delay)
        {
            if (coroutine != null)
            {
                mono.StopCoroutine(coroutine);
            }

            coroutine = mono.StartCoroutine(DelayedCoroutine(enumerator, delay));
        }

        /// <summary>
        /// Start a coroutine when a specified condition is satisfied <br/>
        /// Set an optional max time to timeout and automatically proceed into the <paramref name="enumerator"/>
        /// </summary>
        /// <param name="mono">MonoBehaviour object to start the coroutine</param>
        /// <param name="condition">Condition to satisfy</param>
        /// <param name="enumerator">Enumerator method to start</param>
        /// <param name="maxTime">Max time before timeout (defaults to -1, any negative value = infinite)</param>"
        /// <returns>Coroutine from starting the enumerator</returns>
        public static Coroutine StartConditionalCoroutine(this MonoBehaviour mono, Predicate condition, IEnumerator enumerator, float maxTime = -1)
        {
            return mono.StartCoroutine(ConditionalCoroutine(condition, enumerator, maxTime));
        }

        /// <summary>
        /// Safely start a coroutine when a specified condition is satisfied <br/>
        /// Set an optional max time to timeout and automatically proceed into the <paramref name="enumerator"/>
        /// </summary>
        /// <param name="mono">MonoBehaviour object to start the coroutine</param>
        /// <param name="coroutine">Referenced coroutine to stop</param>
        /// <param name="condition">Condition to satisfy</param>
        /// <param name="enumerator">Enumerator method to start</param>
        /// <param name="maxTime">Max time before timeout (defaults to -1, any negative value = infinite)</param>"
        public static void SafeStartConditionalCoroutine(this MonoBehaviour mono, ref Coroutine coroutine, Predicate condition, IEnumerator enumerator, float maxTime = -1)
        {
            if (coroutine != null)
            {
                mono.StopCoroutine(coroutine);
            }

            coroutine = mono.StartCoroutine(ConditionalCoroutine(condition, enumerator, maxTime));
        }

        #endregion

        #region Private coroutines

        private static IEnumerator DelayedCoroutine(IEnumerator enumerator, float delay)
        {
            yield return new WaitForSeconds(delay);
            yield return enumerator;
        }

        private static IEnumerator DelayedCoroutine(UnityAction action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }

        private static IEnumerator ConditionalCoroutine(Predicate condition, IEnumerator enumerator, float maxTime)
        {
            while (condition.Invoke())
            {
                if (maxTime > 0)
                {
                    maxTime -= Time.deltaTime;
                    if (maxTime < 0) break;
                }

                yield return null;
            }

            yield return enumerator;
        }

        private static IEnumerator ConditionalCoroutine(Predicate condition, UnityAction action, float maxTime)
        {
            while (condition.Invoke())
            {
                if (maxTime > 0)
                {
                    maxTime -= Time.deltaTime;
                    if (maxTime < 0) break;
                }

                yield return null;
            }

            action?.Invoke();
        }

        #endregion
    }
}
