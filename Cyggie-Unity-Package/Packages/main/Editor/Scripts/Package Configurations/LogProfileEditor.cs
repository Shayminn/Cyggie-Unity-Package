using Cyggie.Main.Runtime;
using UnityEditor;

namespace Cyggie.Main.Editor
{
    /// <summary>
    /// Inspector editor for <see cref="LogProfile"/>
    /// </summary>
    [CustomEditor(typeof(LogProfile))]
    internal class LogProfileEditor : PackageConfigurationSettingsEditor
    {
    }
}
