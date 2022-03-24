using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.Core.DownloadService
{
    public class Download
    {
        public DownloadingState State = DownloadingState.NotInСache;
        public string FilePath;
        public Uri Source;

        public event Action OnComplited;
        public event Action<double> OnProgressChanged;

        private WebClient _downloadClient;

        public Download(string filePath, Uri downloadSource)
        {
            FilePath = filePath;
            Source = downloadSource;
        }

        /// <summary>
        /// Downloads file and returns uri to the file
        /// </summary>
        public async Task<Uri> DownloadAsync(CancellationToken token = default)
        {
            _downloadClient = new WebClient();
            State = DownloadingState.Downloading;

            await Task.Run(() => {
                _downloadClient.DownloadFile(Source, FilePath);
            }, token);

            State = DownloadingState.Downloaded;
            _downloadClient.Dispose();

            return new Uri(FilePath, UriKind.Absolute);
        }
    }
}
