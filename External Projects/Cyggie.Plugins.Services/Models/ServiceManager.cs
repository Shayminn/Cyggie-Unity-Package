using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Interfaces;
using Cyggie.Plugins.Utils.Extensions;
using Cyggie.Plugins.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cyggie.Plugins.Services.Models
{
    /// <summary>
    /// Service manager class (singleton for all singletons)
    /// </summary>
    public class ServiceManager : IServiceManager
    {
        private static ServiceManager? _instance = null;
        private static ServiceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Log.Warning("Trying to access Service Manager instance but it has not been created yet. Creating it...", nameof(ServiceManager));
                    ServiceManager manager = new ServiceManager();
                    _instance = manager;
                }

                return _instance;
            }
        }

        private readonly List<IServiceConfiguration> _serviceConfigurations = new List<IServiceConfiguration>();
        private readonly List<IService> _services = new List<IService>();

        private bool _initialized = false;

        #region Initialization

        /// <summary>
        /// Create a new service manager object
        /// </summary>
        /// <param name="initialize">Whether the service manager should immediately initialize</param>
        public ServiceManager(bool initialize = false)
        {
            if (_instance != null)
            {
                Log.Error("Creating a service manager while one already exists! Overwriting the previous one.", nameof(ServiceManager));
            }

            _instance = this;
            if (initialize)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Initialize the service manager with all services in the assembly
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
            {
                Log.Error("Initialize the service manager but it was already initialized!", nameof(ServiceManager));
                return;
            }

            IEnumerable<Type> serviceTypes = TypeHelper.GetAllIsAssignableFrom<IService>();
            Initialize(serviceTypes.ToArray());
        }

        /// <inheritdoc/>
        public void Initialize(params Type[] serviceTypes)
        {
            if (_initialized)
            {
                Log.Error("Trying to initialize service manager, but it is already initialized. Use Create to create new services after initialization.", nameof(ServiceManager));
                return;
            }

            // Create all service configuration objects
            IEnumerable<Type> serviceConfigurationTypes = TypeHelper.GetAllIsAssignableFrom<IServiceConfiguration>();

            // Filter service configurations so only a single one is assigned to each service
            IEnumerable<IServiceConfiguration?> serviceConfigs = serviceConfigurationTypes.Select(type =>
            {
                IServiceConfiguration? config = null;
                try
                {
                    config = (IServiceConfiguration?) Activator.CreateInstance(type);
                }
                catch (TargetInvocationException)
                {
                    // Do nothing
                    // This can happen when creating a service configuration that inherits from ScriptableObject
                    // The configuration will be null
                }
                catch (Exception ex)
                {
                    Log.Error($"Unable to create service configuration of type {type}, exception ({ex.GetType()}): {ex}.", nameof(ServiceManager));
                }

                return config;
            });

            foreach (IServiceConfiguration? config in serviceConfigs)
            {
                if (config == null) continue;
                _serviceConfigurations.Add(config);
            }

            Log.Debug($"Service Manager initializing {serviceTypes.Length} services.", nameof(ServiceManager));
            if (serviceTypes.Length == 0)
            {
                _initialized = true;
                return;
            }

            // Create all services
            foreach (Type serviceType in serviceTypes)
            {
                // Create only the services without initializing them
                IService? service = Create(serviceType, initialize: false);
                if (service == null) continue;

                // Check if the service needs to be initialized (or will it be created/initialized later on)
                if (service.CreateOnInitialize)
                {
                    _services.Add(service);
                }
            }

            // Initialize all services in order of priority
            foreach (IService service in _services.OrderByDescending(x => x.Priority))
            {
                service.Initialize(this);
            }

            _initialized = true;
            Log.Debug($"Service Manager initialized all services: {_services.Count}.", nameof(ServiceManager));

            _services.ForEach(x => x.OnAllServicesInitialized());
        }

        /// <summary>
        /// Create a service of type <see cref="IService"/>
        /// </summary>
        /// <param name="serviceType">Type that implements <see cref="IService"/></param>
        public static IService? Create(Type serviceType) => Create(serviceType, initialize: true);

        private static IService? Create(Type serviceType, bool initialize)
        {
            if (serviceType.IsAssignableFrom(typeof(IService)) && !serviceType.IsAbstract)
            {
                Log.Error($"Unable to create service of type {serviceType}, it is not assignable from {typeof(IService)} or it is abstract.", nameof(ServiceManager));
                return null;
            }

            if (Instance._services.Any(x => x.GetType() == serviceType))
            {
                Log.Error($"Unable to create service of type {serviceType}, service already exists within the list!", nameof(ServiceManager));
                return null;
            }

            IService? iService = (IService?) Activator.CreateInstance(serviceType);
            if (iService != null)
            {
                if (iService.GetType().IsSubclassOfGenericType(typeof(Service<>), out Type? baseServiceType) && baseServiceType != null)
                {
                    if (!(iService is IServiceWithConfiguration serviceWithConfig))
                    {
                        Log.Error($"Unable to convert service to a service with configuration {typeof(IServiceConfiguration)}", nameof(ServiceManager));
                        return null;
                    }

                    IServiceConfiguration? serviceConfiguration = Instance._serviceConfigurations.FirstOrDefault(x =>
                    {
                        return serviceWithConfig.ConfigurationType == x.GetType();
                    });

                    if (serviceConfiguration != null)
                    {
                        serviceWithConfig.SetConfiguration(serviceConfiguration);
                    }
                }

                if (initialize)
                {
                    iService.Initialize(Instance);
                    Instance._services.Add(iService);
                }

                return iService;
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Get a service from the service manager <br/>
        /// Will automatically create it if not found
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>Service object (null if creation was unsuccessful)</returns>
        public static T Get<T>() where T : IService
        {
            T service = (T) Instance._services.FirstOrDefault(x => x.GetType() == typeof(T)) ?? (T) Create(typeof(T));
#pragma warning disable CS8603 // Possible null reference return.
            // Not null is only supported in C# 9+ which is not supported in most Unity versions
            return service;
#pragma warning restore CS8603 // Possible null reference return.
        }

        /// <summary>
        /// Try get a service from the service manager
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="service">Output service (null if not found)</param>
        /// <returns>Found?</returns>
        public static bool TryGet<T>(out T service) where T : IService
        {
            service = (T) Instance._services.FirstOrDefault(x => x.GetType() == typeof(T));
            return service != null;
        }
    }
}
