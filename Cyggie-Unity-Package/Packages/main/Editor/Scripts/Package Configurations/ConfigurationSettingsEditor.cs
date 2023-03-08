using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Configurations;
using UnityEditor;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// Inspector editor for <see cref="ConfigurationSettings"/>
    /// </summary>
    [CustomEditor(typeof(ConfigurationSettings))]
    internal class ConfigurationSettingsEditor : PackageConfigurationSettingsEditor
    {
    }
}
