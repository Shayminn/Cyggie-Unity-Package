using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.ServicesNS.ReferencePool;
using Cyggie.Main.Runtime.Utils.Helpers;
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
        internal static ServiceManagerSettings Settings = null;

        /// <summary>
        /// Instance object of this component <br/>
        /// This gameObject holds all the services
        /// </summary>
        private static ServiceManager _instance = null;

        /// <summary>
        /// List of all initialized services available to use
        /// </summary>
        private readonly List<Service> _services = new List<Service>();

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
            // Get settings saved in Resources folder
            Settings = Resources.Load<ServiceManagerSettings>(ServiceManagerSettings.cResourcesPath);

            if (Settings == null)
            {
                Log.Error($"Unable to find Service Manager Settings in Resources.", nameof(ServiceManager));
                return;
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

            Log.Debug($"Service Manager initialized services. Count: {_services.Count}.", nameof(ServiceManager));
            OnServicesInitialized?.Invoke();
        }

        #endregion

        /// <summary>
        /// Get a service of a specific type
        /// </summary>
        /// <typeparam name="T">Type of service</typeparam>
        /// <returns>Stored service</returns>
        public static T Get<T>() where T : Service
        {
            // Check if Service Manager has been initialized
            if (_instance == null)
            {
                Log.Error($"Failed to get a service, Service Manager has not yet been initialized. Use {nameof(OnServicesInitialized)} or call Get in {nameof(Start)}.", nameof(ServiceManager));
                return default;
            }

            Service service = _instance._services.FirstOrDefault(x => x.GetType() == typeof(T));

            if (service == null)
            {
                Log.Error($"Unable to find service of type: {typeof(T)}.", nameof(ServiceManager));
            }

            return (T) service;
        }

        #region MonoBehaviour implementations

        private void OnGUI()
        {
            _services.ForEach(x => x.OnGUI());
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

            _services.ForEach(x => x.Awake());
        }

        private void OnEnable()
        {
            _services.ForEach(x => x.OnEnable());
        }

        private void Start()
        {
            _services.ForEach(x => x.Start());
        }

        private void OnDisable()
        {
            _services.ForEach(x => x.OnDisable());
        }

        private void Update()
        {
            _services.ForEach(x => x.Update());
        }

        private void FixedUpdate()
        {
            _services.ForEach(x => x.FixedUpdate());
        }

        private void OnDestroy()
        {
            _services.ForEach(x => x.OnDestroy());
        }

        #endregion
    }
}
