using Cyggie.Plugins.UnityServices.Models;
using Cyggie.Plugins.SignalR.Models.Enums;
using UnityEngine;

namespace Cyggie.Plugins.SignalR.Services
{
    /// <summary>
    /// Service configuration for <see cref="SignalRUnityClientService"/>
    /// </summary>
    public class SignalRUnityClientServiceConfiguration : ServiceConfigurationSO
    {
        [Header("Configurations")]
        [SerializeField, Tooltip("Reconnection type applied to the WS client service.")]
        private SignalRReconnectionType _reconnectionType = SignalRReconnectionType.ReconnectConsistentDelay;

        [SerializeField, Tooltip("Delay for reconnection (in ms).")]
        private int _reconnectionDelay = 5000;

        /// <summary>
        /// Reconnection type applied to the WS client service
        /// </summary>
        public SignalRReconnectionType ReconnectionType => _reconnectionType;

        /// <summary>
        /// Delay for reconnection (in ms)<br/>
        /// When type is <see cref="SignalRReconnectionType.ReconnectConsistentDelay"/>, this value is applied at each attempt <br/>
        /// When type is <see cref="SignalRReconnectionType.ReconnectIncrementalDelay"/>, this value is incremented after every attempt
        /// </summary>
        public int ReconnectionDelay => _reconnectionDelay;
    }
}
