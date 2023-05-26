using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.SQLite.Runtime.ServicesNS;
using UnityEngine;

namespace Cyggie.SQLite.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper class to quickly access services
    /// </summary>
    public static class Services
    {
        private static SQLiteService _sqliteService = null;
        public static SQLiteService SQLite => _sqliteService ??= ServiceManager.Get<SQLiteService>();
    }
}
