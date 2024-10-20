﻿using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Interfaces;
using System;

namespace Cyggie.Plugins.Services.Models
{
    #region Service

    /// <summary>
    /// Service (singleton) without a configuration
    /// </summary>
    public abstract class Service : IService
    {
        /// <inheritdoc/>
        public IServiceManager? Manager { get; set; }

        /// <inheritdoc/>
        protected virtual bool CreateOnInitialize => true;

        /// <summary>
        /// Whether the service has been initialized or not yet
        /// </summary>
        protected internal bool _initialized = false;

        /// <inheritdoc/>
        public void Initialize(IServiceManager manager)
        {
            if (_initialized)
            {
                Log.Error("Trying to initialize a service, but it's already initialized!", nameof(Service));
                return;
            }

            Log.Debug($"Service ({GetType().Name}) initialized with no configuration.", nameof(Service));

            Manager = manager;
            OnInitialized();

            _initialized = true;
        }

        /// <inheritdoc/>
        public virtual void OnAllServicesInitialized() { }

        /// <summary>
        /// Called when the object is disposed by <see cref="IDisposable"/>
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// Called right after the service is initialized
        /// </summary>
        protected virtual void OnInitialized() { }
    }

    #endregion

    #region Service with configuration

    /// <summary>
    /// Service (singleton) with configuration of type <typeparamref name="T"/> 
    /// </summary>
    /// <typeparam name="T">Service configuration type</typeparam>
    public abstract class Service<T> : Service, IServiceWithConfiguration
        where T : IServiceConfiguration
    {
        /// <summary>
        /// Configuration associated to object
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        // Not null is only supported in C# 9+ which is not supported in most Unity versions
        public T Configuration { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <inheritdoc/>
        public Type ConfigurationType => typeof(T);

        /// <inheritdoc/>
        public void SetConfiguration<TConfig>(TConfig config)
            where TConfig : IServiceConfiguration
        {
            Configuration = (T) (IServiceConfiguration) config;
        }

        /// <inheritdoc/>
        public new void Initialize(IServiceManager manager)
        {
            if (_initialized)
            {
                Log.Error("Trying to initialize a service, but it's already initialized!", nameof(Service));
                return;
            }

            Log.Debug($"Service ({GetType().Name}) initialized with configuration ({(Configuration == null ? "none" : Configuration.GetType().Name)}).", nameof(Service));

            Manager = manager;
            OnInitialized();

            _initialized = true;
        }
    }

    #endregion
}
