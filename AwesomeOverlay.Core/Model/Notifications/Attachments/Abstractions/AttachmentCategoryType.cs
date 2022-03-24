using System;
using System.Windows.Media;

namespace AwesomeOverlay.Core.Model.Notifications.Attachments.Abstractions
{
    public enum AttachmentType
    {
        Image,
        Video,
        Document
    }

    public class AttachmentCategoryType
    {
        public AttachmentType Type;
        public string IconRK;
    }

    public static class AttachmentCategoryTypes
    {
        public static AttachmentCategoryType Image = new AttachmentCategoryType() {
            Type = AttachmentType.Image,
            IconRK = "ImageAttachmentCategoryIcon"
        };

        public static AttachmentCategoryType Video = new AttachmentCategoryType() {
            Type = AttachmentType.Video,
            IconRK = "VideoAttachmentCategoryIcon"
        };

        public static AttachmentCategoryType Document = new AttachmentCategoryType() {
            Type = AttachmentType.Document,
            IconRK = "DownloadAttachmentCategoryIcon"
        };
    }
}
