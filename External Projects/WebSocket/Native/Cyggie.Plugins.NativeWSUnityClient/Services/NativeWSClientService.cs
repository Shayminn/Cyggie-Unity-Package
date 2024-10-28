using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Models;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cyggie.Plugins.NativeWSClient.Services
{
    /// <summary>
    /// Service that manages the Native WS client
    /// </summary>
    public class NativeWSClientService : Service
    {
        private ClientWebSocket _client = new ClientWebSocket();

        private CancellationTokenSource? _readCancellationSource;
        private int _readBufferSize;
        private int _writeBufferSize;

        /// <summary>
        /// Whether the WS connection is currently connected
        /// </summary>
        public bool IsConnected => _client.State == WebSocketState.Open;

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Set default buffer sizes
            SetBuffers(8192, 8192);
        }

        /// <summary>
        /// Set the receive and send buffer sizes in bytes
        /// </summary>
        /// <param name="receiveSize">Receive buffer size in bytes</param>
        /// <param name="sendSize">Send buffer size in bytes</param>
        public void SetBuffers(int receiveSize, int sendSize)
        {
            _readBufferSize = receiveSize;
            _writeBufferSize = sendSize;

            _client.Options.SetBuffer(_readBufferSize, _writeBufferSize);
        }

        /// <summary>
        /// Set the request headers for the client
        /// </summary>
        /// <param name="headers">Collection of all headers to set</param>
        public void SetRequestHeaders(params KeyValuePair<string, string>[] headers)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                if (string.IsNullOrEmpty(header.Key))
                {
                    Log.Error($"Header key is null or empty, skipping...", nameof(NativeWSClientService));
                    continue;
                }

                if (string.IsNullOrEmpty(header.Value))
                {
                    Log.Error($"Header value for key {header.Key} is null or empty, skipping...", nameof(NativeWSClientService));
                    continue;
                }

                _client.Options.SetRequestHeader(header.Key, header.Value);
                Log.Debug($"Set header to WS client: {header.Key} {header.Value}", nameof(NativeWSClientService));
            }
        }

        /// <summary>
        /// Connect to the WS server
        /// </summary>
        /// <param name="url">Url of the WS server</param>
        public async Task Connect(string url)
        {
            Log.Debug($"Connecting to: {url}", nameof(NativeWSClientService));

            try
            {
                await _client.ConnectAsync(new Uri(url), CancellationToken.None);
                Log.Debug($"Connected to WS server.", nameof(NativeWSClientService));
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to connect to WS server, exception: {ex}", nameof(NativeWSClientService));
            }
        }

        /// <summary>
        /// Send a message to the WS server
        /// </summary>
        /// <param name="message">Message to send</param>
        public async Task Send(string message)
        {
            if (!IsConnected)
            {
                Log.Error($"Failed to send message, server connection is not established.", nameof(NativeWSClientService));
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
            await _client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// Start listening to messages from the WS server
        /// </summary>
        /// <param name="onMessageReceived">Called when a new message is received</param>
        /// <param name="onListeningStopped"></param>
        /// <param name="onError"></param>
        public async Task StartListening(Action<string> onMessageReceived, Action? onListeningStopped = null, Action<Exception>? onError = null)
        {
            if (!IsConnected)
            {
                Log.Error($"Failed to send message, server connection is not established.", nameof(NativeWSClientService));
                return;
            }

            _readCancellationSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _readCancellationSource.Token;
            int errorStack = 0;

            Log.Debug($"Now listening to messages.", nameof(NativeWSClientService));
            while (true)
            {
                try
                {
                    // Wait for message from server
                    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[_readBufferSize]);
                    WebSocketReceiveResult result = await _client.ReceiveAsync(buffer, cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        break;
                    }

                    // Convert received message into string
                    string receivedMessage = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                    Log.Debug($"Received message: {receivedMessage}.", nameof(NativeWSClientService));


                    errorStack = 0;
                    onMessageReceived.Invoke(receivedMessage);
                }
                catch (OperationCanceledException)
                {
                    _readCancellationSource = null;
                    Log.Debug($"Listening cancelled manually.", nameof(NativeWSClientService));
                    break;
                }
                catch (Exception ex)
                {
                    Log.Error($"Unhandled exception during listen: {ex}.", nameof(NativeWSClientService));
                    onError?.Invoke(ex);

                    // Break from looped error after 10
                    errorStack++;
                    if (errorStack > 10) break;
                }
            }

            onListeningStopped?.Invoke();
        }

        /// <summary>
        /// Stop listening to messages from the server
        /// </summary>
        public void StopListening()
        {
            if (!IsConnected)
            {
                Log.Error($"Failed to send message, server connection is not established.", nameof(NativeWSClientService));
                return;
            }

            if (_readCancellationSource == null)
            {
                Log.Error($"Failed to stop listening to server, listening has not started.");
                return;
            }

            _readCancellationSource.Cancel();
        }
    }
}
