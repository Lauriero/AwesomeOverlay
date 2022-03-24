using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;
using AwesomeOverlay.Core.Services.Abstractions;
using AwesomeOverlay.UserDataStorage.Abstraction;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.Core.Services
{
    public class UserStorageService : IUserStorageService
    {
        private IUserDataStorageManager _storageManager;
        private IGlobalServiceAggregator _serviceAggregator;
        private List<IUserService<UserBase>> _userServices;

        /// <inheritdoc />
        public ObservableCollection<UserBase> Users { get; private set; }

        public UserStorageService(IUserDataStorageManager storageManager)
        {
            _storageManager = storageManager;
        }
        
        /// <inheritdoc />
        public async Task InitAsync(IGlobalServiceAggregator serviceAggregator, List<IUserService<UserBase>> userServices, CancellationToken token = default)
        {
            _userServices = userServices;
            _serviceAggregator = serviceAggregator;

            await _storageManager.InitAsync(userServices, token);

            List<UserBase> users = await _storageManager.GetAllUsersAsync(token);
            foreach (UserBase user in users) {
                SubscribeUserEvents(user);

                if (user is INotificationProvider notificationProvider)
                    _serviceAggregator.NotificationsManager.AddNotificationProvider(notificationProvider);

                if (user.Authorized)
                    await _serviceAggregator.Authorization.AuthorizeUserFromStorage(user).ConfigureAwait(false);
            }

            Users = new ObservableCollection<UserBase>(users);
        }

        /// <inheritdoc />
        public async Task<int> GetCurrentUserId(CancellationToken token = default)
        {
            return await _storageManager.GetCurrentUserId(token);
        }

        /// <inheritdoc />
        public async Task AddUserAsync<UserType>(IUserService<UserType> storageUser, UserType user, CancellationToken token)
            where UserType : UserBase
        {
            SubscribeUserEvents(user);

            Users.Add(user);
            await _storageManager.AddUserAsync(storageUser, user, token);
        }

        public void SubscribeUserEvents(UserBase user)
        {
            user.PropertyChanged += (o, e) => {
                Task.Run(async () => {
                    await _storageManager.UpdateUserData(GetUserServiceViaUser(user), user, e.PropertyName);
                });
            };


            user.UserDeleted += UserDeletedAsync;
            if (user is LogoutAbleUser u)
                u.UserUnauthorized += UserUnauthorizedAsync;
        }

        private async Task UserDeletedAsync(UserBase user) 
        {
            Users.Remove(user);

            _serviceAggregator.LocalFileStorage.DeleteUserStorage(user.UserId);
            await _storageManager.DeleteUserAsync(user);
        }

        private async Task UserUnauthorizedAsync(LogoutAbleUser user)
        {
            Console.WriteLine("unauthorized");
            await _storageManager.UnauthorizeUserAsync(user);
        }

        private IUserService<UserBase> GetUserServiceViaUser(UserBase user)
        {
            foreach (IUserService<UserBase> userService in _userServices) {
                Type userType = userService.GetType().GetInterface("IUserService`1").GetGenericArguments()[0];
                if (userType == user.GetType()) {
                    return userService;
                }
            }

            throw new Exception($"User service for {user} is not found") ;
        }
    }
}