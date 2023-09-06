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
        /// Scene that was previously loaded
        /// </summary>
        public Scene PreviousScene { get; internal set; }

        /// <summary>
        /// Scene that was loaded
        /// </summary>
        public Scene NewScene { get; internal set; }

        /// <summary>
        /// Whether the scene was reloaded or changed to a new one
        /// </summary>
        public bool Reloaded => PreviousScene.name == NewScene.name;
    }
}
