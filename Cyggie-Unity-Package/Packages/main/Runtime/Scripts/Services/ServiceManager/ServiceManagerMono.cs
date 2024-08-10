using Cyggie.Main.Runtime.Attributes;
using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.Utils.Helpers;
using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Models;
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
    internal class ServiceManagerMono : MonoBehaviour
    {
        private ServiceManager _serviceManager = null;

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
        private static ServiceManagerSettings _settings = null;

        /// <summary>
        /// Reference to this MonoBehaviour instance for <see cref="ServiceMono"/>s
        /// </summary>
        internal static ServiceManagerMono Instance = null;

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

        #region MonoBehaviour implementations

        private void OnGUI()
        {
            _serviceManager.Services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.OnGUIInternal();
                }
            });
        }

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);

            _serviceManager = new ServiceManager();
            _serviceManager.GenericServiceTypes.Add(typeof(ServiceMono<>));

            List<ServiceIdentifier> erroredIdentifiers = new List<ServiceIdentifier>();
            List<Type> types = new List<Type>();

            foreach (ServiceIdentifier serviceIdentifier in Settings.ServiceIdentifiers)
            {
                Type type = Type.GetType(serviceIdentifier.AssemblyName);

                // Check for null types (happens if the service identifier's assembly name has changed but the identifier has not been recreated)
                if (type == null)
                {
                    Log.Error($"Unable to find type ({serviceIdentifier.AssemblyName}), is null, skipping...");
                    continue;
                }

                types.Add(type);
            }

            _serviceManager.Initialize(Settings.ServiceConfigurations, types.ToArray());

            _serviceManager.Services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.AwakeInternal();
                }
            });
        }

        private void OnEnable()
        {
            _serviceManager.Services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.OnEnableInternal();
                }
            });
        }

        private void Start()
        {
            _serviceManager.Services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.StartInternal();
                }
            });
        }

        private void OnDisable()
        {
            _serviceManager.Services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.OnDisableInternal();
                }
            });
        }

        private void Update()
        {
            _serviceManager.Services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.UpdateInternal();
                }
            });
        }

        private void FixedUpdate()
        {
            _serviceManager.Services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.FixedUpdateInternal();
                }
            });
        }

        private void OnDestroy()
        {
            _serviceManager.Services.ForEach(x =>
            {
                if (x is ServiceMono service)
                {
                    service.OnDestroyInternal();
                }
            });

            ServiceManager.Dispose();
        }

        #endregion
    }
}
