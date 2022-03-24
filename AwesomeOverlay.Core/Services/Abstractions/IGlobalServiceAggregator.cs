using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.Core.Services.Abstractions
{
    public interface IGlobalServiceAggregator
    {
        /// <summary>
        /// Service managing creating user operations 
        /// </summary>
        IUserFactoryService UserFactory { get; }
        
        /// <summary>
        /// Service managing user data storage
        /// </summary>
        IUserStorageService UserStorage { get; }

        /// <summary>
        /// Service managing user authorization logic
        /// </summary>
        IAuthorizationService Authorization { get; }

        /// <summary>
        /// Service managing storage of users' files
        /// </summary>
        ILocalFileStorageService LocalFileStorage { get; }

        /// <summary>
        /// Service managing notifications
        /// </summary>
        INotificationService NotificationsManager { get; }

        /// <summary>
        /// Service handling application hot keys
        /// </summary>
        IHotKeyService HotKeys { get; }

        /// <summary>
        /// Initializate aggregator
        /// </summary>
        Task InitAsync(List<IUserService<UserBase>> userServices, CancellationToken token = default);
    }
}