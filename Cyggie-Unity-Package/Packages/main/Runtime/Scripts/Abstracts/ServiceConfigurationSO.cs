using Cyggie.Plugins.Services.Interfaces;
using System;
using UnityEngine;

namespace Cyggie.Main.Runtime.ServicesNS.ScriptableObjects
{
    /// <summary>
    /// Service configuration for a service
    /// </summary>
    [Serializable]
    public abstract class ServiceConfigurationSO : ScriptableObject, IServiceConfiguration
    {

    }
}
