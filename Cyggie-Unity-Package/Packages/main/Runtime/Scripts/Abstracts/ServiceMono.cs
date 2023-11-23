using Cyggie.Plugins.Services.Interfaces;
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
        protected MonoBehaviour Monobehaviour => ServiceManager as ServiceManagerMono;

        /// <summary>
        /// The Service Manager game object that holds this Service
        /// </summary>
        protected GameObject GameObject => Monobehaviour.gameObject;

        /// <summary>
        /// The Service Manager game object's transform
        /// </summary>
        protected Transform Transform => GameObject.transform;

        /// <summary>
        /// The order of priority in which the service is initialized (by default 0) <br/>
        /// Higher value means this service will be initialized before other services.
        /// </summary>
        protected virtual int Priority { get; } = 0;

        /// <inheritdoc/>
        public IServiceManager ServiceManager { get; set; }

        private Transform _instantiateParent = null;
        private Transform InstantiateParent
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

        #region MonoBehaviour virtuals

        #region Internals

        internal void OnGUIInternal() => OnGUI();

        internal void AwakeInternal() => Awake();

        internal void OnEnableInternal() => OnEnable();

        internal void StartInternal() => Start();

        internal void OnDisableInternal() => OnDisable();

        internal void UpdateInternal() => Update();

        internal void FixedUpdateInternal() => FixedUpdate();

        internal void OnDestroyInternal() => OnDestroy();

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

        /// <summary>
        /// Called when all services have been initialized (equivalent to subscribing to <see cref="ServiceManagerMono.OnServicesInitialized"/>)
        /// </summary>
        protected virtual void OnServicesInitialized() { }

        #endregion

        public void Initialize(IServiceManager manager)
        {
            ServiceManager = manager;
        }

        #region Statics

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
            service.OnServicesInitialized();
            return service;
        }

        #endregion
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
            service.OnServicesInitialized();
            return service;
        }
    }

    #endregion
}
