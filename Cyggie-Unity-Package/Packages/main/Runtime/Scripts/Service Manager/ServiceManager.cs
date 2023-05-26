using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cyggie.Main.Runtime.ServicesNS
{
    /// <summary>
    /// Manager class for all services of type <see cref="Service"/> <br/>
    /// Any subclass of type <see cref="Service"/> will automatically be created and initialized
    /// </summary>
    public class ServiceManager : MonoBehaviour
    {
        /// <summary>
        /// Instance object of this class
        /// </summary>
        private static ServiceManager _instance = null;

        /// <summary>
        /// Settings saved in the Resources folder, managed in Project Settings
        /// </summary>
        private static ServiceManagerSettings _settings = null;

        /// <summary>
        /// List of all initialized services available to use
        /// </summary>
        private readonly List<Service> _services = new List<Service>();

        #region Initialization

        /// <summary>
        /// Called at the start of runtime before awake <br/>
        /// Create the <see cref="ServiceManager"/> object prefab in the scene
        /// </summary>
        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            // Get settings saved in Resources folder
            _settings = Resources.Load<ServiceManagerSettings>(ServiceManagerSettings.cResourcesPath);

            if (_settings == null)
            {
                Debug.LogError($"Unable to find Service Manager Settings in Resources.");
                return;
            }

            if (!_settings.IsValid)
            {
                Debug.LogError($"Service Manager failed to initialize. Settings are invalid; fix them in the Project Settings.");
                return;
            }

            // Create service manager object
            Instantiate(_settings.Prefab);
        }

        /// <summary>
        /// Create instance of all services <br/>
        /// </summary>
        private void InitializeServices()
        {
            IEnumerable<Type> servicesTypes = TypeHelper.GetAllSubclassTypes<Service>(true);

            // Create all the services
            foreach (Type t in servicesTypes)
            {
                Service service = (Service) Activator.CreateInstance(t);

                if (service.AddServiceToServiceManager())
                {
                    _services.Add(service);
                }
            }

            // Initialize all services by order of priority
            foreach (Service service in _services.OrderByDescending(x => x.InternalPriority))
            {
                // Add configuration to service if it exists
                ServiceConfiguration configuration = _settings.ServiceConfigurations.FirstOrDefault(c => c.ServiceType == service.GetType());
                service.Initialize(this, configuration);
            }

            Debug.Log($"[Service Manager] Initialized Services. Count: {_services.Count}.");
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
                Debug.LogError($"Failed to get a service, Service Manager has not yet been initialized.");
                return default;
            }

            Service service = _instance._services.FirstOrDefault(x => x.GetType() == typeof(T));

            if (service == null)
            {
                Debug.LogError($"Unable to find service of type: {typeof(T)}.");
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
            _services.ForEach(x => x.OnDisable());
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
