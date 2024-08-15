using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Models;
using Cyggie.Plugins.WebSocket.Models;
using Cyggie.Plugins.WebSocket.Utils.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cyggie.Plugins.WebSocket.Services
{
    /// <summary>
    /// Service for connecting to a WebSocket server using SignalR
    /// </summary>
    public class WSClientService : Service
    {
        /// <summary>
        /// Called when the connection is established
        /// </summary>
        public Action? OnConnected = null;

        /// <summary>
        /// Called when the connection is closed
        /// </summary>
        public Action<Exception?>? OnClosed = null;

        /// <summary>
        /// Array of reconnection delays based on the number of attempts (usually incrementing on each new attempt) <br/>
        /// By default; 15s, 60s, 150s, 300s, 600s
        /// </summary>
        public int[] ReconnectionDelays { private get; set; } = { 5, 20, 60, 150, 300 };

        private HubConnection? _conn = null;
        private bool _expectDisconnection = false;
        private bool _dispose = false;
        private int _reconnectionAttempts = 0;

        /// <summary>
        /// Whether a connection has been established
        /// </summary>
        public bool IsConnected => _conn != null && !string.IsNullOrEmpty(_conn.ConnectionId);

        /// <summary>
        /// Build a new connection to a hub <paramref name="url"/>
        /// </summary>
        /// <param name="url">Hub URL to connect to</param>
        public async Task BuildConnection(string url)
        {
            if (_conn != null && _conn.State == HubConnectionState.Connected)
            {
                Log.Debug($"Building a new connection while the previous one is active... disconnecting it.", nameof(WSClientService));
                await Disconnect();
            }

            _conn = new HubConnectionBuilder()
                            .WithUrl(url)
                            .Build();

            _conn.Closed += OnConnectionClosed;
        }

        /// <summary>
        /// Register methods/callbacks to the hub connection
        /// </summary>
        public void RegisterReceivingMethods(params WSClientMethod[] methods)
        {
            if (_conn == null)
            {
                Log.Error($"Failed to register receiving methods, connection has not been built yet, use BuildConnection() first.", nameof(WSClientService));
                return;
            }

            foreach (WSClientMethod method in methods)
            {
                Log.Debug($"Registering method to WebSocket client: {method.MethodName} ({(method.ParameterTypes.Any() ? method.PrintParams() : "parameterless")}).", nameof(WSClientService));

                Action<object?[]>? callback = method.Callback;
                callback ??= (object?[] objs) =>
                {
                    Log.Error($"WS Client method callback is null for {method.MethodName}.");
                };

                _conn.On(method.MethodName, method.ParameterTypes, callback);
            }

            Log.Debug($"Registered {methods.Length} callback.", nameof(WSClientService));
        }

        /// <summary>
        /// Establish connection to the hub to start sending/receiving <br/>
        /// Make sure you register receiving methods (using <see cref="RegisterReceivingMethods(WSClientMethod[])"/> before connecting
        /// </summary>
        public async Task Connect()
        {
            if (_conn == null)
            {
                Log.Error($"Failed to connect, connection has not been built yet, use BuildConnection() first.", nameof(WSClientService));
                return;
            }

            try
            {
                Log.Debug($"Connecting to websocket server.", nameof(WSClientService));

                await _conn.StartAsync();

                _reconnectionAttempts = 0;

                Log.Debug($"Connection successfully established.", nameof(WSClientService));
                OnConnected?.Invoke();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to start connection, exception: {ex}.", nameof(WSClientService));
                await Reconnect();
            }
        }

        /// <summary>
        /// Disconnect from the established connection
        /// </summary>
        /// <returns></returns>
        public async Task Disconnect()
        {
            if (_conn == null)
            {
                Log.Error($"Failed to disconnect, connection has not been built yet, use BuildConnection() first.", nameof(WSClientService));
                return;
            }

            if (_conn.State == HubConnectionState.Disconnected)
            {
                Log.Error($"Failed to disconnect, connection is already disconnected.", nameof(WSClientService));
                return;
            }

            Log.Debug($"Current state: {_conn.State}. Disconnecting...", nameof(WSClientService));

            _expectDisconnection = true;
            await _conn.StopAsync();

            Log.Debug($"Connection disconnected successfully.", nameof(WSClientService));
        }

        /// <summary>
        /// Call a method on the connection to the server
        /// </summary>
        /// <param name="methodName">Method name to call</param>
        /// <param name="args">Arguments to pass to the method</param>
        /// <returns></returns>
        public async Task Call(string methodName, params object?[] args)
        {
            if (_conn == null)
            {
                Log.Error($"Failed in call for {methodName}, connection has not been built yet, use BuildConnection() first.", nameof(WSClientService));
                return;
            }

            if (_conn.State != HubConnectionState.Connected)
            {
                Log.Error($"Failed in call for {methodName}, hub is not connected, use Connect() first.");
                return;
            }

            try
            {
                Log.Debug($"Calling {methodName} with {args.Length} arguments.", nameof(WSClientService));

                await _conn.InvokeCoreAsync(methodName, args);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed in call for {methodName} (args: {args.Length}), exception: {ex}.", nameof(WSClientService));
            }
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            _dispose = true;
        }

        private async Task OnConnectionClosed(Exception? ex)
        {
            if (_conn == null) return;

            if (_expectDisconnection)
            {
                _expectDisconnection = false;
                return;
            }

            if (ex != null)
            {
                Log.Debug($"Connection was unexpectedly closed due to an exception: {ex}.", nameof(WSClientService));
            }

            await Reconnect();
        }

        private async Task Reconnect()
        {
            if (_conn == null || IsConnected || _dispose) return;

            int delay = ReconnectionDelays[_reconnectionAttempts];
            Log.Error($"Failed to connect, retrying in {delay}s...", nameof(WSClientService));

            while (delay > 0)
            {
                await Task.Delay(1000);

                if (IsConnected)
                {
                    _reconnectionAttempts = 0;
                    return;
                }
                else
                {
                    delay--;
                }
            }

            _reconnectionAttempts++;
            await Connect();
        }
    }
}
