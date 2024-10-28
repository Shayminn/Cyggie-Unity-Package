using Cyggie.Plugins.HTTP.Utils;
using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Cyggie.Plugins.HTTP.Services
{
    /// <summary>
    /// Service that simplifies sending Http requests
    /// </summary>
    public class HTTPService : Service
    {
        private HttpClient _client = new HttpClient();

        /// <summary>
        /// Set the default request headers for the client
        /// </summary>
        /// <param name="headers">Collection of all headers to set</param>
        public void SetDefaultHeaders(params KeyValuePair<string, string>[] headers)
        {
            AddRequestHeaders(_client.DefaultRequestHeaders, headers);
        }

        /// <summary>
        /// Clear the default request headers for the client
        /// </summary>
        public void ClearDefaultHeaders()
        {
            _client.DefaultRequestHeaders.Clear();
        }

        /// <summary>
        /// Send a GET request to <paramref name="url"/> with <paramref name="headers"/>
        /// </summary>
        /// <param name="url">Url to send request to</param>
        /// <param name="headers">Headers added on top of the default headers to the request</param>
        /// <returns>Response message (empty if failed)</returns>
        public async Task<string> Get(string url, params KeyValuePair<string, string>[] headers)
        {
            // Create request
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            AddRequestHeaders(request.Headers, headers);
            return await Send(request);
        }

        /// <summary>
        /// Send a POST request to <paramref name="url"/> with <paramref name="headers"/>
        /// </summary>
        /// <param name="url">Url to send request to</param>
        /// <param name="text">Text to send</param>
        /// <param name="encoding">Text encoding</param>
        /// <param name="mediaType">Text media type</param>
        /// <param name="headers">Headers added on top of the default headers to the request</param>
        /// <returns>Response message (empty if failed)</returns>
        public async Task<string> Post(string url, string text, Encoding? encoding = null, string mediaType = HTTPMediaTypes.cText, params KeyValuePair<string, string>[] headers)
        {
            // Create request
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(text, encoding ?? Encoding.UTF8, mediaType)
            };

            AddRequestHeaders(request.Headers, headers);
            return await Send(request);
        }

        /// <summary>
        /// Send a PUT request to <paramref name="url"/> with <paramref name="headers"/>
        /// </summary>
        /// <param name="url">Url to send request to</param>
        /// <param name="text">Text to send</param>
        /// <param name="encoding">Text encoding</param>
        /// <param name="mediaType">Text media type</param>
        /// <param name="headers">Headers added on top of the default headers to the request</param>
        public async Task Put(string url, string text, Encoding? encoding = null, string mediaType = HTTPMediaTypes.cText, params KeyValuePair<string, string>[] headers)
        {
            // Create request
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url),
                Content = new StringContent(text, encoding ?? Encoding.UTF8, mediaType)
            };

            AddRequestHeaders(request.Headers, headers);
            await Send(request);
        }

        private async Task<string> Send(HttpRequestMessage request)
        {
            Log.Debug($"Sending a {request.Method.Method} request to {request.RequestUri}...", nameof(HTTPService));

            try
            {
                // Send request
                HttpResponseMessage response = await _client.SendAsync(request);

                // Check response from server
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string responseMessage = await response.Content.ReadAsStringAsync();
                    Log.Debug($"Received {request.Method.Method} response: {responseMessage}", nameof(HTTPService));

                    return responseMessage;
                }
                else
                {
                    Log.Error($"Failed on {request.Method.Method} request, received {response.StatusCode} ({(int) response.StatusCode}) from server.", nameof(HTTPService));
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed on {request.Method.Method} request, unhandled exception: {ex}.", nameof(HTTPService));
            }

            return string.Empty;
        }

        private void AddRequestHeaders(HttpRequestHeaders requestHeaders, params KeyValuePair<string, string>[] headers)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                if (string.IsNullOrEmpty(header.Key))
                {
                    Log.Error($"Header key is null or empty, skipping...", nameof(HTTPService));
                    continue;
                }

                if (string.IsNullOrEmpty(header.Value))
                {
                    Log.Error($"Header value for key {header.Key} is null or empty, skipping...", nameof(HTTPService));
                    continue;
                }

                requestHeaders.Add(header.Key, header.Value);
            }
        }
    }
}
