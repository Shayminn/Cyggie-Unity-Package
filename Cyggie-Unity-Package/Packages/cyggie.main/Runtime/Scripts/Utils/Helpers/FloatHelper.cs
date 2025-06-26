using Codice.CM.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyggie.Main.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper for managing Floats
    /// </summary>
    public static class FloatHelper
    {
        private static Dictionary<MonoBehaviour, Coroutine> _activeTransitions = new Dictionary<MonoBehaviour, Coroutine>();

        #region Lerp

        /// <summary>
        /// Lerp transition a float value from <paramref name="currentValue"/> to <paramref name="targetValue"/>
        /// </summary>
        /// <param name="mono">Monobehaviour to call coroutine (make sure it is active)</param>
        /// <param name="currentValue">Current float value</param>
        /// <param name="targetValue">Target float value</param>
        /// <param name="duration">Duration of the lerp</param>
        /// <param name="valueChanged">Callback every time current value is changed</param>
        /// <param name="completed">Callback when the transition is completed</param>
        public static void LerpTransition(MonoBehaviour mono, float currentValue, float targetValue, float duration, Action<float> valueChanged = null, Action completed = null)
        {
            if (_activeTransitions.TryGetValue(mono, out Coroutine coroutine) && coroutine != null)
            {
                mono.StopCoroutine(coroutine);
                _activeTransitions[mono] = mono.StartCoroutine(LerpCoroutine(currentValue, targetValue, duration, valueChanged, completed));
            }
            else
            {
                _activeTransitions.Add(mono, mono.StartCoroutine(LerpCoroutine(currentValue, targetValue, duration, valueChanged, completed)));
            }
        }

        public static IEnumerator LerpTransitionCoroutine(float currentValue, float targetValue, float duration, Action<float> valueChanged)
        {
            yield return LerpCoroutine(currentValue, targetValue, duration, valueChanged);
        }

        private static IEnumerator LerpCoroutine(float currentValue, float targetValue, float duration, Action<float> valueChanged, Action completed = null)
        {
            float initialValue = currentValue;
            float lerp = 0;
            while (currentValue != targetValue)
            {
                currentValue = Mathf.Lerp(initialValue, targetValue, lerp);
                lerp += Time.deltaTime / duration;

                valueChanged?.Invoke(currentValue);
                yield return null;
            }

            completed?.Invoke();
        }

        #endregion

        #region Smoothing

        /// <summary>
        /// Smoothly transition a float value from <paramref name="currentValue"/> to <paramref name="targetValue"/>
        /// </summary>
        /// <param name="mono">Monobehaviour to call coroutine (make sure it is active)</param>
        /// <param name="currentValue">Current float value</param>
        /// <param name="targetValue">Target float value</param>
        /// <param name="speed">Transition speed</param>
        /// <param name="valueDifference">Estimated value to surpass in order to stop the transition (<paramref name="targetValue"/> - <paramref name="currentValue"/> > <paramref name="valueDifference"/>)</param>
        /// <param name="valueChanged">Callback every time current value is changed</param>
        /// <param name="completed">Callback when the transition is completed</param>
        public static void SmoothTransition(MonoBehaviour mono, float currentValue, float targetValue, float speed = 1, float valueDifference = 0.1f, Action<float> valueChanged = null, Action completed = null)
        {
            if (_activeTransitions.TryGetValue(mono, out Coroutine coroutine) && coroutine != null)
            {
                mono.StopCoroutine(coroutine);
                _activeTransitions[mono] = mono.StartCoroutine(SmoothCoroutine(currentValue, targetValue, speed, valueDifference, valueChanged, completed));
            }
            else
            {
                _activeTransitions.Add(mono, mono.StartCoroutine(SmoothCoroutine(currentValue, targetValue, speed, valueDifference, valueChanged, completed)));
            }
        }

        /// <summary>
        /// Smoothly transition a float value from <paramref name="currentValue"/> to <paramref name="targetValue"/> coroutine
        /// </summary>
        /// <param name="currentValue">Current float value</param>
        /// <param name="targetValue">Target float value</param>
        /// <param name="speed">Transition speed</param>
        /// <param name="valueDifference">Estimated value to surpass in order to stop the transition (<paramref name="targetValue"/> - <paramref name="currentValue"/> > <paramref name="valueDifference"/>)</param>
        /// <param name="valueChanged">Callback every time current value is changed</param>
        /// <returns></returns>
        public static IEnumerator SmoothTransitionCoroutine(float currentValue, float targetValue, float speed, float valueDifference, Action<float> valueChanged)
        {
            yield return SmoothCoroutine(currentValue, targetValue, speed, valueDifference, valueChanged);
        }

        private static IEnumerator SmoothCoroutine(float currentValue, float targetValue, float speed, float valueDifference, Action<float> valueChanged, Action completed = null)
        {
            int multiplier = targetValue > currentValue ? 1 : -1;

            while (Mathf.Abs(targetValue - currentValue) > valueDifference)
            {
                currentValue += multiplier * Time.deltaTime * speed;
                valueChanged?.Invoke(currentValue);

                yield return null;
            }

            valueChanged?.Invoke(targetValue);
            completed?.Invoke();
        }

        #endregion

        /// <summary>
        /// Stop the last coroutine call if it's still ongoing
        /// </summary>
        /// <param name="mono">Monobehaviour that called the smoothing</param>
        public static void CancelTransition(MonoBehaviour mono)
        {
            if (_activeTransitions.TryGetValue(mono, out Coroutine coroutine))
            {
                mono.StopCoroutine(coroutine);
                _activeTransitions.Remove(mono);
            }
        }
    }
}
