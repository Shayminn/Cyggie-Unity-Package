using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Main.Runtime.ServicesNS.ReferencePool;

namespace Cyggie.Main.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper class to manage references to singleton services
    /// </summary>
    public static class MainServices
    {
        private static ReferencePoolService _referencePoolService = null;
        public static ReferencePoolService ReferencePool => _referencePoolService ??= ServiceManager.Get<ReferencePoolService>();
    }
}
