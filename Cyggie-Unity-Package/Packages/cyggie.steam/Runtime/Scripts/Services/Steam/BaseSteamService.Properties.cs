using Steamworks;
using System;

namespace Cyggie.Steam.Runtime.Services
{
    /// <summary>
    /// Partial class that holds all the Properties
    /// </summary>
    public abstract partial class BaseSteamService<T>
    {
        private const int cAuthSessionTicketMaxSize = 1024;
        private AuthTicket _ticket = null;

        /// <summary>
        /// Whether the steam client is connected to the Steam servers
        /// </summary>
        public bool IsConnected => SteamClient.IsLoggedOn;

        /// <summary>
        /// Get the connected steam's user ID
        /// </summary>
        public ulong UserID => SteamClient.SteamId.Value;

        /// <summary>
        /// Get the connected steam's user name
        /// </summary>
        public string Username => SteamClient.Name;

        /// <summary>
        /// Get the current authentication session ticket
        /// </summary>
        public string AuthSessionTicketData => AuthSessionTicket == null ? string.Empty : Convert.ToBase64String(AuthSessionTicket.Data);

        private AuthTicket AuthSessionTicket => _ticket ??= SteamUser.GetAuthSessionTicket();
    }
}
