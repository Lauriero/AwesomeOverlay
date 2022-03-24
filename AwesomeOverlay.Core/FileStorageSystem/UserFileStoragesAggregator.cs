
using System;
using System.Collections.Generic;
using System.IO;

namespace AwesomeOverlay.Core.FileStorageSystem
{
    /// <summary>
    /// Contains data and logic for aggregate all the file storages which are used by specific user
    /// </summary>
    public class UserFileStoragesAggregator
    {
        private DirectoryInfo _storageFolder;
        private Dictionary<string, FileStorage> _storages = new Dictionary<string, FileStorage>();

        public FileStorage this[string storageName] {
            get { return _storages[storageName]; }
        }

        public UserFileStoragesAggregator(string parentFolderPath, int userId)
        {
            _storageFolder = new DirectoryInfo(Path.Combine(parentFolderPath, userId.ToString()));

            if (!_storageFolder.Exists)
                _storageFolder.Create();
        }

        public UserFileStoragesAggregator AddStorage(string storageName)
        {
            if (_storages.ContainsKey(storageName)) {
                throw new Exception("This storage user is already registered");
            }           

            FileStorage storages = new FileStorage(_storageFolder.FullName, storageName);
            _storages[storageName] = storages;

            return this;
        }

        public void Delete()
        {
            _storageFolder.Delete(true);
        }
    }
}
