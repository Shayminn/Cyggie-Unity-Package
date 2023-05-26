using Cyggie.FileManager.Runtime.ServicesNS;
using Cyggie.Main.Runtime.ServicesNS;

namespace Cyggie.FileManager.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper class to quickly access services
    /// </summary>
    public static class Services
    {
        private static FileManagerService _fileManagerService = null;
        public static FileManagerService FileManager => _fileManagerService ??= ServiceManager.Get<FileManagerService>();
    }
}
