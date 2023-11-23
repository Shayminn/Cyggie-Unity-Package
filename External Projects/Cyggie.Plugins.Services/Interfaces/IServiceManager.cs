using System;

namespace Cyggie.Plugins.Services.Interfaces
{
    /// <summary>
    /// Interface of a service manager that initializes and manages all services
    /// </summary>
    public interface IServiceManager
    {
        /// <summary>
        /// Create and initialize an array of services at the start of the application
        /// </summary>
        void Initialize(params Type[] services);
    }
}
