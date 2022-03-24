using AwesomeOverlay.Core.FileStorageSystem;
using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;
using AwesomeOverlay.Core.Services.Abstractions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AwesomeOverlay.Core.Services
{
    public class LocalFileStorageService : ILocalFileStorageService
    {
        public const string FILE_STORAGE_FOLDER_NAME = "file-storage";

        private IGlobalServiceAggregator _serviceAggregator;

        private DirectoryInfo _fileStorageFolder;
        private Dictionary<int, UserFileStoragesAggregator> _userStorages = new Dictionary<int, UserFileStoragesAggregator>();

        /// <inheritdoc />
        public UserFileStoragesAggregator CreateStorageForUser(int userId)
        {
            UserFileStoragesAggregator userFileStorages = new UserFileStoragesAggregator(_fileStorageFolder.FullName, userId);
            _userStorages.Add(userId, userFileStorages);

            return userFileStorages;
        }

        /// <summary>
        /// Deletes user file storage
        /// </summary>
        public void DeleteUserStorage(int userId)
        {
            _userStorages[userId].Delete();
        }

        /// <inheritdoc />
        public async Task InitAsync(IGlobalServiceAggregator serviceAggregator, CancellationToken token)
        {
            _serviceAggregator = serviceAggregator;

            _fileStorageFolder = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, FILE_STORAGE_FOLDER_NAME));
            if (!_fileStorageFolder.Exists)
                _fileStorageFolder.Create();
        }
    }
}
