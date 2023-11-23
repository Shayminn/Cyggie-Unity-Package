using Cyggie.Main.Runtime.ServicesNS.ScriptableObjects;
using Cyggie.SceneChanger.Runtime.Enums;
using Cyggie.SceneChanger.Runtime.ServicesNS;
using Cyggie.SceneChanger.Runtime.Settings;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Cyggie.SceneChanger.Runtime.Configurations
{
    /// <summary>
    /// Settings for <see cref="SceneChangerService"/>
    /// </summary>
    public class SceneChangerServiceConfiguration : PackageServiceConfiguration
    {
        /// <summary>
        /// Loading Screen prefab object
        /// </summary>
        [SerializeField, Tooltip("Loading screen prefab")]
        internal LoadingScreen LoadingScreenPrefab = null;

        #region Loading Screen fields

        [SerializeField, Tooltip("Array of images for the loading screen prefab.")]
        public Texture2D[] Images = null;

        [SerializeField, Tooltip("Whether the image should scale to the resolution.")]
        public bool ScaleImageToResolution = true;

        [SerializeField, Tooltip("Whether the images are randomized.")]
        public bool RandomizeImages = true;

        [SerializeField, Tooltip("Type of randomization.")]
        public SceneChangeRandomType RandomType = SceneChangeRandomType.ResetAfterEach;

        [SerializeField, Tooltip("Array of texts that appears in the loading screen.")]
        public SceneChangeText[] Texts = null;

        [SerializeField, Tooltip("Minimum load time before changing scene.")]
        public float MinimumLoadTime = 0f;

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

        public bool HasImages => Images != null && Images.Length > 0;

        public bool HasTexts => Texts != null && Texts.Length > 0;
    }
}
