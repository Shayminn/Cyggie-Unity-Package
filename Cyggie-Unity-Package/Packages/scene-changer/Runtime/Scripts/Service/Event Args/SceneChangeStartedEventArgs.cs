using Cyggie.SceneChanger.Runtime.ServicesNS;
using UnityEngine.SceneManagement;

namespace Cyggie.SceneChanger.Runtime
{
    /// <summary>
    /// Event arguments for <see cref="SceneChangerService.OnSceneChangeStarted"/>
    /// </summary>
    public class SceneChangeStartedEventArgs
    {
        /// <summary>
        /// Scene name that is starting
        /// </summary>
        public string Scene { get; internal set; }
    }
}
