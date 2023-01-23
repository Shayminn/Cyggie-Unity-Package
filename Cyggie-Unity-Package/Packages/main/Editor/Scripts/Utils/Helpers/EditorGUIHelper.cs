using System;
using UnityEditor;
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
        /// Draw EditorGUI as horizontal
        /// </summary>
        /// <param name="gui">GUI to draw</param>
        public static void DrawHorizontal(Action gui)
        {
            EditorGUILayout.BeginHorizontal();
            gui?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw EditorGUI as vertical
        /// </summary>
        /// <param name="gui">GUI to draw</param>
        public static void DrawVertical(Action gui)
        {
            EditorGUILayout.BeginVertical();
            gui?.Invoke();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Check for change in GUI
        /// </summary>
        /// <param name="gui">GUI to draw</param>
        /// <returns>Any changes?</returns>
        public static bool CheckChange(Action gui)
        {
            EditorGUI.BeginChangeCheck();
            gui?.Invoke();

            return EditorGUI.EndChangeCheck();
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

        /// <summary>
        /// Draw Editor GUI within a scroll view
        /// </summary>
        /// <param name="scrollPosition">Current scroll position</param>
        /// <param name="gui">GUI to drawa</param>
        /// <param name="alwaysShowHorizontal">Whether the scroll view's horizontal bar is always visible</param>
        /// <param name="alwaysShowVertical">Whether the scroll view's vertical bar is always visible</param>
        public static void DrawWithScrollview(ref Vector2 scrollPosition, Action gui, bool alwaysShowHorizontal = false, bool alwaysShowVertical = false)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical);
            gui.Invoke();
            EditorGUILayout.EndScrollView();
        }
    }
}
