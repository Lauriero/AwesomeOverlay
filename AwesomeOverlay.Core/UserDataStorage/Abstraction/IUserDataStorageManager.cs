using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.UserDataStorage.Abstraction
{
    public interface IUserDataStorageManager
    {
        /// <summary>
        /// Initialize database
        /// </summary>
        /// <param name="storageUsers">List of user services</param>
        Task InitAsync(List<IUserService<UserBase>> userServices, CancellationToken token = default);

        /// <summary>
        /// Returns user id for the last user (autoincrement field value of the users table)
        /// </summary>
        /// <returns></returns>
        Task<int> GetCurrentUserId(CancellationToken token = default);

        /// <summary>
        /// Adds new user to a database
        /// </summary>
        /// <typeparam name="UserType">Type of user model</typeparam>
        /// <param name="userService">User service</param>
        /// <param name="user">User model</param>
        Task AddUserAsync<UserType>(IUserService<UserType> userService, UserType user, CancellationToken token = default)
            where UserType : UserBase;

        /// <summary>
        /// Updates certain field of user object
        /// </summary>
        /// <typeparam name="UserType">Type of user</typeparam>
        /// <param name="userService">User service for identifying the storage</param>
        /// <param name="user">User which property has changed</param>
        /// <param name="propertyName">Name of the user property that has been changed</param>
        Task UpdateUserData<UserType>(IUserService<UserType> userService, UserType user, string propertyName, CancellationToken token = default)
            where UserType : UserBase;

        /// <summary>
        /// Gets list of users stored in the database
        /// </summary>
        Task<List<UserBase>> GetAllUsersAsync(CancellationToken token = default);

        /// <summary>
        /// Deletes user from storage
        /// </summary>
        /// <param name="user">Removing user model</param>
        Task DeleteUserAsync(UserBase user, CancellationToken token = default);

        /// <summary>
        /// Set authorized property to false and update it in the database, also nullifies some columns
        /// </summary>
        /// <param name="user">Unauthorized user</param>
        Task UnauthorizeUserAsync<UserType>(UserType user, CancellationToken token = default)
            where UserType : LogoutAbleUser;
    }
}
