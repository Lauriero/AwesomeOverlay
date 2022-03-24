using AwesomeOverlay.Core.Model.Notifications.Attachments.Abstractions;

using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace AwesomeOverlay.ViewModels.Notifications.Attachments.Abstractions
{
    public class AttachmentCategoryVM : BindableBase
    {
        private AttachmentCategory _category;
        public AttachmentCategoryVM(AttachmentCategory category)
        {
            _category = category;
            Attachments = new List<AttachmentBaseVM>(category.Attachments.Select(a => AttachmentsFactory.GetViewModel(a)));

            MenuItemClick = new DelegateCommand(() => {
                SelectedInMenu = !SelectedInMenu;
            });
        }

        public List<AttachmentBaseVM> Attachments { get; private set; }
        public string AttachmentsIconRK => _category.AttachmentsIconRK;

        public ICommand MenuItemClick { get; }

        private bool _selectedInMenu = false;
        public bool SelectedInMenu
        {
            get => _selectedInMenu;
            set {
                SetProperty(ref _selectedInMenu, value);
            }
        }
    }
}
