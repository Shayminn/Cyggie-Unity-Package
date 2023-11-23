using System;
using UnityEngine;

namespace Cyggie.Plugins.Editor.Helpers
{
    /// <summary>
    /// Helper class to GUI drawing.
    /// </summary>
    public static class GUIHelper
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

        /// <summary>
        /// Draw EditorGUI as read only if <paramref name="condition"/> resolves to true, else it will draw it as modifiable.
        /// </summary>
        /// <param name="condition">Condition in order to draw GUI as read only</param>
        /// <param name="gui">GUI to draw</param>
        public static void DrawAsReadOnly(bool condition, Action<bool> gui)
        {
            if (condition)
            {
                GUI.enabled = false;
                gui.Invoke(true);
                GUI.enabled = true;
            }
            else
            {
                gui.Invoke(false);
            }
        }

        /// <summary>
        /// Draw Editor GUI with a different <paramref name="color"/> <br/>
        /// The GUI's color will be back to default after drawing <paramref name="gui"/>
        /// </summary>
        /// <param name="color">Target color</param>
        /// <param name="gui">GUI to draw</param>
        public static void DrawWithColor(Color color, Action gui)
        {
            Color temp = GUI.color;

            GUI.color = color;
            gui.Invoke();
            GUI.color = temp;
        }

        /// <summary>
        /// Draw Editor GUI with a different background <paramref name="color"/> <br/>
        /// The GUI's background color will be back to default after drawing <paramref name="gui"/>
        /// </summary>
        /// <param name="color">Target color</param>
        /// <param name="gui">GUI to draw</param>
        public static void DrawWithBackgroundColor(Color color, Action gui)
        {
            Color temp = GUI.color;

            GUI.backgroundColor = color;
            gui.Invoke();
            GUI.backgroundColor = temp;
        }

        /// <summary>
        /// Draw Editor GUI with a different content <paramref name="color"/> <br/>
        /// The GUI's content color will be back to default after drawing <paramref name="gui"/>
        /// </summary>
        /// <param name="color">Target color</param>
        /// <param name="gui">GUI to draw</param>
        public static void DrawWithTintColor(Color color, Action gui)
        {
            Color temp = GUI.color;

            GUI.contentColor = color;
            gui.Invoke();
            GUI.contentColor = temp;
        }
    }
}
