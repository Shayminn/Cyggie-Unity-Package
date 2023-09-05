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
        /// Scene that was completed
        /// </summary>
        public Scene Scene { get; internal set; }
    }
}
