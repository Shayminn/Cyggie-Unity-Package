using Cyggie.Main.Runtime.Configurations;
using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Main.Runtime.Utils.Extensions;
using Cyggie.Plugins.Logs;
using System;
using UnityEditor;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// Abstract class for creating a Package Configuration Tab used with <see cref="PackageConfigurationSettings"/> by <see cref="PackageConfigurationEditorWindow"/>
    /// </summary>
    internal abstract class PackageConfigurationTab<TService, TSettings> : AbstractPackageConfigurationTab
        where TService : Service
        where TSettings : PackageConfigurationSettings<TService>
    {
        internal override Type SettingsType => typeof(TSettings);

        /// <inheritdoc/>
        internal override string FileName => $"{SettingsType.Name}.asset";

        /// <inheritdoc/>
        internal override string Title => SettingsType.Name.SplitCamelCase();

        /// <summary>
        /// Settings object
        /// </summary>
        protected PackageConfigurationSettings<TService> _settings = null;

        /// <summary>
        /// Serialized object of settings
        /// </summary>
        protected SerializedObject _serializedObject = null;

        private SerializedProperty _initialize = null;
        private int _maxRetryAttempts = 3;

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if (PackageConfigurationEditorWindow.Window.TryGetSettings<TService>(out ServiceConfigurationSO settings))
            {
                _settings = settings as PackageConfigurationSettings<TService>;
                _serializedObject = new SerializedObject(_settings);
                _initialize = _serializedObject.FindProperty(nameof(ServiceConfigurationSO.Initialize));
            }
            else
            {
                if (_maxRetryAttempts <= 0)
                {
                    Log.Error("Unable to initialize configuration tab: {GetType()}. Maximum number of retry attempts reached.", nameof(PackageConfigurationTab<TService, TSettings>));
                    return;
                }

                PackageConfigurationEditorWindow.RefreshServiceConfigurations(PackageConfigurationEditorWindow.Window.ServiceManagerTab.Settings);

                OnInitialized();
                --_maxRetryAttempts;
            }
        }

        /// <inheritdoc/>
        internal override void DrawTab()
        {
            if (_serializedObject == null) return;

            _serializedObject.Update();

            if (_initialize != null)
            {
                EditorGUILayout.PropertyField(_initialize);
                EditorGUILayout.Space(5);
            }

            DrawGUI();

            _serializedObject.ApplyModifiedProperties();
        }

        #region Overloadable methods

        /// <summary>
        /// Called when the settings object is created right before it is saved in the AssetDatabase
        /// </summary>
        protected virtual void OnSettingsCreated() { }

        /// <summary>
        /// Called when drawing the GUI in the <see cref="PackageConfigurationEditorWindow"/>
        /// </summary>
        protected abstract void DrawGUI();

        #endregion
    }
}
