using UnityEngine;

namespace Cyggie.Main.Runtime.Services
{
    /// <summary>
    /// Model abstract class of Service to be used by <see cref="ServiceManager"/>
    /// </summary>
    public abstract class Service
    {
        /// <summary>
        /// The monobehaviour that holds this Service <br/>
        /// This can be used for MonoBehaviour actions (i.e. StartCoroutine)
        /// </summary>
        protected MonoBehaviour _mono = null;

        /// <summary>
        /// Configuration object
        /// </summary>
        protected ServiceConfiguration _configuration = null;

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
        internal virtual void Initialize(MonoBehaviour mono, ServiceConfiguration configuration)
        {
            _mono = mono;

            _configuration = configuration;
            OnInitialized(configuration);
        }

        /// <summary>
        /// Called when the object is initialized
        /// </summary>
        /// <param name="configuration">Configuration for the service, null if not set</param>
        protected virtual void OnInitialized(ServiceConfiguration configuration) { }
    }
}
