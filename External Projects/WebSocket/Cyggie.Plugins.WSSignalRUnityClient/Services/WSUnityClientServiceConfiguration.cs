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
        /// <summary>
        /// Reconnection type applied to the WS client service
        /// </summary>
        [Tooltip("Reconnection type applied to the WS client service.")]
        public WSReconnectionType ReconnectionType { get; private set; } = WSReconnectionType.ReconnectConsistentDelay;

        /// <summary>
        /// Delay for reconnection (in ms)<br/>
        /// When type is <see cref="WSReconnectionType.ReconnectConsistentDelay"/>, this value is applied at each attempt <br/>
        /// When type is <see cref="WSReconnectionType.ReconnectIncrementalDelay"/>, this value is incremented after every attempt
        /// </summary>
        [Tooltip("Delay for reconnection (in ms).")]
        public int ReconnectionDelay { get; private set; } = 5;
    }
}
