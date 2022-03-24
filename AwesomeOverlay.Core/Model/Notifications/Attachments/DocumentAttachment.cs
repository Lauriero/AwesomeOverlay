using AwesomeOverlay.Core.DownloadService;
using AwesomeOverlay.Core.Model.Notifications.Attachments.Abstractions;

using ByteSizeLib;

using Prism.Mvvm;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AwesomeOverlay.Core.Model.Notifications.Attachments
{
    public class DocumentAttachment : BindableBase, IAttachment
    {
        private Download _download;
        public DocumentAttachment(Download download, string extension, ByteSize size)
        {
            _download = download;
            DocumentExtension = extension;
            DocumentSize = Math.Round(size.LargestWholeNumberDecimalValue, 1).ToString() + size.LargestWholeNumberDecimalSymbol;
        }

        /// <summary>
        /// String including size of file and suffix (e.g. 17 MB)
        /// </summary>
        public string DocumentSize { get; }
        public string DocumentExtension { get; }

        public async Task DownloadAsync()
        {
            await _download.DownloadAsync();
        }

        public void OpenFile()
        {
            Process.Start(_download.FilePath);
        }

        public AttachmentCategoryType GetAttachmentType() => AttachmentCategoryTypes.Document;
    }
}
