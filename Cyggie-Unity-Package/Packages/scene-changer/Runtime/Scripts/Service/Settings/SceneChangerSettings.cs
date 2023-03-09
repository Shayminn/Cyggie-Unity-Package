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
        [SerializeField]
        internal LoadingScreen LoadingScreenPrefab = null;

        #region Loading Screen fields

        [SerializeField]
        internal Texture2D[] Images = null;

        [SerializeField]
        internal bool ScaleImageToResolution = true;

        [SerializeField]
        internal bool RandomizeImages = true;

        [SerializeField]
        internal SceneChangeRandomType RandomType = SceneChangeRandomType.ResetAfterEach;

        [SerializeField]
        internal SceneChangeText[] Texts = null;

        [SerializeField]
        internal float MinimumLoadTime = 0f;

        #endregion

        #region Loading Bar fields

        [SerializeField]
        internal Texture2D LoadingBarImage = null;

        [SerializeField]
        internal Color LoadingBarImageColor = Color.gray;

        [SerializeField]
        internal Vector3 LoadingBarPosition = new Vector3(0, -400, 0);

        [SerializeField]
        internal Vector2 LoadingBarSize = new Vector2(1000, 50);

        [SerializeField]
        internal FillMethod LoadingBarFillMethod = FillMethod.Horizontal;

        [SerializeField]
        internal int LoadingBarFillOrigin = 0;

        [SerializeField]
        internal bool PreserveAspectRatio = false;

        [SerializeField]
        internal bool EnableTextProgress = false;

        [SerializeField]
        internal Vector3 TextProgressPosition = Vector3.zero;

        [SerializeField]
        internal Vector2 TextProgressObjectSize = new Vector2(200, 50);

        [SerializeField]
        internal float TextProgressSize = 16f;

        [SerializeField]
        internal Color TextProgressColor = Color.black;

        #endregion

        #region Resolution fields

        [SerializeField]
        internal bool AutoAdjustToResolution = true;

        [SerializeField]
        internal Vector2 ScreenSize = new Vector2(1920, 1080);

        [SerializeField]
        internal float ResolutionCheckDelay = 0.5f;

        #endregion

        internal bool HasImages => Images != null && Images.Length > 0;

        internal bool HasTexts => Texts != null && Texts.Length > 0;

        public override System.Type ServiceType => typeof(SceneChangerService);
    }
}
