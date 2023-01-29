using Cyggie.LanguageManager.Runtime.Configurations;
using Cyggie.Main.Editor.Utils.Helpers;
using UnityEditor;

namespace Cyggie.LanguageManager.Editor.Configurations
{
    /// <summary>
    /// Language Manager Settings inspector editor
    /// </summary>
    [CustomEditor(typeof(LanguageManagerSettings))]
    internal class SceneChangerSettingsEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Override the default inspector GUI to make it readonly
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUIHelper.DrawAsReadOnly(gui: () =>
            {
                base.OnInspectorGUI();
            });
        }
    }
}
