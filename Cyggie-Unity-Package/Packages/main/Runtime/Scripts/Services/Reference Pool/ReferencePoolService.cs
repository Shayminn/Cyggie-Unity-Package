using Cyggie.Main.Runtime.Configurations;
using Cyggie.Plugins.Logs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cyggie.Main.Runtime.ServicesNS.ReferencePool
{
    /// <summary>
    /// Service that manages a Reference Pool <br/>
    /// </br>
    /// Instructions <br/>
    /// Create new <see cref="ReferencePoolObject"/> through Create/Cyggie/Reference Pool/ReferencePoolObject <br/>
    /// Add component <see cref="ReferencePoolInitializer"/> to the game object that needs reference <br/>
    /// Retrive the game object from anywhere using <see cref="GetReference(ReferencePoolObject)"/> <br/>
    /// <br/>
    /// References get reset when changing scenes if they don't exist (is null) anymore.
    /// </summary>
    public sealed class ReferencePoolService : PackageServiceMono
    {
        private Dictionary<ReferencePoolObject, GameObject> _referencePool = new Dictionary<ReferencePoolObject, GameObject>();

        protected override int Priority => int.MaxValue;

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            base.OnEnable();

            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        /// <inheritdoc/>
        protected override void OnDisable()
        {
            base.OnDisable();

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        /// <summary>
        /// Get the game object reference using the key <paramref name="refObj"/>.
        /// </summary>
        /// <param name="refObj">Referenced scriptable object</param>
        /// <returns>Referenced game object (null if not found)</returns>
        public GameObject GetReference(ReferencePoolObject refObj)
        {
            TryGetReference(refObj, out GameObject obj);

            return obj;
        }

        /// <summary>
        /// Try get a game object reference using the key <paramref name="refObj"/>
        /// </summary>
        /// <param name="refObj">Referenced scriptable object</param>
        /// <param name="gameObject">The referenced game object (null if not found)</param>
        /// <returns>Exists?</returns>
        public bool TryGetReference(ReferencePoolObject refObj, out GameObject gameObject)
        {
            gameObject = null;
            if (refObj == null)
            {
                Log.Error($"Argument {nameof(refObj)} is null.", nameof(ReferencePoolService));
                return false;
            }

            if (!_referencePool.TryGetValue(refObj, out gameObject))
            {
                Log.Error($"Reference object not found: {refObj.name}. Make sure this is called after Awake. {nameof(ReferencePoolInitializer)}s are called in Awake and the order of execution is not guaranteed.", nameof(ReferencePoolService));
            }

            return gameObject != null;
        }

        /// <summary>
        /// Add <paramref name="refPoolObj"/> to the pool if it doesn't already exists.
        /// </summary>
        /// <param name="refPoolObj">Reference scriptable object</param>
        /// <param name="gameObject">The game object associated to the referenced scriptable object</param>
        internal void AddToPool(ReferencePoolObject refPoolObj, GameObject gameObject)
        {
            if (_referencePool.ContainsKey(refPoolObj))
            {
                Log.Error($"Pool already contains reference. Make sure that the reference is not being added multiple times! Reference: {refPoolObj}, GameObject: {gameObject}.", nameof(ReferencePoolService));
                return;
            }

            _referencePool.Add(refPoolObj, gameObject);
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (_referencePool.Count > 0)
            {
                _referencePool = _referencePool.Where(x => x.Value != null).ToDictionary(x => x.Key, y => y.Value);
            }
        }
    }
}
