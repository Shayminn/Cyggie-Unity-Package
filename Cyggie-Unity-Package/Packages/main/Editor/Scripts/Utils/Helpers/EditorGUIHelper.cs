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
        /// Draw EditorGUI with a confirm & cancel button
        /// </summary>
        /// <param name="active">Reference bool to determine if the confirm/cancel should be displayed</param>
        /// <param name="confirmLabel">Label above the confirm/cancel buttons</param>
        /// <param name="onConfirm">Invoked when the confirm button is clicked</param>
        /// <param name="onCancel">Invoked when the cancel button is clicked</param>
        /// <param name="onInactiveGUI">Invoked when active is false</param>
        public static void DrawWithConfirm(ref bool active, string confirmLabel, Action onConfirm, Action onCancel = null, Action onInactiveGUI = null)
            => DrawWithConfirm(ref active, new GUIContent(confirmLabel), onConfirm, onCancel, onInactiveGUI);

        /// <summary>
        /// Draw EditorGUI with a confirm & cancel button
        /// </summary>
        /// <param name="active">Reference bool to determine if the confirm/cancel should be displayed</param>
        /// <param name="confirmLabel">GUIContent Label above the confirm/cancel buttons</param>
        /// <param name="onConfirm">Invoked when the confirm button is clicked</param>
        /// <param name="onCancel">Invoked when the cancel button is clicked</param>
        /// <param name="onInactiveGUI">Invoked when active is false</param>
        public static void DrawWithConfirm(ref bool active, GUIContent confirmLabel = null, Action onConfirm = null, Action onCancel = null, Action onInactiveGUI = null)
        {
            if (active)
            {
                if (confirmLabel != null)
                {
                    EditorGUILayout.LabelField(confirmLabel);
                }

                bool newActive = active;
                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    GUIHelper.DrawWithBackgroundColor(Color.green, gui: () =>
                    {
                        // Confirm delete
                        if (GUILayout.Button("Confirm", GUILayout.Width(100)))
                        {
                            newActive = false;
                            onConfirm?.Invoke();
                        }
                    });

                    GUIHelper.DrawWithBackgroundColor(Color.red, gui: () =>
                    {
                        if (GUILayout.Button("Cancel", GUILayout.Width(100)))
                        {
                            newActive = false;
                            onCancel?.Invoke();
                        }
                    });
                });

                if (newActive != active)
                {
                    active = newActive;
                }
            }
            else
            {
                onInactiveGUI?.Invoke();
            }
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
