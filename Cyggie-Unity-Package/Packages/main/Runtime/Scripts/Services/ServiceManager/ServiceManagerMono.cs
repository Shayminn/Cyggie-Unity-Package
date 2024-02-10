using Cyggie.Main.Runtime.Attributes;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.Utils.Helpers;
using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Interfaces;
using Cyggie.Plugins.Services.Models;
using Cyggie.Plugins.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cyggie.Main.Runtime.ServicesNS
{

    /// <summary>
    /// Manager class for all services of type <see cref="ServiceMono"/> <br/> 
    /// Any subclass of type <see cref="ServiceMono"/> will automatically be created and initialized <br/>
    /// It's a singleton of singletons
    /// </summary>
    public class ServiceManagerMono : MonoBehaviour, IServiceManager
    {
        /// <summary>
        /// Settings saved in the Resources folder, managed in Project Settings
        /// </summary>
        internal static ServiceManagerSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = Resources.Load<ServiceManagerSettings>(ServiceManagerSettings.cResourcesPath);
                }

                return _settings;
            }
        }

        private static ServiceManagerMono _instance = null;
        private static ServiceManagerSettings _settings = null;

        private readonly List<IService> _services = new List<IService>();

        private bool _initialized = false;

        #region Public Properties

        /// <summary>
        /// The service manager's game object in the scene
        /// </summary>
        public static GameObject GameObject => _instance.gameObject;

        #endregion

        #region Interface implementation 

        /// <inheritdoc/>
        public void Initialize(params Type[] serviceTypes)
        {
            if (_initialized)
            {
                Log.Error("Initialize the service manager but it was already initialized!", nameof(ServiceManager));
                return;
            }

            Log.Debug($"Service Manager initializing {serviceTypes.Length} services.", nameof(ServiceManager));
            foreach (Type type in serviceTypes)
            {
                IService service = Create(type, initialize: false);

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
            Log.Debug($"Service Manager initialized all services: {_services.Count}.", nameof(ServiceManagerMono));

            _services.ForEach(x => x.OnAllServicesInitialized());
        }

        /// <inheritdoc/>
        public static IService Create(Type serviceType) => Create(serviceType, true);

        private static IService Create(Type serviceType, bool initialize)
        {
            if (initialize && (_instance == null || !_instance._initialized))
            {
                Log.Error($"Unable to create service of type {serviceType}, service manager was not initialized yet.", nameof(ServiceManagerMono));
                return null;
            }

            if (serviceType.IsAssignableFrom(typeof(IService)) && !serviceType.IsAbstract)
            {
                Log.Error($"Unable to create service of type {serviceType}, it is not assignable from {typeof(IService)} or it is abstract.", nameof(ServiceManager));
                return null;
            }

            if (_instance._services.Any(x => x.GetType() == serviceType))
            {
                Log.Error($"Unable to create service of type {serviceType}, service already exists within the list!", nameof(ServiceManager));
                return null;
            }

            IService iService = (IService) Activator.CreateInstance(serviceType);
            if (iService != null)
            {
                bool serviceWithConfiguration = iService.GetType().IsSubclassOfGenericType(typeof(Service<>)) ||
                                                iService.GetType().IsSubclassOfGenericType(typeof(ServiceMono<>));
                if (serviceWithConfiguration)
                {
                    if (iService is not IServiceWithConfiguration serviceWithConfig)
                    {
                        Log.Error($"Unable to convert service to a service with configuration {typeof(IServiceConfiguration)}", nameof(ServiceManager));
                        return null;
                    }

                    IServiceConfiguration serviceConfiguration = _settings.ServiceConfigurations.FirstOrDefault(x =>
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
                    iService.Initialize(_instance);
                    _instance._services.Add(iService);
                }

                return iService;
            }

            Log.Error($"Failed to create service of type {serviceType}.", nameof(ServiceManager));
            return null;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor to set up Log system
        /// </summary>
        static ServiceManagerMono()
        {
            Log.SetLogModel<LogUnity>();

            // Any logs called in a static constructor will always be sent since the settings EnabledLogs has not been retrieved yet
        }

        /// <summary>
        /// Called at the start of runtime before awake <br/>
        /// Create the <see cref="ServiceManagerMono"/> object prefab in the scene
        /// </summary>
        [CustomRuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (Settings == null)
            {
                Log.Error($"Unable to find Service Manager Settings in Resources.", nameof(ServiceManagerMono));
                return;
            }

            if (Settings.Prefab == null || Settings.EmptyPrefab == null)
            {
                Log.Error($"Unable to find Settings' Prefab and/or Empty Prefab. Reopen the configuration window (Alt + C) and they will be auto-assigned.", nameof(ServiceManagerMono));
                return;
            }

            // Enable/Disable logs
            Log.SetLogModel<LogUnity>(); // set it again just in case
            Log.ToggleLogs(Settings.EnabledLogs);

            // Set helpers
            GameObjectHelper.EmptyPrefab = Settings.EmptyPrefab;

            // Create service manager object
            Instantiate(Settings.Prefab);

            // Initialize will be called in Awake
        }

        #endregion

        /// <summary>
        /// Get a service from the service manager
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>Service (null if not found)</returns>
        public static T Get<T>() where T : IService
        {
            if (!TryGetInstance(out IServiceManager instance) || _instance == null) return default;
            return (T) _instance._services.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        /// <summary>
        /// Try get a service from the service manager
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="service">Output service (null if not found)</param>
        /// <returns>Found?</returns>
        public static bool TryGet<T>(out T service) where T : IService
        {
            service = Get<T>();
            return service != null;
        }

        private static bool TryGetInstance(out IServiceManager instance)
        {
            instance = _instance;
            if (instance == null)
            {
                Log.Error("Trying to access Service Manager instance but it has not been created yet.", nameof(ServiceManagerMono));
                return false;
            }

            return true;
        }

        #region MonoBehaviour implementations

        private void OnGUI()
        {
            _services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.OnGUIInternal();
                }
            });
        }

        private void Awake()
        {
            if (_instance != null) return;

            _instance = this;
            DontDestroyOnLoad(_instance);

            IEnumerable<Type> types = Settings.ServiceIdentifiers.Select(x => Type.GetType(x.AssemblyName));
            _instance.Initialize(types.ToArray());

            _services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.AwakeInternal();
                }
            });
        }

        private void OnEnable()
        {
            _services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.OnEnableInternal();
                }
            });
        }

        private void Start()
        {
            _services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.StartInternal();
                }
            });
        }

        private void OnDisable()
        {
            _services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.OnDisableInternal();
                }
            });
        }

        private void Update()
        {
            _services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.UpdateInternal();
                }
            });
        }

        private void FixedUpdate()
        {
            _services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.FixedUpdateInternal();
                }
            });
        }

        private void OnDestroy()
        {
            _services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.OnDestroyInternal();
                }
            });
        }

        #endregion
    }
}
