using Cyggie.FileManager.Runtime.Services;
using Cyggie.Main.Runtime.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Cyggie.FileManager.Runtime.Data
{
    /// <summary>
    /// Model class to derive for an object that can be saved in local files <br/>
    /// Use <see cref="FileManagerService.CreateObject"/> to create an object of this type
    /// </summary>
    public abstract class SaveableObject
    {
        private static FileManagerService _fileManagerService = null;
        internal static FileManagerService FileManagerService => _fileManagerService ??= ServiceManager.Get<FileManagerService>();

        /// <summary>
        /// This object's save file name
        /// </summary>
        [JsonProperty]
        internal string FileName = string.Empty;

        /// <summary>
        /// The file model that this object associates to
        /// </summary>
        [JsonIgnore]
        internal SaveFileModel FileModel = null;

        /// <summary>
        /// The subfolder path that this object's ave field should be saved to
        /// </summary>
        protected virtual string SubfolderPath { get; private set; } = string.Empty;

        /// <summary>
        /// The file extension that this object's save file should end with
        /// </summary>
        protected virtual string FileExtension { get; private set; } = string.Empty;

        /// <summary>
        /// Constructor with optional to save object to file immediately on object creation
        /// </summary>
        /// <param name="saveObject">Whether to save this object immediately or later (defaults to false)</param>
        public SaveableObject(bool saveObject = false)
        {
            if (saveObject)
            {
                Save();
            }
        }

        /// <summary>
        /// Save an object to a file in a local path (creating a new file if it doesn't already exists)
        /// </summary>
        public void Save()
        {
            FileManagerService.SaveObject(this);
        }

        /// <summary>
        /// Delete a saved object file from local path
        /// </summary>
        public void Delete()
        {
            FileManagerService.DeleteObject(this);
        }

        /// <summary>
        /// Append this file's file path 
        /// </summary>
        /// <param name="pathBuilder">Pathbuilder object</param>
        /// <param name="newPath">Whether this path is a new path</param>
        internal void AppendFilePath(StringBuilder pathBuilder, bool newPath)
        {
            // Append subfolder path
            if (!string.IsNullOrEmpty(SubfolderPath))
            {
                if (!SubfolderPath.EndsWith('/'))
                {
                    SubfolderPath = $"{SubfolderPath}/";
                }

                pathBuilder.Append(SubfolderPath);
                Directory.CreateDirectory(pathBuilder.ToString());
            }

            // Append file name
            pathBuilder.Append(newPath ? DateTimeOffset.Now.ToUnixTimeSeconds() : FileName);

            // Append file extension
            if (string.IsNullOrEmpty(FileExtension))
            {
                FileExtension = FileManagerService.DefaultFileExtension;
            }
            else
            {
                if (!FileExtension.StartsWith('.'))
                {
                    FileExtension = $".{FileExtension}";
                }
            }

            pathBuilder.Append(FileExtension);
        }
    }
}