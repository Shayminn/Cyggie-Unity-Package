namespace Cyggie.Main.Runtime.Services
{
    /// <summary>
    /// Model abstract class of Service to be used by <see cref="ServiceManager"/>
    /// </summary>
    public abstract class Service
    {
        private ServiceConfiguration _configuration = null;

        #region MonoBehaviour virtuals

        public virtual void Awake() { }

        public virtual void OnEnable() { }

        public virtual void Start() { }

        public virtual void OnDisable() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void Destroy() { }

        #endregion

        protected T GetConfiguration<T>() where T : ServiceConfiguration
            => (T) _configuration;

        internal void SetConfiguration(ServiceConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
