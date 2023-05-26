using Cyggie.Main.Runtime.Services;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cyggie.ReferencePool.Runtime.ServicesNS
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
    public sealed class ReferencePoolService : Service
    {
        private Dictionary<ReferencePoolObject, GameObject> _referencePool = new Dictionary<ReferencePoolObject, GameObject>();

        /// <inheritdoc/>
        public override void Start()
        {
            base.Start();

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        /// <summary>
        /// Get the game object reference using the key <paramref name="refPoolObj"/>.
        /// </summary>
        /// <param name="refPoolObj">Referenced scriptable object</param>
        /// <returns>Referenced game object (null if not found)</returns>
        public GameObject GetReference(ReferencePoolObject refPoolObj) => _referencePool.ContainsKey(refPoolObj) ?_referencePool[refPoolObj] : null;

        /// <summary>
        /// Try get a game object reference using the key <paramref name="refPoolObj"/>
        /// </summary>
        /// <param name="refPoolObj">Referenced scriptable object</param>
        /// <param name="gameObject">The referenced game object (null if not found)</param>
        /// <returns>Exists?</returns>
        public bool TryGetReference(ReferencePoolObject refPoolObj, out GameObject gameObject) => _referencePool.TryGetValue(refPoolObj, out gameObject);

        /// <summary>
        /// Add <paramref name="refPoolObj"/> to the pool if it doesn't already exists.
        /// </summary>
        /// <param name="refPoolObj">Reference scriptable object</param>
        /// <param name="gameObject">The game object associated to the referenced scriptable object</param>
        internal void AddToPool(ReferencePoolObject refPoolObj, GameObject gameObject)
        {
            if (_referencePool.ContainsKey(refPoolObj))
            {
                Debug.LogError($"[Reference Pool] Pool already contains reference. Make sure that the reference is not being added multiple times! Reference: {refPoolObj}, GameObject: {gameObject}.");
                return;
            }

            _referencePool.Add(refPoolObj, gameObject);
        }

        private void OnActiveSceneChanged(Scene from, Scene to)
        {
            _referencePool = _referencePool.Where(x => x.Value != null).ToDictionary(x => x.Key, y => y.Value);
        }
    }
}
