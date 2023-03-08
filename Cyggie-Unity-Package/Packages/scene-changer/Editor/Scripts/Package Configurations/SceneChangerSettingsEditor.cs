using Cyggie.Main.Editor;
using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.SceneChanger.Runtime.Configurations;
using UnityEditor;
using UnityEngine;

namespace Cyggie.SceneChanger.Editor.Configurations
{
    /// <summary>
    /// Inspector editor for <see cref="SceneChangerSettings"/>
    /// </summary>
    [CustomEditor(typeof(SceneChangerSettings))]
    internal class SceneChangerSettingsEditor : PackageConfigurationSettingsEditor
    {
    }
}
