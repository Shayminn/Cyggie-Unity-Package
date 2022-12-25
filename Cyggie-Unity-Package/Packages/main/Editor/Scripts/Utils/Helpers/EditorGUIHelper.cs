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
        public static void DrawAsReadOnly(Action gui)
        {
            GUI.enabled = false;
            gui.Invoke();
            GUI.enabled = true;
        }

        /// <summary>
        /// Draw EditorGUI as read only if <paramref name="condition"/> resolves to true, else it will draw it as modifiable.
        /// </summary>
        /// <param name="condition">Condition in order to draw GUI as read only</param>
        /// <param name="gui">GUI to draw</param>
        public static void DrawAsReadOnly(bool condition, Action gui)
        {
            if (condition)
            {
                GUI.enabled = false;
                gui.Invoke();
                GUI.enabled = true;
            }
            else
            {
                gui.Invoke();
            }
        }
    }
}
