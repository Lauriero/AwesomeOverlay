using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Services.Abstractions;

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AwesomeOverlay.Core.Model.UserServices
{
    /// <summary>
    /// Interface for user services which can authorize user and provides the logic of authorization process
    /// </summary>
    public interface IUserAuthorizer<out UserType> : INotifyPropertyChanged
        where UserType : UserBase
    {
        /// <summary>
        /// Raised when user become authorized, first parameter is authorized user, second parameter is whether it's a new user or not
        /// </summary>
        event Func<UserType, bool, Task> UserAuthorized;

        /// <summary>
        /// Contains logic of authorization and run on each step and returns error message if authorization was failed or null value otherwise
        /// </summary>
        Task<string> Authorize();

        /// <summary>
        /// Authorize user using data saved in storage
        /// </summary>
        Task AuthorizeFromStorage(UserBase user);

        /// <summary>
        /// Specify whether it's a latest step of authorization
        /// </summary>
        bool IsTheFinalAuthorizationStepActive { get; }

        /// <summary>
        /// Control with authorization fields
        /// </summary>
        UserControl AuthorizationView { get; }

        /// <summary>
        /// Method for creating new user
        /// </summary>
        Func<Task<UserBase>> CreateNewUser { set; }
    }
}