namespace Cyggie.Plugins.SignalR.Models.Enums
{
    /// <summary>
    /// Type of reconnection to apply when the websocket disconnects
    /// </summary>
    public enum SignalRReconnectionType
    {
        /// <summary>
        /// Does not reconnect at all
        /// </summary>
        NoReconnect,

        /// <summary>
        /// Reconnects only once
        /// </summary>
        ReconnectOnce,

        /// <summary>
        /// Reconnects until it is re-established with a consistent delay in between each reconnection attempt
        /// </summary>
        ReconnectConsistentDelay,

        /// <summary>
        /// Reconnects until it is re-established with an incremental delay in between each reconnection attempt <br/>
        /// Initial delay * (Number of reconnection attempted + 1) ^ 2 <br/>
        /// i.e. Initial delay = 5s <br/>
        /// 1st reconnection attempt = 5s <br/>
        /// 2nd reconnection attempt = 20s <br/>
        /// 3rd reconnection attempt = 45s <br/>
        /// ...
        /// </summary>
        ReconnectIncrementalDelay,
    }
}
