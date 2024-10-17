using Cyggie.Plugins.Services.Models;
using Cyggie.Plugins.WebSocket.Models.Enums;

namespace Cyggie.Plugins.WebSocket
{
    /// <summary>
    /// Service configuration for <see cref="WSClientService"/>
    /// </summary>
    public class WSClientServiceConfiguration : ServiceConfiguration
    {
        /// <summary>
        /// Reconnection type applied to the WS client service
        /// </summary>
        public WSReconnectionType ReconnectionType { get; private set; } = WSReconnectionType.ReconnectConsistentDelay;

        /// <summary>
        /// Delay for reconnection (in ms)<br/>
        /// When type is <see cref="WSReconnectionType.ReconnectConsistentDelay"/>, this value is applied at each attempt <br/>
        /// When type is <see cref="WSReconnectionType.ReconnectIncrementalDelay"/>, this value is incremented after every attempt
        /// </summary>
        public int ReconnectionDelay { get; private set; } = 5000;

        /// <summary>
        /// Create new service configuration for <see cref="WSClientService"/>
        /// </summary>
        /// <param name="reconnectionType">Reconnection type (defaults to <see cref="WSReconnectionType.ReconnectConsistentDelay"/></param>
        /// <param name="reconnectionDelay">Delay for reconnection</param>
        public WSClientServiceConfiguration(WSReconnectionType reconnectionType = WSReconnectionType.ReconnectConsistentDelay, int reconnectionDelay = 0)
        {
            ReconnectionType = reconnectionType;
            ReconnectionDelay = reconnectionDelay;
        }
    }
}
