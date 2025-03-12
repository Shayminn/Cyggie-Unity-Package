using System;

namespace Cyggie.SteamNetcode.Runtime.Services
{
    /// <summary>
    /// Partial class that contains all the events supported by the Steam Network service
    /// </summary>
    public partial class SteamNetworkService
    {
        /// <summary>
        /// Called when a lobby is joined
        /// </summary>
        public EventHandler OnLobbyJoined = null;
    }
}
