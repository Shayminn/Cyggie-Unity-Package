using System;

namespace Cyggie.Plugins.Services.Interfaces
{
    /// <summary>
    /// Interface for a service with configuration
    /// </summary>
    public interface IServiceWithConfiguration
    {
        /// <summary>
        /// Type of the configuration
        /// </summary>
        Type ConfigurationType { get; }

        /// <summary>
        /// Set the configuration for the service
        /// </summary>
        /// <typeparam name="T">Configuration type</typeparam>
        void SetConfiguration<T>(T config) where T : IServiceConfiguration;
    }
}
