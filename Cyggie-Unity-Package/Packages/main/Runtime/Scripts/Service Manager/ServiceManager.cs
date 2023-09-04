using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.Utils.Helpers;
using Cyggie.Plugins.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cyggie.Main.Runtime.ServicesNS
{
    /// <summary>
    /// Manager class for all services of type <see cref="Service"/> <br/> 
    /// Any subclass of type <see cref="Service"/> will automatically be created and initialized <br/>
    /// It's a singleton of singletons
    /// </summary>
    public class ServiceManager : MonoBehaviour
    {
        /// <summary>
        /// Action when all services have been created <br/>
        /// This is called before <see cref="Service.Awake"/>
        /// </summary>
        public static Action OnServicesInitialized = null;

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

        private static ServiceManager _instance = null;
        private static ServiceManagerSettings _settings = null;

        private static readonly List<Service> _services = new List<Service>();

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public static GameObject GameObject => _instance.gameObject;

        #endregion

        #region Initialization

        /// <summary>
        /// Called at the start of runtime before awake <br/>
        /// Create the <see cref="ServiceManager"/> object prefab in the scene
        /// </summary>
        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (Settings == null)
            {
                Log.Error($"Unable to find Service Manager Settings in Resources.", nameof(ServiceManager));
                return;
            }

            if (Settings.EnableLog)
            {
                Log.Enable();
            }
            else
            {
                Log.Disable();
            }

            // Create service manager object
            Instantiate(Settings.Prefab);
        }

        /// <summary>
        /// Create instance of all services <br/>
        /// </summary>
        private void InitializeServices()
        {
            IEnumerable<Type> servicesTypes = TypeHelper.GetAllSubclassTypes<Service>(true);

            // Create all the services
            IEnumerable<Service> createdServices = servicesTypes.Select(type => (Service) Activator.CreateInstance(type));

            foreach (Service service in createdServices.OrderByDescending(x => x.InternalPriority))
            {
                // Get the service's configuration and assign it if any
                ServiceConfigurationSO configuration = Settings.ServiceConfigurations.FirstOrDefault(x => x.ServiceType == service.GetType());
                if (configuration != null)
                {
                    service.SetConfigurationSettings(configuration);
                }

                if (!service.CallShouldInitialize()) continue;

                service.Initialize(this);
                _services.Add(service);
            }

            Log.Debug($"Initialized services. Count: {_services.Count}.", nameof(ServiceManager));
            OnServicesInitialized?.Invoke();
        }

        #endregion

        /// <summary>
        /// Get a service of a specific type
        /// </summary>
        /// <typeparam name="T">Type of service</typeparam>
        /// <returns>Stored service (null if not found)</returns>
        public static T Get<T>() where T : Service
        {
            Service service = _services.FirstOrDefault(x => x.GetType() == typeof(T));

            if (service == null)
            {
                Log.Error($"Unable to get service {typeof(T)}. Make sure the service is initialized before calling Get; use {nameof(OnServicesInitialized)} or call Get in {nameof(Start)}.", nameof(ServiceManager));
            }

            return (T) service;
        }

        #region MonoBehaviour implementations

        private void OnGUI()
        {
            _services.ForEach(x => x.OnGUIInternal());
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(_instance);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeServices();

            _services.ForEach(x => x.AwakeInternal());
        }

        private void OnEnable()
        {
            _services.ForEach(x => x.OnEnableInternal());
        }

        private void Start()
        {
            _services.ForEach(x => x.StartInternal());
        }

        private void OnDisable()
        {
            _services.ForEach(x => x.OnDisableInternal());
        }

        private void Update()
        {
            _services.ForEach(x => x.UpdateInternal());
        }

        private void FixedUpdate()
        {
            _services.ForEach(x => x.FixedUpdateInternal());
        }

        private void OnDestroy()
        {
            _services.ForEach(x => x.OnDestroyInternal());
        }

        #endregion
    }
}
