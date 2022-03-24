using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.Core.Services.Abstractions
{
    public interface IUserFactoryService
    {
        /// <summary>
        /// Creates user object of existing user
        /// </summary>
        /// <param name="userService"></param>
        /// <returns></returns>
        UserBase CreateInstance(IUserService<UserBase> userService, int userId);

        /// <summary>
        /// Creates new user instance
        /// </summary>
        /// <returns></returns>
        Task<UserBase> CreateNewUserInstance(IUserService<UserBase> userService, CancellationToken token = default);

        /// <summary>
        /// Intitalize the service
        /// </summary>
        Task InitAsync(IGlobalServiceAggregator serviceAggregator, List<IUserService<UserBase>> userServices, CancellationToken token);
    }
}
