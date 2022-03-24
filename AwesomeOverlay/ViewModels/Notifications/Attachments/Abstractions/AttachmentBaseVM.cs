using AwesomeOverlay.Core.Model.Notifications.Attachments;
using AwesomeOverlay.Core.Model.Notifications.Attachments.Abstractions;

using Prism.Mvvm;

using System;

namespace AwesomeOverlay.ViewModels.Notifications.Attachments.Abstractions
{
    public abstract class AttachmentBaseVM : BindableBase
    {
        public AttachmentBaseVM(IAttachment attachment) { }
        
        /// <summary>
        /// Fires when attachment selected in carousel
        /// </summary>
        public abstract void OnSelect();

        /// <summary>
        /// Fires when attachment unselected in carousel
        /// </summary>
        public abstract void OnUnselect();
    }

    public static class AttachmentsFactory
    {
        public static AttachmentBaseVM GetViewModel(IAttachment attachment)
        {
            if (attachment is ImageAttachment imageAttachment)
                return new ImageAttachmentVM(imageAttachment);
            else if (attachment is DocumentAttachment documentAttachment)
                return new DocumentAttachmentVM(documentAttachment);
            else if (attachment is VideoAttachment videoAttachment)
                return new VideoAttachmentVM(videoAttachment);
            else
                throw new Exception($"{attachment.GetType()} doesn't have corresponding view model");
        }
    }
}
