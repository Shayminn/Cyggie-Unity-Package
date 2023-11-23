namespace Cyggie.Plugins.Utils.Enums
{
    /// <summary>
    /// Enum that contains various of the existing aspect ratios <br/>
    /// Use with <see cref="AspectRatioHelper"/> to retrieve an aspect ratio based on width and height
    /// </summary>
    public enum AspectRatio
    {
        /// <summary>
        /// eg. 720x1280, 768x1366, 900x1600, 1080x1920, 1440x2560, 2160x3840, 2880x5120, 4320x7680
        /// </summary>
        _9x16,

        /// <summary>
        /// eg. 500x750, 720x1080, 1500x2250
        /// </summary>
        _2x3,

        /// <summary>
        /// eg. 1280x720, 1366x768, 1600x900, 1920x1080, 2560x1440, 3840x2160, 5120x2880, 7680x4320
        /// </summary>
        _16x9,

        /// <summary>
        /// eg. 1280x800, 1920x1200, 2560x1600
        /// </summary>
        _16x10,

        /// <summary>
        /// eg. 270x180
        /// </summary>
        _3x2,

        /// <summary>
        /// eg. 1400x1050, 1440x1080, 1600x1200, 1920x1440, 2048x1536
        /// </summary>
        _4x3,

        /// <summary>
        /// eg. 1080x1350
        /// </summary>
        _5x4,

        /// <summary>
        /// eg. 2560x1080, 3440x1440, 3840x1600, 5120x2160
        /// </summary>
        _21x9,

        /// <summary>
        /// eg. 3840x1080, 5120x1440, 7680x2160
        /// </summary>
        _32x9
    }
}
