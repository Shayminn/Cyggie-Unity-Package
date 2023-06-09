using Cyggie.Encryption;
using Cyggie.FileManager.Runtime.Data;
using Cyggie.Main.Runtime;
using Cyggie.Main.Runtime.ServicesNS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cyggie.FileManager.Runtime.ServicesNS
{
    /// <summary>
    /// Service for loading/saving files on the local device with the Newtonsoft's JsonConvert (w/ encryption) <br/>
    /// This supports (mostly) everything but Anonymous classes to be saved on files
    /// </summary>  
    public sealed class FileManagerService : Service
    {
        private string _savePath = "";
        private bool _encrypt = true;
        private string[] _filesToIgnore = null;
        private string _fileExtension = "";

        internal string DefaultFileExtension => _fileExtension;

        /// <inheritdoc/>
        protected override int Priority => 1;

        /// <summary>
        /// Dictionary of all loaded saved objects
        /// </summary>
        private List<SaveFileModel> _savedModels = new List<SaveFileModel>();

        private FileManagerSettings _settings = null;

        /// <inheritdoc/>
        protected override void OnInitialized(ServiceConfigurationSO configuration)
        {
            base.OnInitialized(configuration);

            // Get the service configuration
            if (configuration == null ||
                configuration is not FileManagerSettings fileManagerSettings)
            {
                Log.Error($"Configuration is null or is not type of {typeof(FileManagerSettings)}.", nameof(FileManagerService));
                return;
            }

            _settings = fileManagerSettings;
            _savePath = _settings.UsePersistentDataPath ?
                        Application.persistentDataPath :
                        _settings.LocalSavePath;

            // Make sure the save path is not null or empty
            if (string.IsNullOrEmpty(_savePath))
            {
                Log.Error($"String {nameof(_savePath)} is null or empty.", nameof(FileManagerService));
                return;
            }

            // Make sure the directory exists for the save path
            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_savePath));
            }

            // Make sure the save path ends as a folder (w/ "/")
            if (!_savePath.EndsWith('/'))
            {
                _savePath = _savePath.Insert(_savePath.Length, "/");
            }

            _encrypt = _settings.Encrypted;
            _filesToIgnore = _settings.FilesToIgnore;
            _fileExtension = _settings.DefaultFileExtension;

            LoadSavedObjects();
        }

        /// <summary>
        /// Create a new <see cref="SaveableObject"/> of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Saveable Object type</typeparam>
        /// <returns>Created object (null if <typeparamref name="T"/> is <see cref="SaveableObject"/>)</returns>
        public T CreateObject<T>() where T : SaveableObject
        {
            if (typeof(T) == typeof(SaveableObject))
            {
                Log.Error($"Unable to create object of abstract class {typeof(SaveableObject)}.", nameof(FileManagerService));
                return null;
            }

            T obj = Activator.CreateInstance<T>();
            SaveObject(obj);

            return obj;
        }

        /// <summary>
        /// Save an object to a file in a local path (creating a new file if it doesn't already exists)
        /// </summary>
        /// <typeparam name="T">Type of SaveableObject</typeparam>
        /// <param name="obj">Object to save</param>
        /// <returns>Successful?</returns>
        public bool SaveObject<T>(T obj) where T : SaveableObject
        {
            // Create new path
            bool exists = _savedModels.Any(x => x.Object == obj);
            string newPath = GetUniqueFilePath(obj, true);
            string oldPath = exists ? GetUniqueFilePath(obj, false) : "";

            // Assign the file name for saving in the future
            obj.FileName = Path.GetFileNameWithoutExtension(newPath);

            // Get or create save file model
            SaveFileModel model = exists ? obj.FileModel : new SaveFileModel(obj);
            model.SerializeObjectData();

            // Serialize the model
            if (!TrySerializeObject(model, out string fileContent)) return false;

            // Write the data to the file
            File.WriteAllText(newPath, fileContent);
            if (exists)
            {
                // Delete the old save file
                File.Delete(oldPath);
            }
            else
            {
                obj.FileModel = model;
                _savedModels.Add(model);
            }

            return true;
        }

        /// <summary>
        /// Delete a saved object file from local path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void DeleteObject<T>(T obj) where T : SaveableObject
        {
            string path = GetUniqueFilePath(obj, false);
            File.Delete(path);
        }

        /// <summary>
        /// Get an enumerable of all saved objects of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> Get<T>() where T : SaveableObject
        {
            return _savedModels.Where(x => x.ObjectType == typeof(T)).Select(x => (T)x.Object);
        }

        /// <summary>
        /// Delete all saved files at local path (excluding ignore files from configuration)
        /// </summary>
        public void DeleteAll()
        {
            string[] files = Directory.GetFiles(_savePath);
            foreach (string path in files)
            {
                if (_filesToIgnore.Length > 0)
                {
                    string file = Path.GetFileName(path);
                    if (_filesToIgnore.Contains(file)) continue;
                }

                File.Delete(path);
            }
        }

        /// <summary>
        /// Load (and deserialize) all saved object files in the local path
        /// </summary>
        private void LoadSavedObjects()
        {
            string[] files = Directory.GetFiles(_savePath);
            string data;
            foreach (string path in files)
            {
                if (_filesToIgnore.Length > 0)
                {
                    string file = Path.GetFileName(path);
                    if (_filesToIgnore.Contains(file)) continue;
                }

                data = File.ReadAllText(path);
                if (TryDeserializeObject(data, out SaveFileModel saveFile))
                {
                    _savedModels.Add(saveFile);
                }
            }
        }

        /// <summary>
        /// Try to serialize an object
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="data">Output string json data</param>
        /// <returns>Success?</returns>
        private bool TrySerializeObject(object obj, out string data)
        {
            data = "";
            try
            {
                data = JsonConvert.SerializeObject(obj);

                if (_encrypt)
                {
                    data = AESEncryptor.Encrypt(data);
                }

                return true;
            }
            catch (JsonSerializationException ex)
            {
                Log.Error($"Unable to serialize data: {obj}, exception: {ex}.", nameof(FileManagerService));
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"Unknown error occured, exception: {ex}.", nameof(FileManagerService));
                return false;
            }
        }

        /// <summary>
        /// Try to deserialize data from file to a <see cref="SaveFileModel"/>
        /// </summary>
        /// <param name="data">Data from the saved file</param>
        /// <param name="saveFile">Output file model object that contains <see cref="SaveableObject"/></param>
        /// <returns>Success?</returns>
        private bool TryDeserializeObject(string data, out SaveFileModel saveFile)
        {
            saveFile = default;
            if (string.IsNullOrEmpty(data))
            {
                Log.Error($"String {nameof(data)} is null or empty.", nameof(FileManagerService));
                return false;
            }

            if (_encrypt)
            {
                data = AESEncryptor.Decrypt(data);
            }

            try
            {
                saveFile = JsonConvert.DeserializeObject<SaveFileModel>(data);
                return true;
            }
            catch (JsonSerializationException ex)
            {
                Log.Error($"Unable to deserialize data to {typeof(SaveFileModel)}: \"{data}\". Exception: {ex}.", nameof(FileManagerService));
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"Unknown error occured, exception: {ex}.", nameof(FileManagerService));
                return false;
            }
        }

        /// <summary>
        /// Get the unique path to an object for file creation <br/>
        /// This will append "(index)" if the file name already exists
        /// </summary>
        /// <param name="obj">Object to get the unique path to</param>
        /// <param name="newPath">Whether this is a new path or an existing (it will retrieve the object's path)</param>
        /// <returns>Unique file path</returns>
        private string GetUniqueFilePath(SaveableObject obj, bool newPath)
        {
            // Build full path to file name
            StringBuilder pathBuilder = new StringBuilder(_savePath);
            obj.AppendFilePath(pathBuilder, newPath);

            if (newPath)
            {
                // Check if it is unique
                bool isUnique = !File.Exists(pathBuilder.ToString());

                // Add (index) to file name till it is unique
                if (!isUnique)
                {
                    int tryCount = 1;
                    while (!isUnique)
                    {
                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(pathBuilder.ToString());
                        pathBuilder.Replace(fileNameWithoutExt, $"{fileNameWithoutExt} ({tryCount})");

                        isUnique = !File.Exists(pathBuilder.ToString());
                        ++tryCount;
                    }
                }
            }

            return pathBuilder.ToString();
        }
    }
}
