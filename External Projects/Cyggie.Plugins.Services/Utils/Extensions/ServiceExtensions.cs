using Cyggie.Plugins.Services.Interfaces;
using Cyggie.Plugins.Services.Models;
using System;

namespace Cyggie.Plugins.Services.Utils.Extensions
{
    /// <summary>
    /// Class for extension methods related to Services
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Get the service configuration type from a <see cref="Service{T}"/>
        /// </summary>
        /// <typeparam name="T">Service configuration type</typeparam>
        /// <param name="service">Service with configuration</param>
        /// <returns></returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static Type GetConfigurationType<T>(this Service<T> service)
#pragma warning restore IDE0060 // Remove unused parameter
            where T : IServiceConfiguration
        {
            return typeof(T);
        }
    }
}
