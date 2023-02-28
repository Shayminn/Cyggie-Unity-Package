using Cyggie.Main.Runtime.Configurations;
using Cyggie.SceneChanger.Runtime.Enums;
using Cyggie.SceneChanger.Runtime.Services;
using Cyggie.SceneChanger.Runtime.Settings;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Cyggie.SceneChanger.Runtime.Configurations
{
    /// <summary>
    /// Settings for <see cref="SceneChangerService"/>
    /// </summary>
    internal class SceneChangerSettings : PackageConfigurationSettings
    {
        internal const string cResourcesPath = ConfigurationSettings.cResourcesFolderPath + nameof(SceneChangerSettings);

        /// <summary>
        /// Loading Screen prefab object
        /// </summary>
        [SerializeField, Tooltip("Prefab for the loading screen object.")]
        internal LoadingScreen LoadingScreenPrefab = null;

        #region Loading Screen fields

        [SerializeField, Tooltip("Images to use as loading screen.")]
        internal Texture2D[] Images = null;

        [SerializeField, Tooltip("Auto scale image to resolution or the image will simply be cut off.")]
        internal bool ScaleImageToResolution = true;

        [SerializeField, Tooltip("Randomize the order of the image.")]
        internal bool RandomizeImages = true;

        [SerializeField, Tooltip("Type of randomization\n " +
                                 "ResetAfterEach: Resets the list after every load allowing the same images to be used sequentially.\n" +
                                 "ResetAfterEachNoPreviousRepeat: Resets the list but will also make sure the loading image won't ever repeat.\n" +
                                 "RoundRobin: Does not reset the list until every image has been loaded at least once.")]
        internal SceneChangeRandomType RandomType = SceneChangeRandomType.ResetAfterEach;

        [SerializeField, Tooltip("Text objects to create for the loading screens.")]
        internal SceneChangeText[] Texts = null;

        [SerializeField, Tooltip("Minimum amount of time to load before changing scene even if the scene is ready to be loaded.")]
        internal float MinimumLoadTime = 0f;

        #endregion

        #region Loading Bar fields

        [SerializeField, Tooltip("Image for the loading bar.")]
        internal Texture2D LoadingBarImage = null;

        [SerializeField, Tooltip("Color of the image for the loading bar.")]
        internal Color LoadingBarImageColor = Color.gray;

        [SerializeField, Tooltip("Position of the loading bar.")]
        internal Vector3 LoadingBarPosition = new Vector3(0, -400, 0);

        [SerializeField, Tooltip("Width and Height of the loading bar.")]
        internal Vector2 LoadingBarSize = new Vector2(1000, 50);

        [SerializeField, Tooltip("Fill method of the loading bar image.")]
        internal FillMethod LoadingBarFillMethod = FillMethod.Horizontal;

        [SerializeField, Tooltip("Fill origin of the loading bar image.")]
        internal int LoadingBarFillOrigin = 0;

        [SerializeField, Tooltip("Whether the loading bar image should preseve its aspect ratio.")]
        internal bool PreserveAspectRatio = false;

        [SerializeField, Tooltip("Whether the text progress of the loading bar should be displayed.")]
        internal bool EnableTextProgress = false;

        [SerializeField, Tooltip("Position of the text progress.")]
        internal Vector3 TextProgressPosition = Vector3.zero;

        [SerializeField, Tooltip("Width and Height of the text progress.")]
        internal Vector2 TextProgressObjectSize = new Vector2(200, 50);

        [SerializeField, Tooltip("Font size of the text progress.")]
        internal float TextProgressSize = 16f;

        [SerializeField, Tooltip("Color of the text progress.")]
        internal Color TextProgressColor = Color.black;

        #endregion

        #region Resolution fields

        [SerializeField, Tooltip("Auto adjust the loading screen to the resolution.")]
        internal bool AutoAdjustToResolution = true;

        [SerializeField, Tooltip("Fixed resolution screen size to use.")]
        internal Vector2 ScreenSize = new Vector2(1920, 1080);

        [SerializeField, Tooltip("Delay between each check for a change in resolution.")]
        internal float ResolutionCheckDelay = 0.5f;

        #endregion

        internal bool HasImages => Images != null && Images.Length > 0;

        internal bool HasTexts => Texts != null && Texts.Length > 0;

        public override System.Type ServiceType => typeof(SceneChangerService);
    }
}
