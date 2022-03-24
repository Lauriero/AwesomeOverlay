
using Prism.Mvvm;

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace AwesomeOverlay.Core.Model.Notifications.Attachments.Abstractions
{
    public class AttachmentCategory : BindableBase
    {
        private AttachmentCategory(IEnumerable<IAttachment> attachments, AttachmentCategoryType categoryType)
        {
            Attachments = new List<IAttachment>(attachments);
            AttachmentsIconRK = categoryType.IconRK;
            AttachmentCategoryType = categoryType;
        }

        /// <summary>
        /// Parse list of attachments and sorted them by categories
        /// </summary>
        /// <returns></returns>
        public static List<AttachmentCategory> Parse(IEnumerable<IAttachment> attachments)
        {
            List<AttachmentCategory> categories = new List<AttachmentCategory>();
            List<AttachmentCategoryType> categoriesOrder = new List<AttachmentCategoryType>() {
                AttachmentCategoryTypes.Image, AttachmentCategoryTypes.Video, AttachmentCategoryTypes.Document
            };

            foreach (AttachmentCategoryType categoryType in categoriesOrder) {
                IEnumerable<IAttachment> sortedAttachment = attachments.Where(a => a.GetAttachmentType() == categoryType);
                if (sortedAttachment.Any()) {
                    categories.Add(new AttachmentCategory(sortedAttachment, categoryType));
                }
            }

            return categories;
        }

        public List<IAttachment> Attachments { get; private set; }
        public string AttachmentsIconRK { get; private set; }
        public AttachmentCategoryType AttachmentCategoryType { get; private set; }
    }
}
