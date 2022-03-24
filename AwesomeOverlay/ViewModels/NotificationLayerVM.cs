using AwesomeOverlay.Core.Services.Abstractions;
using AwesomeOverlay.Core.Utilities.CollectionUtilities;
using AwesomeOverlay.ViewModels.Notifications.Messages;

using System.Collections.ObjectModel;

namespace AwesomeOverlay.ViewModels
{
    public class NotificationLayerVM
    {
        private INotificationService _notificationService;
        public NotificationLayerVM(INotificationService notificationService)
        {
            _notificationService = notificationService;

            Notifications = new ObservableCollection<MessageNotificationVM>();
            Notifications.SynchroniseWith(_notificationService.Notifications, n => new MessageNotificationVM(n));
        }

        public ObservableCollection<MessageNotificationVM> Notifications { get; private set; }
    }
}
