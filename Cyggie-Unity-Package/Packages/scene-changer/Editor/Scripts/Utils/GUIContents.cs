using UnityEngine;

namespace Cyggie.SceneChanger.Editor.Utils.Styles
{
    /// <summary>
    /// Styles for all EditorGUI drawings
    /// </summary>
    internal class GUIContents
    {
        // 
        //  Scene Changer Tab
        //
        internal static readonly GUIContent cLoadingScreenPrefab = new GUIContent("Loading Screen Prefab", "Prefab for the loading screen object.");

        internal static readonly GUIContent cImages = new GUIContent("Images", "Images to use as loading screen.");
        internal static readonly GUIContent cScaleImageToResolution = new GUIContent("Scale Image to Resolution", "Auto scale image to resolution or the image will simply be cut off.");
        internal static readonly GUIContent cRandomizeImages = new GUIContent("Randomize Images", "Randomize the order of the image.");
        internal static readonly GUIContent cRandomType = new GUIContent("Random Type", "Type of randomization\n " +
                                 "ResetAfterEach: Resets the list after every load allowing the same images to be used sequentially.\n" +
                                 "ResetAfterEachNoPreviousRepeat: Resets the list but will also make sure the loading image won't ever repeat.\n" +
                                 "RoundRobin: Does not reset the list until every image has been loaded at least once.");
        internal static readonly GUIContent cTexts = new GUIContent("Texts", "Text objects to create for the loading screens.");
        internal static readonly GUIContent cMinimumLoadTime = new GUIContent("Minimum Load Time", "Minimum amount of time to load before changing scene even if the scene is ready to be loaded.");

        internal static readonly GUIContent cLoadingBarImage = new GUIContent("Loading Bar Image", "Image for the loading bar.");
        internal static readonly GUIContent cLoadingBarImageColor = new GUIContent("Loading Bar Image Color", "Color of the image for the loading bar.");
        internal static readonly GUIContent cLoadingBarPosition = new GUIContent("Loading Bar Position", "Position of the loading bar.");
        internal static readonly GUIContent cLoadingBarSize = new GUIContent("Loading Bar Size", "Width and Height of the loading bar.");
        internal static readonly GUIContent cLoadingBarFillMethod = new GUIContent("Loading Bar Fill Method", "Fill method of the loading bar image.");
        internal static readonly GUIContent cLoadingBarFillOrigin = new GUIContent("Loading Bar Fill Origin", "Fill origin of the loading bar image.");
        internal static readonly GUIContent cPreserveAspectRatio = new GUIContent("Preserve Aspect Ratio", "Whether the loading bar image should preseve its aspect ratio.");
        internal static readonly GUIContent cEnableTextProgress = new GUIContent("Enable Text Progress", "Whether the text progress of the loading bar should be displayed.");
        internal static readonly GUIContent cTextProgressPosition = new GUIContent("Text Progress Position", "Position of the text progress.");
        internal static readonly GUIContent cTextProgressObjectSize = new GUIContent("Text Progress Object Size", "Width and Height of the text progress.");
        internal static readonly GUIContent cTextProgressSize = new GUIContent("Text Progress Size", "Font size of the text progress.");
        internal static readonly GUIContent cTextProgressColor = new GUIContent("Text Progress Color", "Color of the text progress.");

        internal static readonly GUIContent cAutoAdjustToResolution = new GUIContent("Auto Adjust to Resolution", "Auto adjust the loading screen to the resolution.");
        internal static readonly GUIContent cScreenSize = new GUIContent("Screen Size", "Fixed resolution screen size to use.");
        internal static readonly GUIContent cResolutionCheckDelay = new GUIContent("Resolution Check Delay", "Delay between each check for a change in resolution.");

        //
        //  Scene Change Text (PropertyDrawer)
        //
        internal static readonly GUIContent cText = new GUIContent("Text", "Text to write.");
        internal static readonly GUIContent cPosition = new GUIContent("Position", "RectTransform's position.");
        internal static readonly GUIContent cObjectSize = new GUIContent("Object Size", "RectTransform's width and height.");
        internal static readonly GUIContent cTextColor = new GUIContent("Text Color", "The color of the Text.");
        internal static readonly GUIContent cTextSize = new GUIContent("Text Size", "The font size of the Text.");
        internal static readonly GUIContent cAlwaysVisible = new GUIContent("Always Visible", "Whether this Text should always be visible.");
        internal static readonly GUIContent cImageSpecific = new GUIContent("Image Specific", "Whether this Text should only be visible to.");
        internal static readonly GUIContent cDisplayAtProgress = new GUIContent("Display at Progress", "Whether this Text should only be displayed when the loading has reached a specific progress.");
    }
}
