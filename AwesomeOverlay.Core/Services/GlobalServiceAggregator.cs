using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;
using AwesomeOverlay.Core.Services.Abstractions;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.Core.Services
{
    public class GlobalServiceAggregator : IGlobalServiceAggregator
    {
        /// <inheritdoc />
        public IUserFactoryService UserFactory { get; }
        
        /// <inheritdoc />
        public IUserStorageService UserStorage { get; }

        /// <inheritdoc />
        public IAuthorizationService Authorization { get; }

        /// <inheritdoc />
        public ILocalFileStorageService LocalFileStorage { get; }

        /// <inheritdoc />
        public INotificationService NotificationsManager { get; }

        /// <inheritdoc />
        public IHotKeyService HotKeys { get; }

        public GlobalServiceAggregator(IUserFactoryService userFactory, IUserStorageService userStorage, IAuthorizationService authorization, ILocalFileStorageService fileStorage, INotificationService notificationService, IHotKeyService hotKeys)
        {
            UserStorage = userStorage;
            UserFactory = userFactory;
            Authorization = authorization;
            LocalFileStorage = fileStorage;
            NotificationsManager = notificationService;
            HotKeys = hotKeys;
        }

        /// <inheritdoc />
        public async Task InitAsync(List<IUserService<UserBase>> userServices, CancellationToken token = default)
        {
            await HotKeys.InitAsync(this, token).ConfigureAwait(false);
            await NotificationsManager.InitAsync(this, token).ConfigureAwait(false);
            await UserFactory.InitAsync(this, userServices, token).ConfigureAwait(false);
            await LocalFileStorage.InitAsync(this, token).ConfigureAwait(false);
            await Authorization.InitAsync(this, userServices.Where(us => us is IUserAuthorizer<UserBase>).ToList(), token).ConfigureAwait(false);
            await UserStorage.InitAsync(this, userServices, token).ConfigureAwait(false);
        }
    }
}
