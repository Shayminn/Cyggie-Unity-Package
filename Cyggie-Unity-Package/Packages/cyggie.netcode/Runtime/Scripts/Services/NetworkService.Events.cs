using System;

namespace Cyggie.Netcode.Runtime.Services
{
    /// <summary>
    /// Partial class that contains all the events supported by the Network service
    /// </summary>
    public partial class NetworkService
    {
        /// <summary>
        /// Called right before we start a new host
        /// </summary>
        public EventHandler<StartHostEventArgs> OnStartHost = null;

        /// <summary>
        /// Called right before we start a new client
        /// </summary>
        public EventHandler OnStartClient = null;

        /// <summary>
        /// Called when we disconnect from a connection
        /// </summary>
        public EventHandler OnDisconnect = null;
    }
}
