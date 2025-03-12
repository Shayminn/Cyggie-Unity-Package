using System;
using UnityEditor;

namespace Cyggie.Main.Editor.Utils.Helpers
{
    /// <summary>
    /// Helper class to Editor GUI drawing.
    /// </summary>
    public static class EditorGUIHelper
    {
        /// <summary>
        /// Draw a gui with a conditional toggle, enabled when <paramref name="condition"/> is true
        /// </summary>
        /// <param name="condition">Condition whether the gui is enabled</param>
        public static void DrawWithEnabledCondition(bool condition, Action gui)
        {
            EditorGUI.BeginDisabledGroup(!condition);
            gui?.Invoke();
            EditorGUI.EndDisabledGroup();
        }
    }
}
