using AwesomeOverlay.Core.DownloadService;
using AwesomeOverlay.Core.Model.Notifications.Attachments.Abstractions;

using Prism.Mvvm;

using System;

namespace AwesomeOverlay.Core.Model.Notifications.Attachments
{
    public class VideoAttachment : BindableBase, IAttachment
    {
        public VideoAttachment(Uri videoSource, Uri previewImage, TimeSpan videoDuration)
        {
            VideoSource = videoSource;
            PreviewImage = previewImage;
            VideoDuration = videoDuration;
        }

        public VideoAttachment(Uri videoSource, Download previewImageDownload, TimeSpan videoDuration)
        {
            VideoSource = videoSource;
            VideoDuration = videoDuration;

            previewImageDownload.OnComplited += () => {
                PreviewImage = new Uri(previewImageDownload.FilePath, UriKind.Absolute);
            };
        }

        public Uri VideoSource { get; private set; }
        public Uri PreviewImage { get; private set; }
        public TimeSpan VideoDuration { get; private set; }


        private TimeSpan _videoPosition;
        public TimeSpan VideoPosition
        {
            get => _videoPosition;
            set {
                SetProperty(ref _videoPosition, value);
            }
        }

        public AttachmentCategoryType GetAttachmentType() => AttachmentCategoryTypes.Video;
    }
}
