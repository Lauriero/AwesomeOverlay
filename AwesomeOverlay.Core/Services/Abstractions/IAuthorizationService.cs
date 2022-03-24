using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.Core.Services.Abstractions
{
    public interface IAuthorizationService
    {
        /// <summary>
        /// Collection of user services which implement IUserAuthorizer interface
        /// </summary>
        ReadOnlyCollection<IUserService<UserBase>> UserAuthorizers { get; }

        /// <summary>
        /// Authorizes user using data saved in storage
        /// </summary>
        /// <returns></returns>
        Task AuthorizeUserFromStorage(UserBase user);

        /// <summary>
        /// Intitalize the service
        /// </summary>
        Task InitAsync(IGlobalServiceAggregator serviceAggregator, List<IUserService<UserBase>> userServices, CancellationToken token);
    }
}