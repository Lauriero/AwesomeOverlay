using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Model.Notifications.Messages;

using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.Core.Services.Abstractions
{
    public interface INotificationService
    {
        /// <summary>
        /// Presented notifications list
        /// </summary>
        ObservableCollection<MessageNotification> Notifications { get; set; }

        /// <summary>
        /// Initialize the service
        /// </summary>
        Task InitAsync(IGlobalServiceAggregator serviceAggregator, CancellationToken token = default);

        /// <summary>
        /// Subscribes on notification provider event
        /// </summary>
        void AddNotificationProvider(INotificationProvider provider);
    }
}