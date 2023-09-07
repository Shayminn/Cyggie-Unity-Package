using Cyggie.SceneChanger.Runtime.ServicesNS;
using UnityEngine.SceneManagement;

namespace Cyggie.SceneChanger.Runtime
{
    /// <summary>
    /// Event arguments for <see cref="SceneChangerService.OnSceneChangeCompleted"/>
    /// </summary>
    public class SceneChangeCompletedEventArgs
    {
        /// <summary>
        /// Scene name that was previously loaded
        /// </summary>
        public string PreviousScene { get; internal set; }

        /// <summary>
        /// Scene name that was loaded
        /// </summary>
        public string NewScene { get; internal set; }

        /// <summary>
        /// Whether the scene was reloaded or changed to a new one
        /// </summary>
        public bool Reloaded => PreviousScene == NewScene;
    }
}
