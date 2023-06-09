using Cyggie.Main.Runtime.Utils.Helpers;
using UnityEngine;

namespace Cyggie.Main.Runtime.ServicesNS.ReferencePool
{
    /// <summary>
    /// Component to register a <see cref="ReferencePoolObject"/> <br/>
    /// After registering, it can be retrieved using <see cref="ReferencePoolService.GetReference(ReferencePoolObject)"/>
    /// </summary>
    public class ReferencePoolInitializer : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField, Tooltip("The reference pool scriptable object to add to the service.")]
        private ReferencePoolObject _referenceObject = null;

        [Header("Events after Initialization")]
        [SerializeField, Tooltip("Whether this gameObject should be set to inactive after adding its reference to the Reference Pool Service.")]
        private bool _disableGameObject = false;

        [SerializeField, Tooltip("Whether this component should be destroyed after adding its reference to the Reference Pool Service.")]
        private bool _destroyComponent = true;

        private bool _initialized = false;

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize this component, adding it to the Service <br/>
        /// This is called before Awake by the Service if this component is found in the scene at the start <br/>
        /// This allows safe calling of Awake in any other component to get this <see cref="_referenceObject"/>
        /// </summary>
        internal void Initialize()
        {
            if (_initialized) return;

            _initialized = true;
            if (_referenceObject == null)
            {
                Log.Error($"Initializer's reference is missing. Assign it in the inspector. GameObject: {gameObject}.", nameof(ReferencePoolInitializer));
                return;
            }

            MainServices.ReferencePool.AddToPool(_referenceObject, gameObject);

            if (_disableGameObject)
            {
                gameObject.SetActive(false);
            }

            if (_destroyComponent)
            {
                Destroy(this);
            }
        }
    }
}
