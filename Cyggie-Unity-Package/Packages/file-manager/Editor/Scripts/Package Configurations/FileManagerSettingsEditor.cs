using Cyggie.FileManager.Runtime.ServicesNS;
using Cyggie.Main.Editor;
using UnityEditor;

namespace Cyggie.FileManager.Editor.Configurations
{
    /// <summary>
    /// Inspector editor for <see cref="FileManagerSettings"/>
    /// </summary>
    [CustomEditor(typeof(FileManagerSettings))]
    internal class FileManagerSettingsEditor : PackageConfigurationSettingsEditor
    {
    }
}
