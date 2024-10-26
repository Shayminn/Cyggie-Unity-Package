using Cyggie.Plugins.Services.Models;
using Cyggie.Plugins.SignalR.Models.Enums;

namespace Cyggie.Plugins.SignalR.Services
{
    /// <summary>
    /// Service configuration for <see cref="SignalRClientService"/>
    /// </summary>
    public class SignalRClientServiceConfiguration : ServiceConfiguration
    {
        /// <summary>
        /// Reconnection type applied to the WS client service
        /// </summary>
        public SignalRReconnectionType ReconnectionType { get; private set; } = SignalRReconnectionType.ReconnectConsistentDelay;

        /// <summary>
        /// Delay for reconnection (in ms)<br/>
        /// When type is <see cref="SignalRReconnectionType.ReconnectConsistentDelay"/>, this value is applied at each attempt <br/>
        /// When type is <see cref="SignalRReconnectionType.ReconnectIncrementalDelay"/>, this value is incremented after every attempt
        /// </summary>
        public int ReconnectionDelay { get; private set; } = 5000;

        /// <summary>
        /// Create new service configuration for <see cref="SignalRClientService"/>
        /// </summary>
        /// <param name="reconnectionType">Reconnection type (defaults to <see cref="SignalRReconnectionType.ReconnectConsistentDelay"/></param>
        /// <param name="reconnectionDelay">Delay for reconnection</param>
        public SignalRClientServiceConfiguration(SignalRReconnectionType reconnectionType = SignalRReconnectionType.ReconnectConsistentDelay, int reconnectionDelay = 0)
        {
            ReconnectionType = reconnectionType;
            ReconnectionDelay = reconnectionDelay;
        }
    }
}
