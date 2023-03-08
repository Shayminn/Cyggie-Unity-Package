using Cyggie.Main.Editor;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.SQLite.Runtime.Services;
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
