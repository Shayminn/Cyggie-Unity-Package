#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

using UnityEngine;

#if !DISABLESTEAMWORKS
using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Plugins.Logs;
using Steamworks;
#endif

namespace Cyggie.Steam.Runtime.Services
{
    /// <summary>
    /// Service that manages the Steam API <br/>
    /// Based on https://raw.githubusercontent.com/rlabrecque/SteamManager/master/SteamManager.cs
    /// </summary>
    public partial class SteamService : ServiceMono
    {
#if !DISABELSTEAMWORKS
        private bool _steamInitialized = false;

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            try
            {
                // If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the
                // Steam client and also launches this game again if the User owns it. This can act as a rudimentary form of DRM.

                // Once you get a Steam AppID assigned by Valve, you need to replace AppId_t.Invalid with it and
                // remove steam_appid.txt from the game depot. eg: "(AppId_t)480" or "new AppId_t(480)".
                // See the Valve documentation for more information: https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
                if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
                {
                    Application.Quit();
                    return;
                }
            }
            catch (System.DllNotFoundException)
            {
                // We catch this exception here, as it will be the first occurrence of it.
                Log.Error($"Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\\n\"", nameof(SteamService));

                Application.Quit();
                return;
            }

            Log.Debug($"Initializing steam API...", nameof(SteamService));

            // Initializes the Steamworks API.
            // If this returns false then this indicates one of the following conditions:
            // [*] The Steam client isn't running. A running Steam client is required to provide implementations of the various Steamworks interfaces.
            // [*] The Steam client couldn't determine the App ID of game. If you're running your application from the executable or debugger directly then you must have a [code-inline]steam_appid.txt[/code-inline] in your game directory next to the executable, with your app ID in it and nothing else. Steam will look for this file in the current working directory. If you are running your executable from a different directory you may need to relocate the [code-inline]steam_appid.txt[/code-inline] file.
            // [*] Your application is not running under the same OS user context as the Steam client, such as a different user or administration access level.
            // [*] Ensure that you own a license for the App ID on the currently active Steam account. Your game must show up in your Steam library.
            // [*] Your App ID is not completely set up, i.e. in Release State: Unavailable, or it's missing default packages.
            // Valve's documentation for this is located here:
            // https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
            _steamInitialized = SteamAPI.Init();
            if (!_steamInitialized)
            {
                Log.Error("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", nameof(SteamService));

                return;
            }

            Log.Debug($"Steam API initialized.", nameof(SteamService));
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            SteamAPI.Shutdown();
        }

        /// <inheritdoc/>
        protected override void Update()
        {
            base.Update();

            // Run Steam client callbacks
            SteamAPI.RunCallbacks();
        }

#endif
    }
}
