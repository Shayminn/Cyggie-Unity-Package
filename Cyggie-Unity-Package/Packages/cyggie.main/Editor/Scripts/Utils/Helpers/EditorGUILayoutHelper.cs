using Cyggie.Plugins.Editor.Helpers;
using System;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Utils.Helpers
{
    /// <summary>
    /// Helper class to Editor GUI Layout drawing.
    /// </summary>
    public static class EditorGUILayoutHelper
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
        /// Draw the editor script reference from a monobehaviour object
        /// </summary>
        public static void DrawScriptReference(MonoBehaviour mono, string label = "Script", bool readOnly = true)
        {
            GUIHelper.DrawAsReadOnly(readOnly, gui: () =>
            {
                EditorGUILayout.ObjectField(label, MonoScript.FromMonoBehaviour(mono), mono.GetType(), false);
            });
        }

        /// <summary>
        /// Draw the editor script reference from a scriptable object
        /// </summary>
        public static void DrawScriptReference(ScriptableObject so, string label = "Script", bool readOnly = true)
        {
            GUIHelper.DrawAsReadOnly(readOnly, gui: () =>
            {
                EditorGUILayout.ObjectField(label, MonoScript.FromScriptableObject(so), so.GetType(), false);
            });
        }

        /// <summary>
        /// Draw EditorGUI as horizontal with no style
        /// </summary>
        /// <param name="gui">GUI to draw</param>
        /// <param name="guiLayoutOptions">Layout options to apply</param>
        public static void DrawHorizontal(Action gui, params GUILayoutOption[] guiLayoutOptions) => DrawHorizontal(gui, GUIStyle.none, guiLayoutOptions);

        /// <summary>
        /// Draw EditorGUI as horizontal with a style
        /// </summary>
        /// <param name="gui">GUI to draw</param>
        /// <param name="style">Style to apply</param>
        /// <param name="guiLayoutOptions">Layout options to apply</param>
        public static void DrawHorizontal(Action gui, GUIStyle style, params GUILayoutOption[] guiLayoutOptions)
        {
            EditorGUILayout.BeginHorizontal(style, guiLayoutOptions);
            gui?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw EditorGUI as vertical with no style
        /// </summary>
        /// <param name="gui">GUI to draw</param>
        /// <param name="guiLayoutOptions">Layout options to apply</param>
        public static void DrawVertical(Action gui, params GUILayoutOption[] guiLayoutOptions) => DrawVertical(gui, GUIStyle.none, guiLayoutOptions);

        /// <summary>
        /// Draw EditorGUI as vertical with a style
        /// </summary>
        /// <param name="gui">GUI to draw</param>
        /// <param name="style">Style to apply</param>
        /// <param name="guiLayoutOptions">Layout options to apply</param>
        public static void DrawVertical(Action gui, GUIStyle style, params GUILayoutOption[] guiLayoutOptions)
        {
            EditorGUILayout.BeginVertical(style, guiLayoutOptions);
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
        public static void DrawWithConfirm(ref bool active, Action onInactiveGUI, string confirmLabel, Action onConfirm, Action onCancel = null)
            => DrawWithConfirm(ref active, onInactiveGUI, new GUIContent(confirmLabel), onConfirm, onCancel);

        /// <summary>
        /// Draw EditorGUI with a confirm & cancel button
        /// </summary>
        /// <param name="active">Reference bool to determine if the confirm/cancel should be displayed</param>
        /// <param name="confirmLabel">GUIContent Label above the confirm/cancel buttons</param>
        /// <param name="onConfirm">Invoked when the confirm button is clicked</param>
        /// <param name="onCancel">Invoked when the cancel button is clicked</param>
        /// <param name="onInactiveGUI">Invoked when active is false</param>
        public static void DrawWithConfirm(ref bool active, Action onInactiveGUI, GUIContent confirmLabel = null, Action onConfirm = null, Action onCancel = null)
        {
            if (active)
            {
                if (confirmLabel != null)
                {
                    EditorGUILayout.LabelField(confirmLabel);
                }

                bool newActive = active;
                DrawHorizontal(gui: () =>
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

        /// <summary>
        /// Create <paramref name="num"/> number of spaces with <paramref name="width"/> width
        /// </summary>
        /// <param name="width">Width space (default to 6f)</param>
        /// <param name="expand">Whether it should expand or not (default to true)</param>
        /// <param name="num">Number of spaces to draw (default to 1)</param>
        public static void DrawSpaces(float width = 6f, bool expand = true, int num = 1)
        {
            for (int i = 0; i < num; i++)
            {
                EditorGUILayout.Space(width, expand);
            }
        }
    }
}
