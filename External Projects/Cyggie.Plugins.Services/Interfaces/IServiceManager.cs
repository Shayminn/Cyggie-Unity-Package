using System;

namespace Cyggie.Plugins.Services.Interfaces
{
    /// <summary>
    /// Interface of a service manager that initializes and manages all services
    /// </summary>
    public interface IServiceManager
    {
        /// <summary>
        /// Delegate event for all services being initialized
        /// </summary>
        public delegate void OnServicesInitializedEvent();

        /// <summary>
        /// Event called when all services have been initialized
        /// </summary>
        public static OnServicesInitializedEvent? OnServicesInitialized;

        /// <summary>
        /// Create and initialize an array of services at the start of the application
        /// </summary>
        void Initialize(params Type[] services);
    }
}
