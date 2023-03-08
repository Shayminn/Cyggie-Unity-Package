using Cyggie.FileManager.Runtime.Services;
using Cyggie.Main.Editor;
using Cyggie.Main.Editor.Utils.Helpers;
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
