﻿using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Models;
using Cyggie.Plugins.SignalR.Models.Enums;
using Cyggie.Plugins.SignalR.Utils.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cyggie.Plugins.SignalR.Services
{
    /// <summary>
    /// Service for connecting to a WebSocket server using SignalR
    /// </summary>
    public class SignalRClientService : Service<SignalRClientServiceConfiguration>
    {
        /// <summary>
        /// Called when the connection is established
        /// </summary>
        public Action? OnConnected = null;

        /// <summary>
        /// Called when the connection is disconnected
        /// </summary>
        public Action? OnDisconnected = null;

        /// <summary>
        /// Called when the connection is closed
        /// </summary>
        public Action<Exception?>? OnClosed = null;

        private HubConnection? _conn = null;
        private bool _expectDisconnection = false;
        private bool _dispose = false;
        private int _reconnectionAttempts = 0;

        /// <summary>
        /// Whether a connection has been established
        /// </summary>
        public bool IsConnected => _conn != null && !string.IsNullOrEmpty(_conn.ConnectionId);

        private readonly List<SignalRClientMethod> _methods = new List<SignalRClientMethod>();

        /// <summary>
        /// Build a new connection to a hub <paramref name="url"/>
        /// </summary>
        /// <param name="url">Hub URL to connect to</param>
        public async Task BuildConnection(string url)
        {
            if (_conn != null && _conn.State == HubConnectionState.Connected)
            {
                Log.Debug($"Building a new connection while the previous one is active... disconnecting it.", nameof(SignalRClientService));
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
        public void RegisterReceivingMethods(params SignalRClientMethod[] methods)
        {
            if (_conn == null)
            {
                Log.Error($"Failed to register receiving methods, connection has not been built yet, use BuildConnection() first.", nameof(SignalRClientService));
                return;
            }

            foreach (SignalRClientMethod method in methods)
            {
                Log.Debug($"Registering method to WebSocket client: {method.MethodName} ({method.PrintParams()}).", nameof(SignalRClientService));

                if (_methods.Any(x => x.MethodName == method.MethodName))
                {
                    Log.Error($"Method overloading on the client-side is not allowed: {method.MethodName}", nameof(SignalRClientService));
                    continue;
                }

                _methods.Add(method);
                Action<object?[]>? callback = method.Callback;
                callback ??= (object?[] objs) =>
                {
                    Log.Error($"WS Client method callback is null for {method.MethodName}.", nameof(SignalRClientService));
                };

                _conn.On(method.MethodName, method.FilteredTypes, callback);
            }

            Log.Debug($"Registered {methods.Length} callback(s).", nameof(SignalRClientService));
        }

        /// <summary>
        /// Establish connection to the hub to start sending/receiving <br/>
        /// Make sure you register receiving methods (using <see cref="RegisterReceivingMethods(SignalRClientMethod[])"/> before connecting
        /// </summary>
        public async Task Connect()
        {
            if (_conn == null)
            {
                Log.Error($"Failed to connect, connection has not been built yet, use BuildConnection() first.", nameof(SignalRClientService));
                return;
            }

            try
            {
                Log.Debug($"Connecting to websocket server...", nameof(SignalRClientService));

                await _conn.StartAsync();

                _reconnectionAttempts = 0;

                Log.Debug($"Connection successfully established.", nameof(SignalRClientService));
                OnConnected?.Invoke();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to start connection, exception: {ex}.", nameof(SignalRClientService));

                await CheckForReconnection();
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
                Log.Error($"Failed to disconnect, connection has not been built yet, use BuildConnection() first.", nameof(SignalRClientService));
                return;
            }

            if (_conn.State == HubConnectionState.Disconnected)
            {
                Log.Error($"Failed to disconnect, connection is already disconnected.", nameof(SignalRClientService));
                return;
            }

            Log.Debug($"Current state: {_conn.State}. Disconnecting...", nameof(SignalRClientService));

            _expectDisconnection = true;
            await _conn.StopAsync();

            _methods.Clear();
            Log.Debug($"Connection disconnected successfully.", nameof(SignalRClientService));
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
                Log.Error($"Failed in call for {methodName}, connection has not been built yet, use BuildConnection() first.", nameof(SignalRClientService));
                return;
            }

            if (_conn.State != HubConnectionState.Connected)
            {
                Log.Error($"Failed in call for {methodName}, hub is not connected, use Connect() first.");
                return;
            }

            try
            {
                Log.Debug($"Calling {methodName} with {args.Length} arguments.", nameof(SignalRClientService));

                await _conn.InvokeCoreAsync(methodName, args);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed in call for {methodName} (args: {args.Length}), exception: {ex}.", nameof(SignalRClientService));
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

            if (ex != null)
            {
                Log.Error($"Connection was unexpectedly closed due to an exception: {ex}.", nameof(SignalRClientService));
            }

            OnClosed?.Invoke(ex);

            // Disconnection was expected, don't try to reconnect (called from Disconnect)
            if (_expectDisconnection)
            {
                _expectDisconnection = false;
                return;
            }

            await CheckForReconnection();
        }

        private async Task CheckForReconnection()
        {
            if (_conn == null || IsConnected || _dispose) return;

            bool shouldReconnect = false;
            double delayInMs = 0;
            switch (Configuration.ReconnectionType)
            {
                case SignalRReconnectionType.NoReconnect:
                    break;

                case SignalRReconnectionType.ReconnectOnce:
                    shouldReconnect = _reconnectionAttempts == 0;
                    delayInMs = Configuration.ReconnectionDelay;
                    break;

                case SignalRReconnectionType.ReconnectConsistentDelay:
                    shouldReconnect = true;
                    delayInMs = Configuration.ReconnectionDelay;
                    break;

                case SignalRReconnectionType.ReconnectIncrementalDelay:
                    shouldReconnect = true;
                    delayInMs = Configuration.ReconnectionDelay * Math.Pow(_reconnectionAttempts + 1, 2);
                    break;

                default:
                    Log.Error($"Unhandled switch-case type of {typeof(SignalRReconnectionType)}: {Configuration.ReconnectionType}.", nameof(SignalRClientService));
                    break;
            }

            if (shouldReconnect)
            {
                await Reconnect(delayInMs);
            }
        }

        private async Task Reconnect(double delayInMs = 0)
        {
            if (_conn == null || IsConnected || _dispose) return;

            if (delayInMs > 0)
            {
                Log.Error($"Failed to connect, retrying in {delayInMs}ms...", nameof(SignalRClientService));

                const int checkIntervalInMs = 250;
                while (delayInMs > 0)
                {
                    await Task.Delay(checkIntervalInMs);

                    if (IsConnected)
                    {
                        _reconnectionAttempts = 0;
                        return;
                    }
                    else
                    {
                        delayInMs -= checkIntervalInMs;
                    }
                }
            }

            _reconnectionAttempts++;
            await Connect();
        }
    }
}
