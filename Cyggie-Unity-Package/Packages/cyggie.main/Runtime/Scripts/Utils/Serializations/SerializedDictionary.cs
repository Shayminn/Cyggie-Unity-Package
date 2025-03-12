using System;
using System.Collections;
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
    public abstract class SerializedDictionary<TKey, TValue> :
        AbstractSerializedDictionary,
        IDictionary<TKey, TValue>,
        ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        [SerializeField]
        internal TKey _addKey = default;
#endif

        [SerializeField]
        private List<TKey> _keys = new List<TKey>();

        [SerializeField]
        private List<TValue> _values = new List<TValue>();

        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Before serialize <br/>
        /// Copy values from dictionary to serialized fields
        /// </summary>
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (KeyValuePair<TKey, TValue> kv in _dictionary)
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
            Clear();

            for (int i = 0; i < _keys.Count; i++)
            {
                Add(_keys[i], _values[i]);
            }
        }

        #region Dictionary interface

        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;
        public TValue this[TKey key] { get => _dictionary[key]; set => _dictionary[key] = value; }

        public void Add(TKey key, TValue value) => _dictionary.Add(key, value);
        public void Add(KeyValuePair<TKey, TValue> item) => _dictionary.Add(item.Key, item.Value);
        public void Clear() => _dictionary.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.ContainsKey(item.Key) && _dictionary.ContainsValue(item.Value);
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
        public bool Remove(TKey key) => _dictionary.Remove(key);
        public bool Remove(KeyValuePair<TKey, TValue> item) => _dictionary.Remove(item.Key);
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int numElements = arrayIndex + _dictionary.Count;
            array = array[0..numElements];

            foreach (KeyValuePair<TKey, TValue> kv in _dictionary)
            {
                array[arrayIndex] = kv;
                arrayIndex++;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

        #endregion
    }
}
