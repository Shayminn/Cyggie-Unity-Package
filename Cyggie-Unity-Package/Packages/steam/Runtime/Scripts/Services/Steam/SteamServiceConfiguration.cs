using Cyggie.Plugins.UnityServices.Models;
using UnityEngine;

namespace Cyggie.Steam.Runtime.Services
{
    /// <summary>
    /// Service configuration for <see cref="SteamService"/>
    /// </summary>
    public class SteamServiceConfiguration : ServiceConfigurationSO
    {
        [SerializeField, Tooltip("Whether the Steam debug callbacks should be enabled. (Enable strictly for development purposes as it is heavy on the system)")]
        private bool _enableDebugCallbacks = false;

        public bool EnableDebugCallbacks => _enableDebugCallbacks;
    }
}
