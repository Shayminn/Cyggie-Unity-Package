using Cyggie.LanguageManager.Runtime.Configurations;
using Cyggie.Main.Editor;
using Cyggie.Main.Editor.Utils.Helpers;
using UnityEditor;

namespace Cyggie.LanguageManager.Editor.Configurations
{
    /// <summary>
    /// Inspector editor for <see cref="LanguageManagerSettings"/>
    /// </summary>
    [CustomEditor(typeof(LanguageManagerSettings))]
    internal class SceneChangerSettingsEditor : PackageConfigurationSettingsEditor
    {
    }
}
