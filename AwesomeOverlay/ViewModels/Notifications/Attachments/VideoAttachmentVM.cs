using AwesomeOverlay.Core.Model.Notifications.Attachments;
using AwesomeOverlay.ViewModels.Notifications.Attachments.Abstractions;

using Prism.Commands;

using System;
using System.Windows.Input;

namespace AwesomeOverlay.ViewModels.Notifications.Attachments
{
    class VideoAttachmentVM : AttachmentBaseVM
    {
        private VideoAttachment _attachment;
        public VideoAttachmentVM(VideoAttachment attachment) : base(attachment)
        {
            _attachment = attachment;

            MouseEnterCommand = new DelegateCommand(() => IsBlackerLayerVisible = true);
            MouseLeaveCommand = new DelegateCommand(() => IsBlackerLayerVisible = false);

            PlayCommand = new DelegateCommand(() => {
                IsVideoPlaying = true;
            });
            PauseCommand = new DelegateCommand(() => {
                IsVideoPlaying = false;
            });
        }

        public Uri VideoSource => _attachment.VideoSource;
        public Uri PreviewImage => _attachment.PreviewImage;
        public TimeSpan VideoDuration => _attachment.VideoDuration;

        private TimeSpan _videoPosition;
        public TimeSpan VideoPosition
        {
            get => _videoPosition;
            set {
                SetProperty(ref _videoPosition, value);
            }
        }

        public ICommand PlayCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand MouseEnterCommand { get; private set; }
        public ICommand MouseLeaveCommand { get; private set; }

        private bool _isVideoPlaying = false;
        public bool IsVideoPlaying
        {
            get => _isVideoPlaying;
            set {
                IsPreviewVisible = false;
                SetProperty(ref _isVideoPlaying, value);
            }
        }

        private bool _isBlackerLayerVisible = false;
        public bool IsBlackerLayerVisible
        {
            get => _isBlackerLayerVisible;
            set {
                SetProperty(ref _isBlackerLayerVisible, value);
            }
        }

        private bool _isPreviewVisible = true;
        public bool IsPreviewVisible
        {
            get => _isPreviewVisible;
            set {
                SetProperty(ref _isPreviewVisible, value);
            }
        }

        public override void OnSelect() { }
        public override void OnUnselect() { }
    }
}
