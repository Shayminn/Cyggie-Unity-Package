using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cyggie.Main.Runtime.Configurations
{
    internal class ConfigurationSettings : ScriptableObject
    {
        internal const string cFileName = "ConfigurationSettings.asset";
        internal const string cDefaultFolderPath = "Assets/Resources/Cyggie/Package Configurations/";

        [SerializeField, Tooltip("")]
        internal string ConfigurationsPath = cDefaultFolderPath;
    }
}
