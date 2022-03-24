
using AwesomeOverlay.Core.FileStorageSystem;
using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;
using AwesomeOverlay.Core.Services.Abstractions;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.Services
{
    public class UserFactoryService : IUserFactoryService
    {
        private int _nextUserId = 0;
        private List<IUserService<UserBase>> _userServices;
        private IGlobalServiceAggregator _serviceAggregator;

        /// <inheritdoc />
        public UserBase CreateInstance(IUserService<UserBase> userService, int userId)
        {
            Type userModelType = userService.GetType().GetInterface("IUserService`1").GetGenericArguments()[0];
            UserBase user = Activator.CreateInstance(userModelType, _serviceAggregator.LocalFileStorage.CreateStorageForUser(userId)) as UserBase;
            user.UserId = userId;

            return user;
        }

        /// <inheritdoc />
        public async Task<UserBase> CreateNewUserInstance(IUserService<UserBase> userService, CancellationToken token = default)
        {
            if (_nextUserId == 0) {
                _nextUserId = await _serviceAggregator.UserStorage.GetCurrentUserId(token) + 1;
            }

            UserFileStoragesAggregator userFileStorage = _serviceAggregator.LocalFileStorage.CreateStorageForUser(_nextUserId);

            Type userModelType = userService.GetType().GetInterface("IUserService`1").GetGenericArguments()[0];
            UserBase user = Activator.CreateInstance(userModelType, userFileStorage) as UserBase;
            user.UserId = _nextUserId;

            if (user is INotificationProvider notificationProvider)
                _serviceAggregator.NotificationsManager.AddNotificationProvider(notificationProvider);

            _nextUserId++;
            return user;
        }

        /// <inheritdoc />
        public async Task InitAsync(IGlobalServiceAggregator serviceAggregator, List<IUserService<UserBase>> userServices, CancellationToken token) {
            _serviceAggregator = serviceAggregator;
            _userServices = userServices;

            foreach (IUserService<UserBase> userService in userServices) {
                if (userService is IUserAuthorizer<UserBase> userAuthorizer) {
                    userAuthorizer.CreateNewUser = async () => {
                        return await CreateNewUserInstance(userService);
                    };
                }
            }
        }
    }
}
