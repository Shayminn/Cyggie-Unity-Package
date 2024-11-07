using System;
using System.Collections;
using UnityEngine;

namespace Cyggie.Main.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper for managing Floats
    /// </summary>
    public static class FloatHelper
    {
        private static Coroutine _smoothingCoroutine = null;

        /// <summary>
        /// Smoothly transition a float value from <paramref name="currentValue"/> to <paramref name="targetValue"/>
        /// </summary>
        /// <param name="mono">Monobehaviour to call coroutine (make sure it is active)</param>
        /// <param name="currentValue">Current float value</param>
        /// <param name="targetValue">Target float value</param>
        /// <param name="speed">Transition speed</param>
        /// <param name="valueDifference">Estimated value to surpass in order to stop the transition (<paramref name="targetValue"/> - <paramref name="currentValue"/> > <paramref name="valueDifference"/>)</param>
        /// <param name="onValueChanged">Callback every time current value is changed</param>
        /// <param name="onTransitionCompleted">Callback when the transition is completed</param>
        public static void SmoothTransition(MonoBehaviour mono, float currentValue, float targetValue, float speed = 1, float valueDifference = 0.1f, Action<float> onValueChanged = null, Action onTransitionCompleted = null)
        {
            _smoothingCoroutine = mono.StartCoroutine(Smoothing(currentValue, targetValue, speed, valueDifference, onValueChanged, onTransitionCompleted));
        }

        /// <summary>
        /// Stop the last <see cref="SmoothTransition(MonoBehaviour, float, float, float, Action{float}, Action)"/> call if it's still ongoing
        /// </summary>
        /// <param name="mono">Monobehaviour that called the smoothing</param>
        public static void CancelSmoothTransition(MonoBehaviour mono)
        {
            if (_smoothingCoroutine != null)
            {
                mono.StopCoroutine(_smoothingCoroutine);
            }
        }

        private static IEnumerator Smoothing(float currentValue, float targetValue, float speed, float valueDifference, Action<float> onValueChanged, Action onTransitionCompleted)
        {
            int multiplier = targetValue > currentValue ? 1 : -1;

            while (Mathf.Abs(targetValue - currentValue) > valueDifference)
            {
                currentValue += multiplier * Time.deltaTime * speed;
                onValueChanged?.Invoke(currentValue);

                yield return null;
            }

            onValueChanged?.Invoke(targetValue);
            onTransitionCompleted?.Invoke();
            _smoothingCoroutine = null;
        }
    }
}
