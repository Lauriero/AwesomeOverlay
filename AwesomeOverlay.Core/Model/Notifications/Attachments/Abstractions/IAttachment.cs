namespace AwesomeOverlay.Core.Model.Notifications.Attachments.Abstractions
{
    public interface IAttachment
    {
        /// <summary>
        /// Returns type of the attachment
        /// </summary>
        /// <returns></returns>
        AttachmentCategoryType GetAttachmentType();
    }
}
