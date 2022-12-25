using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyggie.SceneChanger.Runtime.Utils
{
    public struct SceneChangerPaths
    {
        public const string cPackagePath = "Packages/cyggie.scene-changer/";

        public const string cPackageEditorPath = cPackagePath + "Editor/";
        public const string cPackageRuntimePath = cPackagePath + "Runtime/";

        public const string cSettingsPath = cPackageEditorPath + "SceneChangerSettings.asset";
        public const string cLoadingScreenPath = cPackageRuntimePath + "Prefabs/LoadingScreen.prefab";
    }
}
