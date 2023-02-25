using UnityEngine;

namespace Cyggie.Main.Runtime.Services
{
    /// <summary>
    /// Model abstract class of Service to be used by <see cref="ServiceManager"/>
    /// </summary>
    public abstract class Service
    {
        private ServiceManager _manager = null;

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
        /// The game object that holds this Service
        /// </summary>
        protected GameObject GameObject => _manager.gameObject;

        /// <summary>
        /// The order of priority in which the service is initialized (by default 0) <br/>
        /// Higher value means this service will be initialized before other services.
        /// </summary>
        protected virtual int Priority { get; } = 0;

        internal int InternalPriority => Priority;

        #region MonoBehaviour virtuals

        public virtual void Awake() { }

        public virtual void OnEnable() { }

        public virtual void Start() { }

        public virtual void OnDisable() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void Destroy() { }

        #endregion

        /// <summary>
        /// Object has been initialized
        /// </summary>
        /// <param name="configuration"></param>
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

        /// <summary>
        /// Called when the object is initialized
        /// </summary>
        /// <param name="configuration">Configuration for the service, null if not set</param>
        protected virtual void OnInitialized(ServiceConfiguration configuration) { }

        /// <summary>
        /// Called when all services have been initialized (equivalent to subscribing to <see cref="ServiceManager.OnServicesInitialized"/>)
        /// </summary>
        protected virtual void OnServicesInitialized() { }
    }
}
