using AwesomeOverlay.Core.Model.Notifications.Attachments;
using AwesomeOverlay.Core.Model.Notifications.Attachments.Abstractions;
using AwesomeOverlay.ViewModels.Notifications.Attachments.Abstractions;

using System;

namespace AwesomeOverlay.ViewModels.Notifications.Attachments
{
    class ImageAttachmentVM : AttachmentBaseVM
    {
        public ImageAttachment _attachment;
        public ImageAttachmentVM(ImageAttachment attachment) : base(attachment)
        {
            _attachment = attachment;
        }

        public Uri ImageSource => _attachment.ImageSource;

        public override void OnSelect() { }
        public override void OnUnselect() { }
    }
}
