using System;
using UnityEngine;

namespace Cyggie.Main.Editor.Utils.Helpers
{
    /// <summary>
    /// Helper class to Editor GUI drawing.
    /// </summary>
    public static class EditorGUIHelper
    {
        /// <summary>
        /// Draw EditorGUI as read only.
        /// </summary>
        public static void DrawAsReadOnly(Action readOnlyGUI)
        {
            GUI.enabled = false;
            readOnlyGUI.Invoke();
            GUI.enabled = true;
        }
    }
}
