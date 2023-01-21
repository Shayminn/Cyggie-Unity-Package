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
        /// Validate that ServiceType is part of <see cref="Service"/>
        /// </summary>
        private void OnValidate()
        {
            if (ServiceType != null)
            {
                if (!ServiceType.IsAssignableFrom(typeof(Service)) || ServiceType.IsAbstract)
                {
                    Debug.LogError($"Service Configuration of type {GetType()} has a {nameof(ServiceType)} that does not derive from {nameof(Service)}");
                }
            }
        }
#endif
    }
}
