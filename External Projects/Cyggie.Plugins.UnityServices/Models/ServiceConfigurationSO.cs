using Cyggie.Plugins.Services.Interfaces;
using Cyggie.Plugins.Services.Models;
using System;
using UnityEngine;

namespace Cyggie.Plugins.UnityServices.Models
{
    /// <summary>
    /// Abstract class used by the ServiceManagerMono in Unity <br/>
    /// 
    /// Note: <br/>
    /// If you inherit this with a custom <see cref="ServiceConfiguration"/> and use this with <see cref="ServiceManager"/> <br/>
    /// Your configuration will be NULL <br/>
    /// Since it is impossible to create a ScriptableObject from an external project outside of Unity <br/>
    /// However, you can still use const variables within it if you wish to
    /// </summary>
    [Serializable]
    public abstract class ServiceConfigurationSO : ScriptableObject, IServiceConfiguration
    {

    }
}
