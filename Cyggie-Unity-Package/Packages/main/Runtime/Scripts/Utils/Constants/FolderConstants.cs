namespace Cyggie.Main.Runtime.Utils.Constants
{
    /// <summary>
    /// Struct that contains all constants related to folder names
    /// </summary>
    public struct FolderConstants
    {
        // Unity folders
        public const string cAssets = "Assets/";
        public const string cResources = "Resources/";
        public const string cStreamingAssets = "StreamingAssets/";

        // Cyggie folders
        public const string cCyggie = "Cyggie/";
        public const string cLogProfiles = "LogProfiles/";
        public const string cScriptableObjects = "ScriptableObjects/";

        // Shortcuts
        public const string cCyggieResources = cCyggie + cResources;
        public const string cCyggieStreamingAssets = cCyggie + cStreamingAssets;
        public const string cCyggieScriptableObjects = cCyggie + cScriptableObjects;

        public const string cPackageConfigurations = cScriptableObjects + "PackageConfigurations/";
        public const string cServiceConfigurations = cScriptableObjects + "ServiceConfigurations/";
    }
}
