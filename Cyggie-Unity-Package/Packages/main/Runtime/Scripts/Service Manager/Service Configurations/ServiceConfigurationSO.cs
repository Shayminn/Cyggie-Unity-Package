using System;
using UnityEngine;

namespace Cyggie.Main.Runtime.ServicesNS
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class ServiceConfigurationSO : ScriptableObject
    {
        /// <summary>
        /// The type of service that this configuration is associated to
        /// </summary>
        public abstract Type ServiceType { get; }

#if UNITY_EDITOR

        /// <summary>
        /// Whether this configuration is dedicated to a Cyggie Package service
        /// </summary>
        internal virtual bool IsPackageSettings => false;

        /// <summary>
        /// Called when the scriptable object is created <br/>
        /// Assign any necessary references that needs to be assigned
        /// </summary>
        internal virtual void OnScriptableObjectCreated() { }

#endif
 
    }
}
