using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Main.Runtime.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// Editor window for Package Configurations, managing all existing tabs in project <see cref="PackageConfigurationTab"/>
    /// </summary>
    internal class PackageConfigurationEditorWindow : EditorWindow
    {
        private List<PackageConfigurationTab> _tabs = null;
        private PackageConfigurationTab _selectedTab = null;
        private string[] _tabStrings = null;

        private int _selectedTabIndex = 0;
        private Vector2 _tabScrollViewPos = Vector2.zero;

        /// <summary>
        /// Initialize the window with tabs
        /// </summary>
        /// <param name="tabs">List of tabs in projects</param>
        internal void Initialize(List<PackageConfigurationTab> tabs)
        {
            _tabs = tabs;
            _selectedTab = _tabs.FirstOrDefault();

            _tabStrings = _tabs.Select(x => x.ClassName).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void OnGUI()
        {
            if (_tabStrings == null || _tabs == null || _selectedTabIndex >= _tabs.Count)
            {
                Close();
                return;
            }

            EditorGUIHelper.DrawWithScrollview(ref _tabScrollViewPos, gui: () =>
            {
                EditorGUIHelper.DrawHorizontal(gui: () =>
                {
                    EditorGUILayout.Space(5);
                    EditorGUIHelper.DrawVertical(gui: () =>
                    {
                        EditorGUILayout.Space(5);
                        EditorGUIHelper.DrawHorizontal(gui: () =>
                        {
                            EditorGUILayout.LabelField("Select configuration: ", EditorStyles.boldLabel, GUILayout.Width(140));

                            if (EditorGUIHelper.CheckChange(gui: () => _selectedTabIndex = EditorGUILayout.Popup(_selectedTabIndex, _tabStrings)))
                            {
                                _selectedTab = _tabs[_selectedTabIndex];
                            }
                        });
                        EditorGUILayout.Space(10);

                        EditorGUILayout.LabelField(_selectedTab.SettingsType.Name.SplitCamelCase(), EditorStyles.boldLabel);
                        _selectedTab.DrawGUI();

                        EditorGUILayout.Space(5);
                    });
                    EditorGUILayout.Space(5);
                });
            }, horizontalStyle: GUIStyle.none);

            ////EditorGUILayout.Space(5);
            //EditorGUIHelper.DrawHorizontal(gui: () =>
            //{
            //    EditorGUILayout.LabelField("Select configuration: ", EditorStyles.boldLabel, GUILayout.Width(140));

            //    if (EditorGUIHelper.CheckChange(gui: () => _selectedTabIndex = EditorGUILayout.Popup(_selectedTabIndex, _tabStrings)))
            //    {
            //        _selectedTab = _tabs[_selectedTabIndex];
            //    }
            //});

            //EditorGUILayout.Space(10);

            //EditorGUIHelper.DrawWithScrollview(ref _tabScrollViewPos, gui: () =>
            //{
            //    EditorGUIUtility.labelWidth = _selectedTab.LabelWidth;
            //    EditorGUIUtility.fieldWidth = _selectedTab.FieldWidth;

            //    EditorGUILayout.LabelField(_selectedTab.SettingsType.Name.SplitCamelCase(), EditorStyles.boldLabel);
            //    _selectedTab.DrawGUI();
            //}, horizontalStyle: GUIStyle.none, guiLayoutOptions: GUILayout.Width(500));
        }
    }
}
