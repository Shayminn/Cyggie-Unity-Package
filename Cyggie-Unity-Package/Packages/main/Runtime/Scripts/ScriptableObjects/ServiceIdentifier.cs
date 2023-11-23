using Cyggie.Main.Runtime.Attributes;
using System;
using UnityEngine;

namespace Cyggie.Main.Runtime.ServicesNS.ScriptableObjects
{
    /// <summary>
    /// Identifier used to save a service type and create it during runtime
    /// </summary>
    [Serializable]
    public class ServiceIdentifier : ScriptableObject
    {
        [SerializeField, ReadOnly, Tooltip("Assembly qualified name used to create the service.")]
        private string _assemblyName = "";

        public string AssemblyName
        {
            get => _assemblyName;
            internal set => _assemblyName = value;
        }
    }
}
