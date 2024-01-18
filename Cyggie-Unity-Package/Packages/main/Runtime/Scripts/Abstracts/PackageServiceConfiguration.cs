using Cyggie.Plugins.UnityServices.Models;
using System;

namespace Cyggie.Main.Runtime.ServicesNS.ScriptableObjects
{
    /// <summary>
    /// Package service configuration <br/>
    /// This type of service configuration is not shown when using the Cyggie/Create windows <br/>
    /// They are automatically created when assigning the Identifier
    /// </summary>
    [Serializable]
    public abstract class PackageServiceConfiguration : ServiceConfigurationSO
    {
    }
}
