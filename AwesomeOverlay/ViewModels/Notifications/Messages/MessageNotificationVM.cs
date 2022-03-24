using AwesomeOverlay.Model.Notifications.Messages;
using AwesomeOverlay.ViewModels.Notifications.Attachments.Abstractions;

using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AwesomeOverlay.ViewModels.Notifications.Messages
{
    public class MessageNotificationVM : BindableBase
    {
        private MessageNotification _notification;
        public MessageNotificationVM(MessageNotification notification)
        {
            _notification = notification;

            if (_notification.PathToChatAvatarImage != null) {
                IsSimpleMessage = false;
                IsChatMessage = true;
            } else {
                IsSimpleMessage = true;
                IsChatMessage = false;
            }

            if (_notification.VoiceMessage != null) {
                IsVoiceMessage = true;
                VoiceMessage = new AudioMessageVM(_notification.VoiceMessage);
            }

            if (_notification.AttachmentCategories.Count != 0)
                WithAttachments = true;

            AttachmentCategories = new List<AttachmentCategoryVM>(_notification.AttachmentCategories.Select(ac => new AttachmentCategoryVM(ac)));
            AttachmentCategories.ForEach(a => a.PropertyChanged += (o, e) => {
                if (e.PropertyName == "SelectedInMenu") {
                    if (a.SelectedInMenu) {
                        IsAttachmentCategoryOpen = true;
                        SelectedAttachmentCategory = (AttachmentCategoryVM)o;
                        AttachmentCategories.Where(at => at != (AttachmentCategoryVM)o).ToList().ForEach(at => at.SelectedInMenu = false);
                    } else {
                        if (SelectedAttachmentCategory == (AttachmentCategoryVM)o) {
                            IsAttachmentCategoryOpen = false;
                        }
                    }
                }
            });

            ExpandTextButtonClick = new DelegateCommand(() => {
                TextExpanded = !TextExpanded;
            });
        }

        public Drawing MessanagerIcon { get; }
        public bool IsSimpleMessage { get; private set; } = false;
        public Uri SenderAvatar => _notification.SenderAvatar;
        public string MessageText => _notification.MessageText;
        public string MessageTitle
        {
            get {
                if (IsChatMessage)
                    return _notification.ChatTitle + " | " + _notification.SenderName;
                else
                    return _notification.SenderName;
            }
        }

        #region Voice message
        public bool IsVoiceMessage { get; }
        public AudioMessageVM VoiceMessage { get; }
        #endregion

        #region Chat message
        public bool IsChatMessage { get; private set; } = false;
        public ImageBrush ChatAvatar
        {
            get {
                if (IsChatMessage) {
                    return new ImageBrush(new BitmapImage(_notification.PathToChatAvatarImage));
                } else {
                    return null;
                }
            }
        }
        #endregion

        #region Text expanding properties
        public ICommand ExpandTextButtonClick { get; }

        private bool _textExpanded = false;
        public bool TextExpanded
        {
            get => _textExpanded;
            set {
                SetProperty(ref _textExpanded, value);
            }
        }

        private bool _textCanExpand;
        public bool TextCanExpand
        {
            get => _textCanExpand;
            set {
                SetProperty(ref _textCanExpand, value);
            }
        }
        #endregion

        #region Attachments properties
        public List<AttachmentCategoryVM> AttachmentCategories { get; }
        public bool WithAttachments { get; private set; } = false;

        private bool _isAttachmentCategoryOpen = false;
        public bool IsAttachmentCategoryOpen
        {
            get => _isAttachmentCategoryOpen;
            set {
                SetProperty(ref _isAttachmentCategoryOpen, value);
            }
        }

        private AttachmentCategoryVM _selectedAttachmentCategory;
        public AttachmentCategoryVM SelectedAttachmentCategory
        {
            get => _selectedAttachmentCategory;
            set {
                SetProperty(ref _selectedAttachmentCategory, value);
            }
        }
        #endregion
    }
}
