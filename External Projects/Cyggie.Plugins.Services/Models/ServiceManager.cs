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

        /// <summary>
        /// List of active services
        /// </summary>
        public readonly List<IService> Services = new List<IService>();

        /// <summary>
        /// Generic <see cref="IService"/> types used by the Service Manager to assign configuration when creating services
        /// </summary>
        public readonly List<Type> GenericServiceTypes = new List<Type>()
        {
            typeof(Service<>)
        };

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
        /// Initialize the service manager with all services and service configurations in the assembly
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
            {
                Log.Error("Trying to initialize service manager, but it is already initialized. Use Create to create new services after initialization.", nameof(ServiceManager));
                return;
            }

            IEnumerable<Type> serviceTypes = TypeHelper.GetAllIsAssignableFrom<IService>();
            Initialize(serviceTypes.ToArray());
        }

        /// <summary>
        /// Initialize the service manager with all service configurations and specific list of services
        /// </summary>
        /// <param name="serviceTypes"></param>
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

            Initialize(serviceConfigs, serviceTypes);
        }

        /// <summary>
        /// Initialize the service manager with a specific list of services and service configurations
        /// </summary>
        /// <param name="configs">List of configurations to use</param>
        /// <param name="serviceTypes">List of services to create</param>
        public void Initialize(IEnumerable<IServiceConfiguration?> configs, params Type[] serviceTypes)
        {
            if (_initialized)
            {
                Log.Error("Trying to initialize service manager, but it is already initialized. Use Create to create new services after initialization.", nameof(ServiceManager));
                return;
            }

            foreach (IServiceConfiguration? config in configs)
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
                    Services.Add(service);
                }
            }

            // Initialize all services in order of priority
            foreach (IService service in Services.OrderByDescending(x => x.Priority))
            {
                service.Initialize(this);
            }

            _initialized = true;
            Log.Debug($"Service Manager initialized all services: {Services.Count}.", nameof(ServiceManager));

            Services.ForEach(x => x.OnAllServicesInitialized());
        }

        /// <summary>
        /// Create a service of type <see cref="IService"/>
        /// </summary>
        /// <param name="serviceType">Type that implements <see cref="IService"/></param>
        public static IService? Create(Type serviceType) => Create(serviceType, initialize: true);

        /// <summary>
        /// Create a service of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of IService to create</typeparam>
#pragma warning disable CS8603 // Possible null reference return.
        public static T Create<T>() where T : IService => (T) Create(typeof(T), initialize: true);
#pragma warning restore CS8603 // Possible null reference return.

        private static IService? Create(Type serviceType, bool initialize)
        {
            if (serviceType.IsAssignableFrom(typeof(IService)) && !serviceType.IsAbstract)
            {
                Log.Error($"Unable to create service of type {serviceType}, it is not assignable from {typeof(IService)} or it is abstract.", nameof(ServiceManager));
                return null;
            }

            if (Instance.Services.Any(x => x.GetType() == serviceType))
            {
                Log.Error($"Unable to create service of type {serviceType}, service already exists within the list!", nameof(ServiceManager));
                return null;
            }

            IService? iService = (IService?) Activator.CreateInstance(serviceType);
            if (iService != null)
            {
                if (Instance.GenericServiceTypes.Any(type => iService.GetType().IsSubclassOfGenericType(type)))
                {
                    if (!(iService is IServiceWithConfiguration serviceWithConfig))
                    {
                        Log.Error($"Unable to convert service to a service with configuration {typeof(IServiceConfiguration)}", nameof(ServiceManager));
                        return null;
                    }

                    IServiceConfiguration? serviceConfiguration = Instance._serviceConfigurations.FirstOrDefault(x =>
                    {
                        return serviceWithConfig.ConfigurationType.IsAssignableFrom(x.GetType());
                    });

                    if (serviceConfiguration != null)
                    {
                        serviceWithConfig.SetConfiguration(serviceConfiguration);
                    }
                }

                if (initialize)
                {
                    iService.Initialize(Instance);
                    Instance.Services.Add(iService);
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
        /// <param name="autoCreate">Auto create the service when it is not found</param>
        /// <param name="isAssignableFrom">When true, it checks whether the service is assignable from <typeparamref name="T"/></param>
        /// <returns>Service (null if not found)</returns>
        public static T Get<T>(bool autoCreate = true, bool isAssignableFrom = true) where T : IService
        {
#pragma warning disable CS8603 // Possible null reference return.
            // Not null is only supported in C# 9+ which is not supported in most Unity versions
            IService? service = (T) Instance.Services.FirstOrDefault(x => isAssignableFrom ? typeof(T).IsAssignableFrom(x.GetType()) : x.GetType() == typeof(T));
            if (service == null)
            {
                if (autoCreate)
                {
                    service = Create(typeof(T));
                }
                else
                {
                    Log.Error($"Unable to get service {typeof(T)}, make sure it is initialized with the service manager or use {nameof(autoCreate)}.", nameof(ServiceManager));
                }
            }

            return (T) service;
#pragma warning restore CS8603 // Possible null reference return.
        }

        /// <summary>
        /// Try get a service from the service manager
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="service">Output service (null if not found)</param>
        /// <param name="isAssignableFrom">When true, it checks whether the service is assignable from <typeparamref name="T"/></param>
        /// <returns>Found?</returns>
        public static bool TryGet<T>(out T service, bool isAssignableFrom = false) where T : IService
        {
            service = Get<T>(isAssignableFrom);
            return service != null;
        }
    }
}
