using Cyggie.Main.Runtime.ServicesNS;

namespace Cyggie.Main.Runtime.Configurations
{
    /// <summary>
    /// Abstract class for creating a new Package Configuration Settings used with <see cref="PackageConfigurationTab"/> <br/>
    /// These Package Configuration are created only within the Cyggie package and can't/shouldn't be created outside <br/>
    /// They inherit from <see cref="ServiceConfiguration"/> to easily apply these Settings to the <see cref="Service"/> <br/>
    /// And they are automatically un-listed from the <see cref="ServiceManagerSettings"/> and it's editor tab
    /// </summary>
    internal abstract class PackageConfigurationSettings<T> : ServiceConfiguration<T> where T : Service
    {

#if UNITY_EDITOR

        /// <inheritdoc/>
        internal override bool IsPackageSettings => true;

#endif

    }
}
