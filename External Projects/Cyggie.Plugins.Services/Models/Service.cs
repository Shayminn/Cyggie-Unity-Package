using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Interfaces;
using System;

namespace Cyggie.Plugins.Services.Models
{
    #region Service

    /// <summary>
    /// Service (singleton) without a configuration
    /// </summary>
    public abstract class Service : IService
    {
        /// <inheritdoc/>
        public IServiceManager? ServiceManager { get; set; }

        /// <inheritdoc/>
        protected virtual int Priority => -1;

        /// <inheritdoc/>
        protected virtual bool CreateOnInitialize => true;

        private bool _initialized = false;

        /// <inheritdoc/>
        public void Initialize(IServiceManager manager)
        {
            if (_initialized)
            {
                Log.Error("Trying to initialize a service, but it's already initialized!", nameof(Service));
                return;
            }

            ServiceManager = manager;
            OnInitialized();

            Log.Debug($"Service ({GetType()}) has been initialized with no configuration.", nameof(Service));
            _initialized = true;
        }

        /// <summary>
        /// Called right after the service is initialized
        /// </summary>
        protected virtual void OnInitialized() { }
    }

    #endregion

    #region Service with configuration

    /// <summary>
    /// Service (singleton) with configuration of type <typeparamref name="T"/> 
    /// </summary>
    /// <typeparam name="T">Service configuration type</typeparam>
    public abstract class Service<T> : Service, IServiceWithConfiguration
        where T : IServiceConfiguration
    {
        /// <summary>
        /// Configuration associated to object
        /// </summary>
        public T Configuration { get; private set; }

        /// <inheritdoc/>
        public Type ConfigurationType => typeof(T);

        /// <inheritdoc/>
        public void SetConfiguration<TConfig>(TConfig config)
            where TConfig : IServiceConfiguration
        {
            Configuration = (T) (IServiceConfiguration) config;
        }
    }

    #endregion
}
