using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyggie.SceneChanger.Runtime.Utils.Constants
{
    internal struct SceneChangerPaths
    {
        public const string cPackage = "Packages/cyggie.scene-changer/";
        public const string cRuntime = cPackage + "Runtime/";

        public const string cResources = cRuntime + "Resources/";
        public const string cPrefabs = cRuntime + "Prefabs/";

        public const string cSettings = cResources + SceneChangerConstants.cSettingsFileWithExtension;
        public const string cLoadingScreenPrefab = cRuntime + "Prefabs/LoadingScreen.prefab";
    }
}
