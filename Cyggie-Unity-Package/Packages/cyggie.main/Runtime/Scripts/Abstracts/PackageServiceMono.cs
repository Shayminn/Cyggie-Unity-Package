using Cyggie.Main.Runtime.ServicesNS.ScriptableObjects;

namespace Cyggie.Main.Runtime.ServicesNS
{
    #region Service

    /// <summary>
    /// Package service different from <see cref="ServiceMono"/> <br/>
    /// It is not creatable from the Cyggie/Create windows <br/>
    /// Classes that implement this must be created in its own ways (i.e. CreateAssetMenu attribute)
    /// </summary>
    public abstract class PackageServiceMono : ServiceMono
    {
    }

    #endregion

    #region Service with configuration

    /// <summary>
    /// Package service different from <see cref="ServiceMono{T}"/> <br/>
    /// It is not creatable from the Cyggie/Create windows <br/>
    /// Classes that implement this must be created in its own ways (i.e. CreateAssetMenu attribute)
    /// </summary>
    /// <typeparam name="T">Package service configuration type</typeparam>
    public abstract class PackageServiceMono<T> : ServiceMono<T>
        where T : PackageServiceConfiguration
    {
    }

    #endregion
}
