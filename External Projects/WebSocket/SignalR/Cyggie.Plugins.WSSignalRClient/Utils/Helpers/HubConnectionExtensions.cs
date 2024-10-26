using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Cyggie.Plugins.SignalR.Utils.Helpers
{
    /// <summary>
    /// Extensions for <see cref="HubConnection"/>
    /// </summary>
    internal static class HubConnectionExtensions
    {
        /// <summary>
        /// Registers a handler that will be invoked when the hub method with the specified method name is invoked
        /// </summary>
        /// <param name="hubConnection">Hub connection to register a handler for</param>
        /// <param name="methodName">Method name to register</param>
        /// <param name="parameterTypes">Array of type arguments that the method has</param>
        /// <param name="handler">Handler/callback when the method is called</param>
        /// <returns></returns>
        internal static IDisposable On(this HubConnection hubConnection, string methodName, Type[] parameterTypes, Action<object?[]> handler)
        {
            return hubConnection.On(methodName, parameterTypes, (parameters, state) =>
            {
                var currentHandler = (Action<object?[]>) state;
                currentHandler(parameters);
                return Task.CompletedTask;
            }, handler);
        }
    }
}
