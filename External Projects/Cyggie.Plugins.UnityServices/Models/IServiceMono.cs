using UnityEngine;

namespace Cyggie.Plugins.UnityServices.Models
{
    /// <summary>
    /// Interface for a service that needs access to the Monobehaviour of the service manager
    /// </summary>
    public interface IServiceMono
    {
        /// <summary>
        /// Called when the monobehaviour is assigned after all services are initialized
        /// </summary>
        public void OnMonoBehaviourAssigned(MonoBehaviour mono);
    }
}
