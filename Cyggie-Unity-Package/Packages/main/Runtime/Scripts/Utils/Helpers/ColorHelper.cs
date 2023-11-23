using Cyggie.Main.Runtime.Utils.Extensions;
using System;
using System.Collections;
using UnityEngine;

namespace Cyggie.Main.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper class for managing <see cref="UnityEngine.Color"/>
    /// </summary>
    public static class ColorHelper
    {
        /// <summary>
        /// Smoothly transition to alpha color of <paramref name="color"/>
        /// </summary>
        /// <param name="mono">MonoBehaviour object to start the coroutine</param>
        /// <param name="color">Color to change as reference</param>
        /// <param name="targetAlpha">Target alpha value to transition to</param>
        /// <param name="transitionDuration">The duration of the transition from current color's alpha to target alpha</param>
        /// <param name="onColorUpdated">Action called on every tick of change in alpha value</param>
        /// <param name="onTransitionCompleted">Action called when the transition is complete</param>
        public static void TransitionColorAlpha(MonoBehaviour mono, Color color, float targetAlpha, float transitionDuration = 1f, Action<Color> onColorUpdated = null, Action onTransitionCompleted = null)
        {
            // Start coroutine to transition 
            mono.StartCoroutine(TransitionColorAlpha(color, targetAlpha, transitionDuration, onColorUpdated, onTransitionCompleted));
        }

        /// <summary>
        /// Coroutine to change alpha color on every tick for <see cref="TransitionColorAlpha(MonoBehaviour, Color, float, float, Action{Color}, Action)"/>
        /// </summary>
        /// <param name="color">Color struct</param>
        /// <param name="targetAlpha">Target alpha value to transition to</param>
        /// <param name="transitionDuration">The duration of the transition from current color's alpha to target alpha</param>
        /// <param name="onColorUpdated">Action called when the color's alpha is updated</param>
        /// <param name="onTransitionCompleted">Action called when the transition is complete</param>
        /// <returns>Coroutine enumerator</returns>
        private static IEnumerator TransitionColorAlpha(Color color, float targetAlpha, float transitionDuration = 1f, Action<Color> onColorUpdated = null, Action onTransitionCompleted = null)
        {
            float currentAlpha = color.a;
            bool increment = targetAlpha > currentAlpha;

            float speed = 1 / transitionDuration; // transition speed
            int multiplier = increment ? 1 : -1;

            while (currentAlpha != targetAlpha)
            {
                // Increment/Decrement the current alpha
                currentAlpha += multiplier * speed * Time.deltaTime;

                // Lerp min/max value
                currentAlpha = increment ?
                               Mathf.Clamp(currentAlpha, currentAlpha, targetAlpha) :
                               Mathf.Clamp(currentAlpha, targetAlpha, currentAlpha);

                // Change color's alpha
                color = color.ChangeAlpha(currentAlpha);

                onColorUpdated?.Invoke(color);

                yield return null;
            }

            onTransitionCompleted?.Invoke();
        }
    }
}
