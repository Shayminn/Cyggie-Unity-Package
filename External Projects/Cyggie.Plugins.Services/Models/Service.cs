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

        /// <summary>
        /// The order of priority in which the service is initialized (by default 0) <br/>
        /// Higher value means this service will be initialized before other services.
        /// </summary>
        protected virtual int Priority => 0;

        /// <inheritdoc/>
        protected virtual bool CreateOnInitialize => true;

        /// <summary>
        /// Whether the service has been initialized or not yet
        /// </summary>
        protected internal bool _initialized = false;

        /// <inheritdoc/>
        public virtual void Initialize(IServiceManager manager)
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

        /// <inheritdoc/>
        public virtual void OnAllServicesInitialized() { }
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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        // Not null is only supported in C# 9+ which is not supported in most Unity versions
        public T Configuration { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <inheritdoc/>
        public Type ConfigurationType => typeof(T);

        /// <inheritdoc/>
        public void SetConfiguration<TConfig>(TConfig config)
            where TConfig : IServiceConfiguration
        {
            Configuration = (T) (IServiceConfiguration) config;
        }

        /// <inheritdoc/>
        public override void Initialize(IServiceManager manager)
        {
            if (_initialized)
            {
                Log.Error("Trying to initialize a service, but it's already initialized!", nameof(Service));
                return;
            }

            ServiceManager = manager;
            OnInitialized();

            Log.Debug($"Service ({GetType()}) has been initialized with configuration ({(Configuration == null ? "none" : Configuration.GetType().ToString())}).", nameof(Service));    
            _initialized = true;
        }
    }

    #endregion
}
