using Cyggie.Main.Runtime.Utils.Extensions;
using System;

namespace Cyggie.Main.Editor.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    internal abstract class AbstractPackageConfigurationTab
    {
        /// <summary>
        /// 
        /// </summary>
        internal abstract Type SettingsType { get; }

        /// <summary>
        /// File name to the settings (includes extension .asset)
        /// </summary>
        internal abstract string FileName { get; }

        /// <summary>
        /// Title for the Settings Tab that appears in the Package Configuration Window
        /// </summary>
        internal abstract string Title { get; }

        /// <summary>
        /// Dropdown name for the tab (used to display in Window selection popup)
        /// </summary>
        internal virtual string DropdownName => GetType().Name.Replace("Tab", "").SplitCamelCase();

        /// <summary>
        /// Any other file paths that the settings require <br/>
        /// The file paths referenced here will be moved altogether with <see cref="ConfigurationSettings.ResourcesPath"/> <br/>
        /// </summary>
        internal virtual string[] SettingsOtherPaths { get; }

        /// <summary>
        /// Draw the package configuration settings tab
        /// </summary>
        internal abstract void DrawTab();

        /// <summary>
        /// 
        /// </summary>
        internal void CallInitialized() => OnInitialized();

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnInitialized() { }
    }
}
