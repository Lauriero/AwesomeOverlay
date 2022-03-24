using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;
using AwesomeOverlay.Core.Services.Abstractions;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.Core.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private IGlobalServiceAggregator _serviceAggregator;
        private List<IUserAuthorizer<UserBase>> _userAuthorizers = new List<IUserAuthorizer<UserBase>>();

        /// <inheritdoc />
        public ReadOnlyCollection<IUserService<UserBase>> UserAuthorizers { get; private set; }

        /// <inheritdoc />
        public async Task AuthorizeUserFromStorage(UserBase user)
        {
            foreach (var userAuthorizer in _userAuthorizers) {
                if (userAuthorizer.GetType().GetInterface("IUserAuthorizer`1").GetGenericArguments()[0] == user.GetType()) {
                    await userAuthorizer.AuthorizeFromStorage(user).ConfigureAwait(false);
                    return;
                }
            }

            throw new Exception($"{user} is not the user type of any user services");
        }

        /// <inheritdoc />
        public async Task InitAsync(IGlobalServiceAggregator serviceAggregator, List<IUserService<UserBase>> userServices, CancellationToken token)
        {
            _serviceAggregator = serviceAggregator;

            UserAuthorizers = new ReadOnlyCollection<IUserService<UserBase>>(userServices);
            foreach (IUserService<UserBase> userService in UserAuthorizers) {
                IUserAuthorizer<UserBase> userAuthorizer = userService as IUserAuthorizer<UserBase>;
                _userAuthorizers.Add(userAuthorizer);

                userAuthorizer.UserAuthorized += async (user, isNew) => {
                    if (isNew)
                        await _serviceAggregator.UserStorage.AddUserAsync(userService, user);

                    if (user is INotificationProvider provider)
                        await provider.StartNotificationRecieving().ConfigureAwait(false);
                };
            }
        }
    }
}
