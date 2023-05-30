using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.ReferencePool.Runtime.ServicesNS;
using UnityEngine;

namespace Cyggie.ReferencePool.Runtime
{
    /// <summary>
    /// Component to register a <see cref="ReferencePoolObject"/> <br/>
    /// After registering, it can be retrieved using <see cref="ReferencePoolService.GetReference(ReferencePoolObject)"/>
    /// </summary>
    public class ReferencePoolInitializer : MonoBehaviour
    {
        [SerializeField, Tooltip("The reference pool scriptable object to add to the service.")]
        private ReferencePoolObject _refPoolObj = null;

        [SerializeField, Tooltip("Whether this gameObject should be set to inactive rigth after adding itself to the Reference Pool service.")]
        private bool _disableAfterAwake = false;

        private static ReferencePoolService _service = null;
        private static ReferencePoolService Service => _service ??= ServiceManager.Get<ReferencePoolService>();

        private void Awake()
        {
            if (_refPoolObj == null)
            {
                Debug.LogError($"[Reference Pool] Initializer's reference is missing. Assign it in the inspector. GameObject: {gameObject}.");
                return;
            }

            if (Service == null)
            {
                Debug.LogError("[Reference Pool] Service not found. Make sure it is initialized.");
                return;
            }

            Service.AddToPool(_refPoolObj, gameObject);

            if (_disableAfterAwake)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
