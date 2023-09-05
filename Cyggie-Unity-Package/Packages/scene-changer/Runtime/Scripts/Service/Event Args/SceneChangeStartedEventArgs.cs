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
        /// Scene that is starting
        /// </summary>
        public Scene Scene { get; internal set; }
    }
}
