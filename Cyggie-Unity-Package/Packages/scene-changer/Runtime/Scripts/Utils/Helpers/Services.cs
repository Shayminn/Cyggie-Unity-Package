using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.SceneChanger.Runtime.ServicesNS;

namespace Cyggie.SceneChanger.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper class to quickly access services
    /// </summary>
    public static class Services
    {
        private static SceneChangerService _sceneChangerService = null;
        public static SceneChangerService SceneChanger => _sceneChangerService ??= ServiceManager.Get<SceneChangerService>();
    }
}
