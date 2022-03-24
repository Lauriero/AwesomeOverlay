using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Services.Abstractions;
using AwesomeOverlay.Model.Notifications.Messages;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace AwesomeOverlay.Core.Services
{
    public class NotificationService : INotificationService
    {
        /// <summary>
        /// Shows how much notifications can be shown in list
        /// </summary>
        private const int MAX_NOTIFICATIONS_COUNT = 7;

        /// <summary>
        /// Period of deleting notification from list
        /// </summary>
        private const int REMOVING_TIMER_PERIOD = 5000;

        /// <summary>
        /// Collection with invisible notifications
        /// </summary>
        private Queue<MessageNotification> _invisibleNotifications = new Queue<MessageNotification>();

        /// <summary>
        /// Timer to removing notifications with REMOVING_TIMER_PERIOD interval
        /// </summary>
        private System.Timers.Timer _removingTimer;
        private void RemovingTimer_Tick(object state, ElapsedEventArgs e)
        {
            if (Notifications.Count != 0) {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    Notifications.RemoveAt(0);
                }), DispatcherPriority.ApplicationIdle).Task.GetAwaiter().OnCompleted(() => {
                    if (_invisibleNotifications.Count != 0) {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                            Notifications.Add(_invisibleNotifications.Dequeue());
                        }), DispatcherPriority.ApplicationIdle);
                    } else if (Notifications.Count == 0) {
                        _removingTimer.Stop();
                    }
                });
            }
        }

        /// <inheritdoc />
        public ObservableCollection<MessageNotification> Notifications { get; set; } = new ObservableCollection<MessageNotification>();

        /// <inheritdoc />
        public async Task InitAsync(IGlobalServiceAggregator serviceAggregator, CancellationToken token = default)
        {
            _removingTimer = new System.Timers.Timer(REMOVING_TIMER_PERIOD);
            _removingTimer.Stop();
            _removingTimer.Elapsed += RemovingTimer_Tick;

            Notifications.CollectionChanged += (s, e) => {
                if (e.Action == NotifyCollectionChangedAction.Add && !_removingTimer.Enabled) {
                    _removingTimer.Start();
                }
            };
        }

        /// <inheritdoc />
        public void AddNotificationProvider(INotificationProvider provider)
        {
            provider.NotificationRecieved += AddNotificationToList;
        }

        private void AddNotificationToList(MessageNotification notification)
        {
            if (Notifications.Count >= MAX_NOTIFICATIONS_COUNT) {
                _invisibleNotifications.Enqueue(notification);
            } else {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    Notifications.Add(notification);
                }), DispatcherPriority.ApplicationIdle);
            }
        }
    }
}
