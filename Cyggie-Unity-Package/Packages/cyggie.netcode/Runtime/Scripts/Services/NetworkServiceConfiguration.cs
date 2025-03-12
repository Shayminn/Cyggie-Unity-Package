using Cyggie.Plugins.UnityServices.Models;
using Unity.Netcode;
using UnityEngine;

namespace Cyggie.Netcode.Runtime.Services
{
    /// <summary>
    /// Service configuration for <see cref="NetworkService"/>
    /// </summary>
    public class NetworkServiceConfiguration : ServiceConfigurationSO
    {
        [Header("References")]
        [SerializeField, Tooltip("Reference to the prefab that contains the network manager.")]
        private NetworkManager _networkManagerPrefab = null;

        [Header("Configurations")]
        [SerializeField, Tooltip("Maximum number of members in the same lobby.")]
        private int _maxMembers = 4;

        /// <summary>
        /// Maximum number of members in the same lobby
        /// </summary>
        public int MaxMembers => _maxMembers;

        /// <summary>
        /// Reference to the prefab that contains the network manager
        /// </summary>
        public NetworkManager NetworkManagerPrefab => _networkManagerPrefab;
    }
}
