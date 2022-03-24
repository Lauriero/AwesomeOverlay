using AwesomeOverlay.Core.DownloadService;

using System;
using System.IO;
using System.Net;

namespace AwesomeOverlay.Core.FileStorageSystem
{
    public class FileStorage
    {
        private DirectoryInfo _storageDirectory;
        private DirectoryInfo _eternalStorageDirectory;
        private DirectoryInfo _tempStorageDirectory;

        public FileStorage(string aggregatorStoragePath, string storageName)
        {
            string storagePath = Path.Combine(aggregatorStoragePath, storageName);
            string eternalStoragePath = Path.Combine(storagePath, "eternal");
            string tempStoragePath = Path.Combine(storagePath, "temp");

            _storageDirectory = new DirectoryInfo(storagePath);
            _eternalStorageDirectory = new DirectoryInfo(eternalStoragePath);
            _tempStorageDirectory = new DirectoryInfo(tempStoragePath);

            if (!_storageDirectory.Exists) {
                _storageDirectory.Create();
                _eternalStorageDirectory.Create();
                _tempStorageDirectory.Create();

                return;
            }

            if (!_eternalStorageDirectory.Exists)
                _eternalStorageDirectory.Create();

            if (!_tempStorageDirectory.Exists) {
                _tempStorageDirectory.Create();
            } else {
                foreach (string filePath in Directory.GetFiles(_tempStorageDirectory.FullName)) {
                    File.Delete(filePath);
                }
            }
        }

        public Download CreateDownload(Uri downloadUri, string fileName, FileStoringMode storingMode)
        {
            string downloadPath;
            Download download = null;

            switch (storingMode) {
                case FileStoringMode.Eternal:
                    downloadPath = Path.Combine(_eternalStorageDirectory.FullName, fileName);
                    
                    download = new Download(downloadPath, downloadUri);
                    download.State = File.Exists(downloadPath) ? DownloadingState.Downloaded : DownloadingState.NotInСache;
                    break;
                case FileStoringMode.Temp:
                    downloadPath = Path.Combine(_tempStorageDirectory.FullName, fileName);

                    download = new Download(downloadPath, downloadUri);
                    download.State = File.Exists(downloadPath) ? DownloadingState.Downloaded : DownloadingState.NotInСache;
                    break;
            }

            return download;
        }

        public Uri GetFile(string fileName)
        {
            foreach (FileInfo file in _eternalStorageDirectory.GetFiles()) {
                if (file.Name == fileName) {
                    return new Uri(file.FullName, UriKind.Absolute);
                }
            }

            foreach (FileInfo file in _tempStorageDirectory.GetFiles()) {
                if (file.Name == fileName) {
                    return new Uri(file.FullName, UriKind.Absolute);
                }
            }

            return null;
        }

        public bool ContainsFile(string fileName) =>
            File.Exists(Path.Combine(_storageDirectory.FullName, fileName)) 
            || File.Exists(Path.Combine(_eternalStorageDirectory.FullName, fileName));
    }
}
