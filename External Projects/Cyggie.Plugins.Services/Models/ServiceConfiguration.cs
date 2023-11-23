using Cyggie.Plugins.Services.Interfaces;

namespace Cyggie.Plugins.Services.Models
{
    /// <summary>
    /// Service configuration for a service type of <see cref="IService"/>
    /// </summary>
    /// <typeparam name="T">Service type</typeparam>
    public abstract class ServiceConfiguration : IServiceConfiguration
    {
    }
}
