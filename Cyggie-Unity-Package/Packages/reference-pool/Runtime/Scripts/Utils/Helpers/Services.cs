using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.ReferencePool.Runtime.ServicesNS;

namespace Cyggie.ReferencePool.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper class to quickly access services
    /// </summary>
    public static class Services
    {
        private static ReferencePoolService _refPoolService = null;
        public static ReferencePoolService ReferencePool => _refPoolService ??= ServiceManager.Get<ReferencePoolService>();
    }
}