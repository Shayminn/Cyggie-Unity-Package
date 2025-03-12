using System;
using UnityEngine;

namespace Cyggie.Main.Runtime.Attributes
{
    /// <summary>
    /// Custom attribute to <see cref="RuntimeInitializeOnLoadMethodAttribute"/> <br/>
    /// This is a workaround to <see cref="RuntimeInitializeOnLoadMethodAttribute"/> imitating its behaviour <br/>
    /// This does not have the limitation of the class implementing only MonoBehaviour
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CustomRuntimeInitializeOnLoadMethodAttribute : PropertyAttribute
    {
        public RuntimeInitializeLoadType LoadType { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="loadType">Load type timing ()</param>
        public CustomRuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType loadType = RuntimeInitializeLoadType.AfterSceneLoad)
        {
            LoadType = loadType;
        }
    }
}
