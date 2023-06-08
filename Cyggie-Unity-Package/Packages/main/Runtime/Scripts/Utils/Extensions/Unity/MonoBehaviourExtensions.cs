using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cyggie.Main.Runtime.Utils.Extensions
{
    /// <summary>
    /// Extension class to <see cref="MonoBehaviour"/> objects
    /// </summary>
    public static class MonoBehaviourExtensions
    {
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
    }
}
