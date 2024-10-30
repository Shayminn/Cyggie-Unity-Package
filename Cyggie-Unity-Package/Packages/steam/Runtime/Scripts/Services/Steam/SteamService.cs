using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Plugins.Logs;
using Steamworks;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Cyggie.Steam.Runtime.Services
{
    /// <summary>
    /// Service that manages the Steam API
    /// </summary>
    public partial class SteamService : ServiceMono<SteamServiceConfiguration>
    {
        #region Mono implementations

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            try
            {
                SteamClient.Init(480);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to initialize steam client, exception: {ex}.", nameof(SteamService));
            }
        }

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (Configuration.EnableDebugCallbacks)
            {
                Steamworks.Dispatch.OnDebugCallback += OnDebugCallback;
            }

            Steamworks.Dispatch.OnException += OnException;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (Configuration.EnableDebugCallbacks)
            {
                Steamworks.Dispatch.OnDebugCallback -= OnDebugCallback;
            }

            Steamworks.Dispatch.OnException -= OnException;
        }

        /// <inheritdoc/>
        protected override void Update()
        {
            base.Update();

            // Run any pending Steam callbacks
            SteamClient.RunCallbacks();
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            AuthSessionTicket.Cancel();
            SteamClient.Shutdown();
        }

        #endregion

        #region Public API

        /// <summary>
        /// Get the currently connected steam user's avatar
        /// </summary>
        /// <returns>Steam avatar in Texture 2D</returns>
        public async Task<Texture2D> GetAvatar()
        {
            try
            {
                // Get Avatar using await
                Steamworks.Data.Image? avatar = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
                if (avatar == null)
                {
                    Log.Error($"Failed to get avatar, object was null.", nameof(SteamService));
                    return null;
                }

                Texture2D texture = new Texture2D((int) avatar.Value.Width, (int) avatar.Value.Height, TextureFormat.ARGB32, false);
                texture.filterMode = FilterMode.Trilinear;

                // Flip image
                for (int x = 0; x < avatar.Value.Width; x++)
                {
                    for (int y = 0; y < avatar.Value.Height; y++)
                    {
                        var p = avatar.Value.GetPixel(x, y);
                        texture.SetPixel(x, (int) avatar.Value.Height - y, new UnityEngine.Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
                    }
                }

                texture.Apply();
                return texture;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get avatar, unhandled exception: {ex}.", nameof(SteamService));
                return null;
            }
        }

        #endregion

        #region Event handlers

        private void OnDebugCallback(CallbackType type, string message, bool b)
        {
            Log.Debug($"[Steam Debug Callback]: {message} ({type} | {b}).", nameof(SteamService));
        }

        private void OnException(Exception ex)
        {
            Log.Error($"[Steam Exception]: {ex}.", nameof(SteamService));
        }

        #endregion
    }
}
