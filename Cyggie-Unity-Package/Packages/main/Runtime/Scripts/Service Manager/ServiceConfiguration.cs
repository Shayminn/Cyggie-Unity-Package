using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cyggie.Main.Runtime.Services
{
    [Serializable]
    public abstract class ServiceConfiguration : ScriptableObject
    {
        public abstract Type ServiceType { get; }

#if UNITY_EDITOR
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
