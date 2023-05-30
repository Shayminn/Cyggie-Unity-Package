using Cyggie.Main.Runtime.Configurations;
using UnityEditor;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// Inspector editor for <see cref="ServiceManagerSettings"/>
    /// </summary>
    [CustomEditor(typeof(ServiceManagerSettings))]
    internal class ServiceManagerSettingsEditor : PackageConfigurationSettingsEditor
    {
    }
}
