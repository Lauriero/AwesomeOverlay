using AwesomeOverlay.Model.Notifications.Messages;

using System;
using System.Security.Policy;
using System.Threading.Tasks;

namespace AwesomeOverlay.Core.Model.Users
{
    public interface INotificationProvider
    {
        /// <summary>
        /// Raise when user recieve notification
        /// </summary>
        event Action<MessageNotification> NotificationRecieved;

        /// <summary>
        /// Starts processes needed to recieve notifications
        /// </summary>
        Task StartNotificationRecieving();
    }
}