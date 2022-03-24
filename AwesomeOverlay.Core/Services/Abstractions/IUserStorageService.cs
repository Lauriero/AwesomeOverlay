using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.Core.Services.Abstractions
{
    public interface IUserStorageService
    {
        /// <summary>
        /// Collection of users
        /// </summary>
        ObservableCollection<UserBase> Users { get; }

        /// <summary>
        /// Initialize the service
        /// </summary>
        Task InitAsync(IGlobalServiceAggregator serviceAggregator, List<IUserService<UserBase>> userServices, CancellationToken token);

        /// <summary>
        /// Returns user id for the last user (autoincrement field value of the users table)
        /// </summary>
        /// <returns></returns>
        Task<int> GetCurrentUserId(CancellationToken token = default);

        /// <summary>
        /// Adds user to storage
        /// </summary>
        Task AddUserAsync<UserType>(IUserService<UserType> storageUser, UserType user, CancellationToken token = default)
            where UserType : UserBase;
    }
}