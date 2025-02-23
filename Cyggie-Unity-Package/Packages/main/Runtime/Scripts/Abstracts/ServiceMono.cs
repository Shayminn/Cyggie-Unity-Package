using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Interfaces;
using Cyggie.Plugins.Services.Models;
using System.Linq;
using UnityEngine;

namespace Cyggie.Main.Runtime.ServicesNS
{
    #region Service

    /// <summary>
    /// Model abstract class of Service to be used by <see cref="ServiceManagerMono"/>
    /// </summary>
    public abstract class ServiceMono : IService
    {
        /// <summary>
        /// The monobehaviour that holds this Service <br/>
        /// This can be used for MonoBehaviour actions (i.e. StartCoroutine)
        /// </summary>
        protected MonoBehaviour Monobehaviour => ServiceManagerMono.Instance;

        /// <summary>
        /// The Service Manager game object that holds this Service
        /// </summary>
        protected GameObject GameObject => Monobehaviour.gameObject;

        /// <summary>
        /// The Service Manager game object's transform
        /// </summary>
        protected Transform Transform => GameObject.transform;

        /// <summary>
        /// Whether the service has been initialized or not yet
        /// </summary>
        protected internal bool _initialized = false;

        /// <summary>
        /// The order of priority in which the service is initialized (by default 0) <br/>
        /// Higher value means this service will be initialized before other services.
        /// </summary>
        protected virtual int Priority { get; } = 0;

        /// <inheritdoc/>
        public IServiceManager Manager { get; set; }

        /// <summary>
        /// This service's transform parent assigned to <see cref="Instantiate"/> <br/>
        /// Child of <see cref="Transform"/>
        /// </summary>
        public Transform InstantiateParent
        {
            get
            {
                if (_instantiateParent == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = GetType().Name;

                    _instantiateParent = obj.transform;
                    _instantiateParent.SetParent(Transform);
                }

                return _instantiateParent;
            }
        }
        private Transform _instantiateParent = null;

        #region MonoBehaviour virtuals

        #region Internals

        internal void OnGUI_Internal() => OnGUI();

        internal void Awake_Internal() => Awake();

        internal void OnEnable_Internal() => OnEnable();

        internal void Start_Internal() => Start();

        internal void OnDisable_Internal() => OnDisable();

        internal void Update_Internal() => Update();

        internal void FixedUpdate_Internal() => FixedUpdate();

        internal void OnDestroy_Internal() => OnDestroy();

        #endregion

        #region Protected

        /// <summary>
        /// MonoBehaviour's OnGUI <br/>
        /// OnGUI is called for rendering and handling GUI events
        /// </summary>
        protected virtual void OnGUI() { }

        /// <summary>
        /// MonoBehaviour's Awake <br/>
        /// Awake is called when the script instance is being loaded
        /// </summary>
        protected virtual void Awake() { }

        /// <summary>
        /// MonoBehaviour's OnEnable <br/>
        /// This function is called when the object becomes enabled and active
        /// </summary>
        protected virtual void OnEnable() { }

        /// <summary>
        /// MonoBehaviour's Start <br/>
        /// Start is called just before any of the Update methods is called the first time
        /// </summary>
        protected virtual void Start() { }

        /// <summary>
        /// MonoBehaviour's OnDisable <br/>
        /// This function is called when the object becomes disabled and inactive
        /// </summary>
        protected virtual void OnDisable() { }

        /// <summary>
        /// MonoBehaviour's Update <br/>
        /// Update is called every frame, if the MonoBehaviour is enabled
        /// </summary>
        protected virtual void Update() { }

        /// <summary>
        /// MonoBehaviour's FixedUpdate <br/>
        /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled
        /// </summary>
        protected virtual void FixedUpdate() { }

        /// <summary>
        /// MonoBehaviour's OnDestroy <br/>
        /// This function is called when the MonoBehaviour will be destroyed
        /// </summary>
        protected virtual void OnDestroy() { }

        #endregion

        #endregion

        #region Protected methods

        /// <summary>
        /// Instantiate an object parented to the default parent <br/>
        /// The default parent is a children of <see cref="Transform"/> (the Service Manager's transform) and is named after the Service name
        /// </summary>
        /// <param name="obj">Object to instantiate</param>
        protected T Instantiate<T>(T obj) where T : Object => Instantiate(obj, InstantiateParent);

        /// <summary>
        /// Instantiate an object parented to <paramref name="parent"/> <br/>
        /// Shortcut for instantiating an object
        /// </summary>
        /// <param name="obj">Object to instantiate</param>
        /// <param name="parent">Parent to assign to the object</param>
        /// <returns></returns>
        protected T Instantiate<T>(T obj, Transform parent) where T : Object => GameObject.Instantiate(obj, parent: parent);

        #endregion

        #region Protected virtuals

        /// <summary>
        /// Called when the object is initialized <br/>
        /// At this point, not all services have been initialized yet <br/>
        /// If your service depends on another service, make sure that service is already instantiated using <see cref="ServiceMono.Priority"/> or use <br/>
        /// <see cref="OnServicesInitialized"/> instead
        /// </summary>
        protected virtual void OnInitialized() { }

        #endregion

        /// <inheritdoc/>
        public virtual void Initialize(IServiceManager manager)
        {
            if (_initialized)
            {
                Log.Error("Trying to initialize a service, but it's already initialized!", nameof(ServiceMono));
                return;
            }

            Log.Debug($"Service ({GetType()}) initialized with no configuration.", nameof(ServiceMono));

            Manager = manager;
            OnInitialized();

            _initialized = true;
        }

        /// <inheritdoc/>
        public virtual void OnAllServicesInitialized() { }

        /// <inheritdoc/>
        public virtual void Dispose() { }

        /// <summary>
        /// Manually create a service <br/>
        /// This can be useful when using a Service within an Editor window <br/>
        /// In runtime, the <see cref="ServiceManagerMono"/> loads all enabled services on start
        /// </summary>
        /// <typeparam name="T">Type of service to create</typeparam>
        /// <returns>Created service</returns>
        public static T Create<T>() where T : ServiceMono, new()
        {
            T service = new T();
            service.OnInitialized();
            return service;
        }
    }

    #endregion

    #region Service with configuration

    public abstract class ServiceMono<T> : ServiceMono, IServiceWithConfiguration
        where T : IServiceConfiguration
    {
        /// <summary>
        /// Service configuration associated to the object <br/>
        /// Null if it doesn't exist, create a new class that implements <see cref="ServiceConfiguration{Service}"/> to automatically assign it to your service
        /// </summary>
        public T Configuration { get; private set; }

        /// <inheritdoc/>
        public System.Type ConfigurationType => typeof(T);

        /// <inheritdoc/>
        public void SetConfiguration<TConfig>(TConfig config) where TConfig : IServiceConfiguration
        {
            Configuration = (T) (IServiceConfiguration) config;
        }

        /// <inheritdoc/>
        public override void Initialize(IServiceManager manager)
        {
            if (_initialized)
            {
                Log.Error("Trying to initialize a service, but it's already initialized!", nameof(ServiceMono));
                return;
            }

            Log.Debug($"Service ({GetType().Name}) initialized with configuration ({(Configuration == null ? "none" : Configuration.GetType().ToString())}).", nameof(ServiceMono));

            Manager = manager;
            OnInitialized();

            _initialized = true;
        }

        /// <summary>
        /// Manually create a service <br/>
        /// This can be useful when using a Service within an Editor window <br/>
        /// In runtime, the <see cref="ServiceManagerMono"/> loads all enabled services on start
        /// </summary>
        /// <typeparam name="T">Type of service to create</typeparam>
        /// <returns>Created service</returns>
        public static new TService Create<TService>()
            where TService : ServiceMono<T>, new()
        {
            TService service = new TService();
            IServiceConfiguration config = ServiceManagerMono.Settings.ServiceConfigurations.FirstOrDefault(x => x.GetType() == typeof(T));

            if (config != null)
            {
                service.Configuration = (T) config;
            }

            service.OnInitialized();
            return service;
        }
    }

    #endregion
}
