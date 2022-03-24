using AwesomeOverlay.Core.Model.Notifications.Attachments.Abstractions;

using System;

namespace AwesomeOverlay.Core.Model.Notifications.Attachments
{
    public class ImageAttachment : IAttachment
    {
        public ImageAttachment(Uri pathToImage)
        {
            ImageSource = pathToImage;
        }

        public Uri ImageSource { get; private set; }
        public AttachmentCategoryType GetAttachmentType() => AttachmentCategoryTypes.Image;
    }
}
