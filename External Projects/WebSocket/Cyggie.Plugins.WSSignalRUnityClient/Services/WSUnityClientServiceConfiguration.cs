using Cyggie.Plugins.UnityServices.Models;
using Cyggie.Plugins.WebSocket.Models.Enums;
using UnityEngine;

namespace Cyggie.Plugins.WebSocket
{
    /// <summary>
    /// Service configuration for <see cref="WSUnityClientService"/>
    /// </summary>
    public class WSUnityClientServiceConfiguration : ServiceConfigurationSO
    {
        [Header("Configurations")]
        [SerializeField, Tooltip("Reconnection type applied to the WS client service.")]
        private WSReconnectionType _reconnectionType = WSReconnectionType.ReconnectConsistentDelay;

        [SerializeField, Tooltip("Delay for reconnection (in ms).")]
        private int _reconnectionDelay = 5000;

        /// <summary>
        /// Reconnection type applied to the WS client service
        /// </summary>
        public WSReconnectionType ReconnectionType => _reconnectionType;

        /// <summary>
        /// Delay for reconnection (in ms)<br/>
        /// When type is <see cref="WSReconnectionType.ReconnectConsistentDelay"/>, this value is applied at each attempt <br/>
        /// When type is <see cref="WSReconnectionType.ReconnectIncrementalDelay"/>, this value is incremented after every attempt
        /// </summary>
        public int ReconnectionDelay => _reconnectionDelay;
    }
}
