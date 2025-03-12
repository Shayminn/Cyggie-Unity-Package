using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Cyggie.FileManager.Runtime.Data
{
    /// <summary>
    /// Data model for storing/retrieving objects into data files
    /// </summary>
    internal sealed class SaveFileModel
    {
        [JsonProperty]
        internal Type ObjectType = null;

        [JsonProperty]
        internal string ObjectData = "";

        [JsonIgnore]
        internal SaveableObject Object = null;

        /// <summary>
        /// Constructor used when creating a new save file
        /// </summary>
        /// <param name="obj">Object to save</param>
        internal SaveFileModel(SaveableObject obj)
        {
            ObjectType = obj.GetType();
            Object = obj;
            Object.FileModel = this;
        }

        /// <summary>
        /// Constructor used by <see cref="JsonConvert.DeserializeObject"/> when loading saved objects
        /// </summary>
        /// <param name="objectType">Saveable object type</param>
        /// <param name="objectData">Serialized object</param>
        [JsonConstructor]
        internal SaveFileModel(Type objectType, string objectData)
        {
            ObjectType = objectType;
            Object = (SaveableObject) JsonConvert.DeserializeObject(objectData, objectType);
            Object.FileModel = this;
        }

        /// <summary>
        /// Serialize the object data based on the current values of <see cref="Object"/> <br/>
        /// This should be immediately called right before saving the file
        /// </summary>
        internal void SerializeObjectData()
        {
            ObjectData = JsonConvert.SerializeObject(Object);
        }
    }
}
