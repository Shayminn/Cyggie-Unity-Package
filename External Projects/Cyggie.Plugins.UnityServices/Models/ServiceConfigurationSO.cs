using Cyggie.Plugins.Services.Interfaces;
using System;
using UnityEngine;

namespace Cyggie.Plugins.UnityServices.Models
{
    /// <summary>
    /// Abstract class used by the ServiceManagerMono in Unity
    /// </summary>
    [Serializable]
    public abstract class ServiceConfigurationSO : ScriptableObject, IServiceConfiguration
    {

    }
}
