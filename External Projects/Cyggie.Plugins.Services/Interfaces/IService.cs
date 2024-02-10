namespace Cyggie.Plugins.Services.Interfaces
{
    /// <summary>
    /// Interface for a singular service (singleton instance)
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Reference to the service manager that manages all services
        /// </summary>
        IServiceManager? ServiceManager { get; set; }

        /// <summary>
        /// Priority of initialization (1 being higher than 0)
        /// </summary>
        int Priority => -1;

        /// <summary>
        /// Whether this service should be created when <see cref="IServiceManager.Initialize(System.Type[])"/> is called
        /// </summary>
        bool CreateOnInitialize => true;

        /// <summary>
        /// Initialize the service
        /// </summary>
        /// <param name="manager">Reference to the service manager that initializes and creates this service</param>
        void Initialize(IServiceManager manager);

        /// <summary>
        /// Called when all services have been initialized
        /// </summary>
        void OnAllServicesInitialized();
    }
}
