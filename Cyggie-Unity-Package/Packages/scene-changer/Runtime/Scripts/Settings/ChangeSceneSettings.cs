namespace Cyggie.SceneChanger.Runtime.Settings
{
    /// <summary>
    /// Model class used for <see cref="SceneChanger.ChangeScene"/> to enable/disable related features
    /// </summary>
    public class ChangeSceneSettings
    {
        public bool EnableLoadingScreen { get; set; }

        public bool EnableLoadingBar { get; set; }

        public ChangeSceneFade FadeIn { get; set; } = null;

        public ChangeSceneFade FadeOut { get; set; } = null;

        public int[] TextIndexes { get; set; } = null;

        public ChangeSceneInputSettings InputSettings { get; set; } = null;

        public bool HasFadeIn => FadeIn != null;

        public bool HasFadeOut => FadeOut != null;

        public bool HasTextIndexes => TextIndexes != null && TextIndexes.Length > 0;

        public bool HasInputSettings => InputSettings != null;

        #region Static defaults

        /// <summary>
        /// Enables loading screen and loading bar
        /// </summary>
        public static readonly ChangeSceneSettings Default = new ChangeSceneSettings()
        {
            EnableLoadingScreen = true,
            EnableLoadingBar = true,
            FadeIn = null,
            FadeOut = null,
            TextIndexes = null,
            InputSettings = null
        };

        /// <summary>
        /// Enables loading screen and loading bar with wait for keyboard input
        /// </summary>
        public static readonly ChangeSceneSettings DefaultWithKeyboardInput = new ChangeSceneSettings()
        {
            EnableLoadingScreen = true,
            EnableLoadingBar = true,
            FadeIn = null,
            FadeOut = null,
            TextIndexes = null,
            InputSettings = ChangeSceneInputSettings.DefaultKeyboard
        };

        /// <summary>
        /// Enables loading screen and loading bar with wait for keyboard or mouse input
        /// </summary>
        public static readonly ChangeSceneSettings DefaultWithKeyboardAndMouseInput = new ChangeSceneSettings()
        {
            EnableLoadingScreen = true,
            EnableLoadingBar = true,
            FadeIn = null,
            FadeOut = null,
            TextIndexes = null,
            InputSettings = ChangeSceneInputSettings.DefaultKeyboardAndMouse
        };

        /// <summary>
        /// Enables loading screen, loading bar, fade in and fade out
        /// </summary>
        public static readonly ChangeSceneSettings EnableAll = new ChangeSceneSettings()
        {
            EnableLoadingScreen = true,
            EnableLoadingBar = true,
            FadeIn = ChangeSceneFade.Default,
            FadeOut = ChangeSceneFade.Default,
            TextIndexes = null,
        };

        /// <summary>
        /// Enables loading screen, loading bar, fade in and fade out with wait for keyboard input
        /// </summary>
        public static readonly ChangeSceneSettings EnableAllWithKeyboardInput = new ChangeSceneSettings()
        {
            EnableLoadingScreen = true,
            EnableLoadingBar = true,
            FadeIn = ChangeSceneFade.Default,
            FadeOut = ChangeSceneFade.Default,
            InputSettings = ChangeSceneInputSettings.DefaultKeyboard
        };

        /// <summary>
        /// Enables loading screen, loading bar, fade in and fade out with wait for keyboard or mouse input
        /// </summary>
        public static readonly ChangeSceneSettings EnableAllWithKeyboardAndMouseInput = new ChangeSceneSettings()
        {
            EnableLoadingScreen = true,
            EnableLoadingBar = true,
            FadeIn = ChangeSceneFade.Default,
            FadeOut = ChangeSceneFade.Default,
            InputSettings = ChangeSceneInputSettings.DefaultKeyboardAndMouse
        };

        /// <summary>
        /// Enables fade in and fade out, without any loading screen/loading bar
        /// </summary>
        public static readonly ChangeSceneSettings FadeOnly = new ChangeSceneSettings()
        {
            EnableLoadingScreen = false,
            EnableLoadingBar = false,
            FadeIn = ChangeSceneFade.Default,
            FadeOut = ChangeSceneFade.Default
        };

        #endregion

        /// <summary>
        /// Default constructor with params of Text indexes to use
        /// </summary>
        /// <param name="textIndexes">Text indexes to explicitly use</param>
        public ChangeSceneSettings(params int[] textIndexes)
        {
            TextIndexes = textIndexes;
        }

        /// <summary>
        /// Constructor with existing settings to add Text Indexes to use
        /// </summary>
        /// <param name="settings">Existing settings</param>
        /// <param name="textIndexes">Text indexes to explicitly use</param>
        public ChangeSceneSettings(ChangeSceneSettings settings, params int[] textIndexes)
        {
            EnableLoadingScreen = settings.EnableLoadingScreen;
            EnableLoadingBar = settings.EnableLoadingBar;
            FadeIn = settings.FadeIn;
            FadeOut = settings.FadeOut;
            TextIndexes = textIndexes;
        }
    }
}
