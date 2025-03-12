using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Plugins.Logs;
using Unity.Netcode;

namespace Cyggie.Netcode.Runtime.Services
{
    /// <summary>
    /// Service that manages the network using Unity's Netcode
    /// </summary>
    public partial class NetworkService : ServiceMono<NetworkServiceConfiguration>
    {
        /// <summary>
        /// Netcode manager that handles the networking
        /// </summary>
        public NetworkManager NetworkManager { get; private set; }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if (Configuration.NetworkManagerPrefab == null)
            {
                Log.Error($"Failed on initialized, network manager prefab reference is null. Assign it in the Inspector.", nameof(NetworkService));
            }
            else
            {
                // Parent must be null since Network Manager can't be parented to another object
                NetworkManager = Instantiate(Configuration.NetworkManagerPrefab, parent: null);
            }
        }

        /// <inheritdoc/>
        protected override void OnApplicationQuit()
        {
            Disconnect();
        }

        /// <summary>
        /// Start a network host
        /// </summary>
        public void StartHost()
        {
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            OnStartHost?.Invoke(this, new StartHostEventArgs(Configuration.MaxMembers));

            NetworkManager.Singleton.StartHost();
        }

        /// <summary>
        /// Start a network client
        /// </summary>
        public void StartClient()
        {
            NetworkManager.Singleton.OnClientStarted += OnClientStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            OnStartClient?.Invoke(this, null);

            NetworkManager.Singleton.StartClient();
        }

        public void Disconnect()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening) return;
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            }
            else
            {
                NetworkManager.Singleton.OnClientStarted -= OnClientStarted;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }

            NetworkManager.Singleton.Shutdown(true);
            OnDisconnect?.Invoke(this, null);
            Log.Debug($"Network connection disconnected successfully.", nameof(NetworkService));
        }

        private void OnServerStarted()
        {
            Log.Debug($"Server started successfully.", nameof(NetworkService));
        }

        private void OnClientStarted()
        {
            Log.Debug($"Client started successfully.", nameof(NetworkService));
        }

        private void OnClientConnected(ulong obj)
        {
            Log.Debug($"Client connected: {obj}.");
        }
        private void OnClientDisconnected(ulong obj)
        {
            Log.Debug($"Client disconnected: {obj}.");
        }
    }
}
