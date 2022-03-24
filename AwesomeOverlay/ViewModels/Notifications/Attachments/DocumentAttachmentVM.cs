using AwesomeOverlay.Core.Model.Notifications.Attachments;
using AwesomeOverlay.ViewModels.Notifications.Attachments.Abstractions;

using Prism.Commands;

using System.Windows.Input;

namespace AwesomeOverlay.ViewModels.Notifications.Attachments
{
    class DocumentAttachmentVM : AttachmentBaseVM
    {
        private DocumentAttachment _attachment;
        public DocumentAttachmentVM(DocumentAttachment attachment) : base(attachment)
        {
            _attachment = attachment;

            DownloadCommand = new DelegateCommand(async () => {
                IsDownloading = true;

                await _attachment.DownloadAsync();

                IsDownloaded = true;
                IsDownloading = false;
            });

            OpenCommand = new DelegateCommand(() => {
                _attachment.OpenFile();
            });
        }

        public ICommand DownloadCommand { get; }
        public ICommand OpenCommand { get; }

        /// <summary>
        /// String including size of file and suffix (e.g. 17 MB)
        /// </summary>
        public string DocumentSize => _attachment.DocumentSize;
        public string DocumentExtension => _attachment.DocumentExtension;

        private double _downloadingProgress;
        public double DownloadingProgress
        {
            get => _downloadingProgress;
            set {
                SetProperty(ref _downloadingProgress, value);
            }
        }

        private bool _isDownloading;
        public bool IsDownloading
        {
            get => _isDownloading;
            set {
                SetProperty(ref _isDownloading, value);
                RaisePropertyChanged("IsDownloadingBtnVisible");
            }
        }

        private bool _isDownloaded;
        public bool IsDownloaded
        {
            get => _isDownloaded;
            set {
                SetProperty(ref _isDownloaded, value);
                RaisePropertyChanged("IsDownloadingBtnVisible");
            }
        }

        public bool IsDownloadingBtnVisible => !IsDownloaded && !IsDownloading;

        public override void OnSelect() { }
        public override void OnUnselect() { }
    }
}
