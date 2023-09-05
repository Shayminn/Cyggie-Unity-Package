using Cyggie.SceneChanger.Runtime.ServicesNS;

namespace Cyggie.SceneChanger.Runtime
{
    /// <summary>
    /// Event arguments for <see cref="SceneChangerService.OnSceneChangeStarted"/>
    /// </summary>
    public class SceneChangeStartedEventArgs
    {
        /// <summary>
        /// Name of the scene to start
        /// </summary>
        public string SceneName { get; private set; }

        /// <summary>
        /// Internal constructor for scene change started event <br/>
        /// This event can only be created internally since it is exclusively called from <see cref="SceneChangerService"/>
        /// </summary>
        /// <param name="sceneName">Name of the scene</param>
        internal SceneChangeStartedEventArgs(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}
