using UnityEngine;

namespace Cyggie.Main.Runtime.ServicesNS
{
    /// <summary>
    /// Model abstract class of Service to be used by <see cref="ServiceManager"/>
    /// </summary>
    public abstract class Service
    {
        /// <summary>
        /// Configuration object
        /// </summary>
        protected ServiceConfiguration _configuration = null;

        /// <summary>
        /// The monobehaviour that holds this Service <br/>
        /// This can be used for MonoBehaviour actions (i.e. StartCoroutine)
        /// </summary>
        protected MonoBehaviour Monobehaviour => _manager;

        /// <summary>
        /// The Service Manager game object that holds this Service
        /// </summary>
        protected GameObject GameObject => _manager.gameObject;

        /// <summary>
        /// The Service Manager game object's transform
        /// </summary>
        protected Transform Transform => GameObject.transform;

        /// <summary>
        /// The order of priority in which the service is initialized (by default 0) <br/>
        /// Higher value means this service will be initialized before other services.
        /// </summary>
        protected virtual int Priority { get; } = 0;

        internal int InternalPriority => Priority;

        private ServiceManager _manager = null;

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

        /// <summary>
        /// MonoBehaviour's OnGUI <br/>
        /// OnGUI is called for rendering and handling GUI events
        /// </summary>
        public virtual void OnGUI() { }

        /// <summary>
        /// MonoBehaviour's Awake <br/>
        /// Awake is called when the script instance is being loaded
        /// </summary>
        public virtual void Awake() { }

        /// <summary>
        /// MonoBehaviour's OnEnable <br/>
        /// This function is called when the object becomes enabled and active
        /// </summary>
        public virtual void OnEnable() { }

        /// <summary>
        /// MonoBehaviour's Start <br/>
        /// Start is called just before any of the Update methods is called the first time
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// MonoBehaviour's OnDisable <br/>
        /// This function is called when the object becomes disabled and inactive
        /// </summary>
        public virtual void OnDisable() { }

        /// <summary>
        /// MonoBehaviour's Update <br/>
        /// Update is called every frame, if the MonoBehaviour is enabled
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// MonoBehaviour's FixedUpdate <br/>
        /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled
        /// </summary>
        public virtual void FixedUpdate() { }

        /// <summary>
        /// MonoBehaviour's OnDestroy <br/>
        /// This function is called when the MonoBehaviour will be destroyed
        /// </summary>
        public virtual void OnDestroy() { }

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
        /// Override this to add a condition to whether the Service is created and added to the <see cref="ServiceManager"/> <br/>
        /// Defaults to true
        /// </summary>
        /// <returns>Whether this service should be added</returns>
        protected virtual bool AddService() { return true; }

        /// <summary>
        /// Called when the object is initialized <br/>
        /// At this point, not all services have been initialized yet <br/>
        /// If your service depends on another service, make sure that service is already instantiated using <see cref="Service.Priority"/> or use <br/>
        /// <see cref="OnServicesInitialized"/> instead
        /// </summary>
        /// <param name="configuration">Configuration for the service, null if not set</param>
        protected virtual void OnInitialized(ServiceConfiguration configuration) { }

        /// <summary>
        /// Called when all services have been initialized (equivalent to subscribing to <see cref="ServiceManager.OnServicesInitialized"/>)
        /// </summary>
        protected virtual void OnServicesInitialized() { }

        #endregion

        #region Internals

        /// <summary>
        /// Whether this object should be added to ServiceManager in runtime
        /// </summary>
        internal bool AddServiceToServiceManager() => AddService();

        /// <summary>
        /// Object has been initialized
        /// </summary>
        /// <param name="configuration">Configuration for the service, null if not set</param>
        internal virtual void Initialize(ServiceManager manager, ServiceConfiguration configuration)
        {
            _manager = manager;
            ServiceManager.OnServicesInitialized += OnServicesInitialize;

            _configuration = configuration;
            OnInitialized(configuration);
        }

        /// <summary>
        /// Internal Event handler for <see cref="ServiceManager.OnServicesInitialized"/>
        /// </summary>
        internal virtual void OnServicesInitialize()
        {
            ServiceManager.OnServicesInitialized -= OnServicesInitialize;
            OnServicesInitialized();
        }

        #endregion
    }
}
