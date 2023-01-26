using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cyggie.Main.Runtime.Serializations
{
    /// <summary>
    /// Serializable dictionary, implementation based on SerializedDictionary from "com.unity.render-pipelines.cores"
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> _keys = new List<TKey>();

        [SerializeField]
        private List<TValue> _values = new List<TValue>();

        /// <summary>
        /// Before serialize <br/>
        /// Copy values from Dictionary to serialized fields
        /// </summary>
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (KeyValuePair<TKey, TValue> kv in this)
            {
                _keys.Add(kv.Key);
                _values.Add(kv.Value);
            }
        }

        /// <summary>
        /// After deserialize <br/>
        /// Copy values from serialized fields to dictionary
        /// </summary>
        public void OnAfterDeserialize()
        {
            for (int i = 0; i < _keys.Count; i++)
            {
                Add(_keys[i], _values[i]);
            }

            _keys.Clear();
            _values.Clear();
        }
    }
}
