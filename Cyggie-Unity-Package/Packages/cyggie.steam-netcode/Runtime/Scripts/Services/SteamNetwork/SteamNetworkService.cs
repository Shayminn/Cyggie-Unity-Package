using Cyggie.Netcode.Runtime.Services;
using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Models;
using Cyggie.Steam.Runtime.Services;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using System;
using Unity.Netcode;

namespace Cyggie.SteamNetcode.Runtime.Services
{
    /// <summary>
    /// Service that manages the Steam API and connects it to lobbies w/ Netcode
    /// </summary>
    public partial class SteamNetworkService : BaseSteamService<SteamNetworkServiceConfiguration>
    {
        private Lobby _currentLobby;
        private NetworkService _networkService = null;
        private FacepunchTransport _transport = null;

        public Lobby Lobby => _currentLobby;

        /// <inheritdoc/>
        public override void OnAllServicesInitialized()
        {
            base.OnAllServicesInitialized();

            if (!ServiceManager.TryGet(out _networkService))
            {
                Log.Error($"Failed on all services initialized, steam network service requires the {typeof(NetworkService)} to be included in the service manager.", nameof(SteamNetworkService));
                return;
            }

            if (!_networkService.NetworkManager.TryGetComponent(out _transport))
            {
                Log.Error($"Failed on all services initialized, unable to find {typeof(FacepunchTransport)} in network manager, make sure you assign a network manager with the {typeof(FacepunchTransport)} component.", nameof(SteamNetworkService));
                return;
            }
        }

        #region Mono implementations

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            base.OnEnable();

            SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
            SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
            SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;

            _networkService.OnStartHost += OnStartHost;
            _networkService.OnStartClient += OnStartClient;
            _networkService.OnDisconnect += OnDisconnect;
        }

        /// <inheritdoc/>
        protected override void OnDisable()
        {
            base.OnDisable();

            SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyInvite -= OnLobbyInvite;
            SteamMatchmaking.OnLobbyGameCreated -= OnLobbyGameCreated;
            SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;

            _networkService.OnStartHost -= OnStartHost;
            _networkService.OnStartClient -= OnStartClient;
            _networkService.OnDisconnect -= OnDisconnect;
        }

        #endregion

        #region Event handlers

        #region Steam

        private void OnLobbyCreated(Result result, Steamworks.Data.Lobby lobby)
        {
            if (result != Result.OK)
            {
                Log.Error($"Failed to create lobby.", nameof(SteamNetworkService));
                return;
            }

            lobby.SetPublic();
            lobby.SetJoinable(true);
            lobby.SetGameServer(lobby.Owner.Id);
            Log.Debug($"Lobby created successfully. Owner: {lobby.Owner.Name}", nameof(SteamNetworkService));
        }

        private void OnLobbyEntered(Steamworks.Data.Lobby lobby)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                OnLobbyJoined?.Invoke(this, null);
                return;
            }

            _transport.targetSteamId = _currentLobby.Owner.Id;
            _networkService.StartClient();
            OnLobbyJoined?.Invoke(this, null);
        }

        private void OnLobbyMemberJoined(Steamworks.Data.Lobby lobby, Friend friend)
        {
            Log.Debug($"Lobby member joined.", nameof(SteamNetworkService));
        }

        private void OnLobbyMemberLeave(Steamworks.Data.Lobby lobby, Friend friend)
        {
            Log.Debug($"Lobby member leave.", nameof(SteamNetworkService));
        }

        private void OnLobbyInvite(Friend friend, Steamworks.Data.Lobby lobby)
        {
            Log.Debug($"Lobby invite received from: {friend.Name} ({friend.Id}).", nameof(SteamNetworkService));
        }

        private void OnLobbyGameCreated(Steamworks.Data.Lobby lobby, uint ip, ushort port, SteamId steamId)
        {
            Log.Debug($"Lobby created successfully.", nameof(SteamNetworkService));

        }

        private async void OnGameLobbyJoinRequested(Steamworks.Data.Lobby lobby, SteamId steamId)
        {
            // When you accept an invite or join a friend
            RoomEnter joinedLobby = await lobby.Join();
            if (joinedLobby != RoomEnter.Success)
            {
                Log.Error($"Failed on game lobby join requested: {joinedLobby}.", nameof(SteamNetworkService));
                return;
            }

            _currentLobby = lobby;
            Log.Debug($"Joined lobby successfully.", nameof(SteamNetworkService));
        }

        #endregion

        #region Network

        private async void OnStartHost(object sender, StartHostEventArgs args)
        {
            Lobby? createdLobby = await SteamMatchmaking.CreateLobbyAsync(args.MaxMembers);
            if (createdLobby == null)
            {
                Log.Error($"Failed on start host, unable to create lobby from steam matchmaking.", nameof(SteamNetworkService));
                return;
            }

            _currentLobby = createdLobby.Value;
        }

        private void OnStartClient(object sender, EventArgs args)
        {
        }

        private void OnDisconnect(object sender, EventArgs args)
        {

        }

        #endregion

        #endregion
    }
}
