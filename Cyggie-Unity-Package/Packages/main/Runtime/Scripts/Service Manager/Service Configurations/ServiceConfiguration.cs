using System;
using UnityEngine;

namespace Cyggie.Main.Runtime.ServicesNS
{
    /// <summary>
    /// Configuration class to apply to a <see cref="Service"/>
    /// </summary>
    public abstract class ServiceConfiguration<T> : ServiceConfigurationSO where T : Service
    {
        /// <inheritdoc/>
        public sealed override Type ServiceType => typeof(T);
    }
}
