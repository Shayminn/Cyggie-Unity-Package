using System;
using UnityEngine;

namespace Cyggie.Main.Runtime.Services
{
    /// <summary>
    /// Configuration class to apply to a <see cref="Service"/>
    /// </summary>
    [Serializable]
    public abstract class ServiceConfiguration : ScriptableObject
    {
        /// <summary>
        /// Type that implements <see cref="Service"/>
        /// </summary>
        public abstract Type ServiceType { get; }

#if UNITY_EDITOR

        /// <summary>
        /// Validate the service configuration <br/>
        /// Making sure that <see cref="ServiceType"/> is assigned, and derives from <see cref="ServiceType"/> (and is not abstract)
        /// </summary>
        /// <returns>Valid?</returns>
        internal bool Validate()
        {
            // Validate if ServiceType is assigned
            return ServiceType != null && (typeof(Service).IsAssignableFrom(ServiceType) && !ServiceType.IsAbstract);
        }

        /// <summary>
        /// Validate that ServiceType is part of <see cref="Service"/>
        /// </summary>
        protected virtual void OnValidate()
        {
            if (!Validate())
            {
                Debug.LogError($"Service Configuration of type {GetType()} has a {nameof(ServiceType)} that does not derive from {nameof(Service)}");
            }
        }
#endif
    }
}
