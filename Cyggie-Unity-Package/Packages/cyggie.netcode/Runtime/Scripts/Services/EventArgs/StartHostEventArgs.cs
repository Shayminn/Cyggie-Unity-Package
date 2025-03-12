using System;

namespace Cyggie.Netcode.Runtime.Services
{
    /// <summary>
    /// Event args for <see cref="NetworkService.OnStartHost"/>
    /// </summary>
    public class StartHostEventArgs : EventArgs
    {
        /// <summary>
        /// Maximum number of member allowed in this lobby
        /// </summary>
        public int MaxMembers { get; private set; }

        internal StartHostEventArgs(int maxMembers)
        {
            MaxMembers = maxMembers;
        }
    }
}
