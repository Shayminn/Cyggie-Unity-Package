using Steamworks;
using System.Linq;
using System;

namespace Cyggie.Steam.Runtime.Services
{
    /// <summary>
    /// Partial class that holds all the Properties
    /// </summary>
    public partial class SteamService
    {
#if !DISABELSTEAMWORKS
        private const int cAuthSessionTicketMaxSize = 1024;

        /// <summary>
        /// Get the connected steam's user ID
        /// </summary>
        public CSteamID UserID => SteamUser.GetSteamID();

        /// <summary>
        /// Get the current authentication session ticket
        /// </summary>
        public byte[] AuthSessionTicket
        {
            get
            {
                SteamNetworkingIdentity identity = new SteamNetworkingIdentity();

                byte[] ticketData = new byte[cAuthSessionTicketMaxSize];
                HAuthTicket ticket = SteamUser.GetAuthSessionTicket(ticketData, cAuthSessionTicketMaxSize, out uint ticketSize, ref identity);

                if (ticket != HAuthTicket.Invalid && ticketSize > 0)
                {
                    return ticketData.Take((int) ticketSize).ToArray();
                }
                else
                {
                    return Array.Empty<byte>();
                }
            }
        }
#endif
    }
}
