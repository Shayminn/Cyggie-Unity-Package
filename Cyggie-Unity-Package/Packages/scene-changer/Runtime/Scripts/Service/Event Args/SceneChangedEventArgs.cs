using Cyggie.SceneChanger.Runtime.ServicesNS;
using UnityEngine.SceneManagement;

namespace Cyggie.SceneChanger.Runtime
{
    /// <summary>
    /// Event arguments for <see cref="SceneChangerService.OnSceneChanged"/>
    /// </summary>
    public class SceneChangedEventArgs
    {
        /// <summary>
        /// The previous scene before we transitioned
        /// </summary>
        public Scene PreviousScene { get; internal set; }

        /// <summary>
        /// The new scene after we transitioned
        /// </summary>
        public Scene NewScene { get; internal set; }
    }
}
