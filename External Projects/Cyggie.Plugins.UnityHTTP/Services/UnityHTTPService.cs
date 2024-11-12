using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Models;
using Cyggie.Plugins.UnityServices.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Cyggie.Plugins.UnityHTTP.Services
{
    /// <summary>
    /// Service that simplifies sending <see cref="UnityWebRequest"/>
    /// </summary>
    public class UnityHTTPService : Service, IServiceMono
    {
        private readonly Dictionary<string, string> _defaultHeaders = new Dictionary<string, string>();
        private MonoBehaviour? _mono = null;

        /// <inheritdoc/>
        public void OnMonoBehaviourAssigned(MonoBehaviour mono)
        {
            _mono = mono;
        }

        /// <summary>
        /// Add default headers to all requests <br/>
        /// This does not include requests created outside of the service
        /// </summary>
        /// <param name="headers">Headers to always include in future requests</param>
        public void AddDefaultHeaders(params KeyValuePair<string, string>[] headers)
        {
            // Add headers to request
            foreach (KeyValuePair<string, string> header in headers)
            {
                if (string.IsNullOrEmpty(header.Key))
                {
                    Log.Error($"Header key is null or empty, skipping...", nameof(UnityHTTPService));
                    continue;
                }

                if (string.IsNullOrEmpty(header.Value))
                {
                    Log.Error($"Header value for key {header.Key} is null or empty, skipping...", nameof(UnityHTTPService));
                    continue;
                }

                if (_defaultHeaders.ContainsKey(header.Key))
                {
                    _defaultHeaders[header.Key] = header.Value;
                }
                else
                {
                    _defaultHeaders.Add(header.Key, header.Value);
                }
            }
        }

        /// <summary>
        /// Clear default headers that were previously added
        /// </summary>
        public void ClearDefaultHeaders()
        {
            _defaultHeaders.Clear();
        }

        #region HTTP API

        /// <summary>
        /// Send a <paramref name="method"/> web request to <paramref name="url"/>
        /// </summary>
        /// <param name="url">Url address</param>
        /// <param name="method">HTTP method (i.e. Get, Post, Put...)</param>
        /// <param name="data">Optional data to add to the request</param>
        /// <param name="encoding">Data encoding to use (defaults to <see cref="Encoding.UTF8"/></param>
        /// <param name="contentType">Data's content type</param>
        /// <param name="callback">Called when there's a successful response from the request</param>
        /// <param name="headers">Headers to include the request (on top of defaults)</param>
        public void SendRequest(string url, string method, string data = "", Encoding? encoding = null, string contentType = "", Action<string>? callback = null, params KeyValuePair<string, string>[] headers)
        {
            UnityWebRequest? request = CreateRequest(url, method, data, encoding, contentType, headers);
            if (request == null) return;

            SendRequest(request, callback);
        }

        /// <summary>
        /// Send an unity web request
        /// </summary>
        /// <param name="request">Request to send</param>
        /// <param name="callback">Called when there's a successful response from the request</param>
        public void SendRequest(UnityWebRequest request, Action<string>? callback = null)
        {
            if (_mono == null)
            {
                Log.Error($"Failed to send request, mono is null.", nameof(UnityHTTPService));
                return;
            }

            _mono.StartCoroutine(SendRequestCoroutine(request, callback));
        }

        /// <summary>
        /// Send a <paramref name="method"/> web request to <paramref name="url"/>
        /// </summary>
        /// <param name="url">Url address</param>
        /// <param name="method">HTTP method (i.e. Get, Post, Put...)</param>
        /// <param name="data">Optional data to add to the request</param>
        /// <param name="encoding">Data encoding to use (defaults to <see cref="Encoding.UTF8"/></param>
        /// <param name="contentType">Data's content type</param>
        /// <param name="callback">Called when there's a successful response from the request</param>
        /// <param name="headers">Headers to include the request (on top of defaults)</param>
        public IEnumerator SendRequestCoroutine(string url, string method, string data = "", Encoding? encoding = null, string contentType = "", Action<string>? callback = null, params KeyValuePair<string, string>[] headers)
        {
            UnityWebRequest? request = CreateRequest(url, method, data, encoding, contentType, headers);
            if (request == null) yield break;

            yield return SendRequestCoroutine(request);
        }

        /// <summary>
        /// Send an unity web request
        /// </summary>
        /// <param name="request">Request to send</param>
        /// <param name="callback">Called when there's a successful response from the request</param>
        public IEnumerator SendRequestCoroutine(UnityWebRequest request, Action<string>? callback = null)
        {
            if (!ValidateRequest(request)) yield break;

            Log.Debug($"Sending a {request.method} request to {request.uri}...", nameof(UnityHTTPService));
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                if (string.IsNullOrEmpty(response))
                {
                    Log.Warning($"{request.method} response from \"{request.uri}\" is null or empty.", nameof(UnityHTTPService));
                    yield break;
                }

                callback?.Invoke(response);
            }
            else
            {
                Log.Error($"Failed to send {request.method} request, received {request.result} ({request.responseCode}).", nameof(UnityHTTPService));
            }
        }

        #endregion

        private UnityWebRequest? CreateRequest(string url, string method, string data = "", Encoding? encoding = null, string contentType = "", params KeyValuePair<string, string>[] headers)
        {
            try
            {
                // Create request
                UnityWebRequest request = new UnityWebRequest(url, method)
                {
                    downloadHandler = new DownloadHandlerBuffer()
                };

                // Add upload data
                if (!string.IsNullOrEmpty(data))
                {
                    if (string.IsNullOrEmpty(contentType))
                    {
                        Log.Error($"Failed to create unity web request, found data but no content type was specified.", nameof(UnityHTTPService));
                        return null;
                    }

                    byte[] bytes = (encoding ?? Encoding.UTF8).GetBytes(data);
                    request.uploadHandler = new UploadHandlerRaw(bytes);
                    request.uploadHandler.contentType = contentType;
                }

                // Add default headers to request
                if (_defaultHeaders.Any())
                {
                    headers = (KeyValuePair<string, string>[]) _defaultHeaders.Concat(headers);
                }

                // Add headers to request
                foreach (KeyValuePair<string, string> header in headers)
                {
                    if (string.IsNullOrEmpty(header.Key))
                    {
                        Log.Error($"Header key is null or empty, skipping...", nameof(UnityHTTPService));
                        continue;
                    }

                    if (string.IsNullOrEmpty(header.Value))
                    {
                        Log.Error($"Header value for key {header.Key} is null or empty, skipping...", nameof(UnityHTTPService));
                        continue;
                    }

                    request.SetRequestHeader(header.Key, header.Value);
                }

                return request;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to create unity web request. Exception: {ex}.", nameof(UnityHTTPService));
                return null;
            }
        }

        private bool ValidateRequest(UnityWebRequest request)
        {
            if (request == null)
            {
                Log.Error($"Failed to send request, request is null.", nameof(UnityHTTPService));
                return false;
            }

            if (request.uri == null)
            {
                Log.Error($"Failed to send request, request's uri is null.", nameof(UnityHTTPService));
                return false;
            }

            if (string.IsNullOrEmpty(request.method))
            {
                Log.Error($"Failed to send request, request's method is null or empty.", nameof(UnityHTTPService));
                return false;
            }

            return true;
        }
    }
}
