using Cyggie.Main.Editor;
using Cyggie.SQLite.Runtime.ServicesNS;
using UnityEditor;

namespace Cyggie.SQLite.Editor.Configurations
{
    /// <summary>
    /// Inspector editor for <see cref="SQLiteSettings"/>
    /// </summary>
    [CustomEditor(typeof(SQLiteSettings))]
    internal class SQLiteSettingsEditor : PackageConfigurationSettingsEditor
    {
    }
}
