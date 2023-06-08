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
        /// Draw EditorGUI as horizontal
        /// </summary>
        /// <param name="gui">GUI to draw</param>
        public static void DrawHorizontal(Action gui, params GUILayoutOption[] guiLayoutOptions)
        {
            EditorGUILayout.BeginHorizontal(guiLayoutOptions);
            gui?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw EditorGUI as vertical
        /// </summary>
        /// <param name="gui">GUI to draw</param>
        public static void DrawVertical(Action gui, params GUILayoutOption[] guiLayoutOptions)
        {
            EditorGUILayout.BeginVertical(guiLayoutOptions);
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
        /// Draw Editor GUI within a scroll view
        /// </summary>
        /// <param name="scrollPosition">Current scroll position</param>
        /// <param name="gui">GUI to drawa</param>
        /// <param name="alwaysShowHorizontal">Whether the scroll view's horizontal bar is always visible</param>
        /// <param name="alwaysShowVertical">Whether the scroll view's vertical bar is always visible</param>
        public static void DrawWithScrollview(ref Vector2 scrollPosition, Action gui, bool alwaysShowHorizontal = false, bool alwaysShowVertical = false, GUIStyle horizontalStyle = null, GUIStyle verticalStyle = null, GUIStyle backgroundStyle = null, params GUILayoutOption[] guiLayoutOptions)
        {
            horizontalStyle ??= GUI.skin.horizontalScrollbar;
            verticalStyle ??= GUI.skin.verticalScrollbar;
            backgroundStyle ??= GUI.skin.scrollView;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalStyle, verticalStyle, backgroundStyle, guiLayoutOptions);
            gui.Invoke();
            EditorGUILayout.EndScrollView();
        }
    }
}
