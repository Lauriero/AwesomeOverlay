using AwesomeOverlay.Core.FileStorageSystem;
using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.Core.Services.Abstractions
{
    public interface ILocalFileStorageService
    {
        /// <summary>
        /// Creates user file storages for added user
        /// </summary>
        /// <returns></returns>
        UserFileStoragesAggregator CreateStorageForUser(int userId);

        /// <summary>
        /// Deletes user file storage
        /// </summary>
        public void DeleteUserStorage(int userId);

        /// <summary>
        /// Intitalize the service
        /// </summary>
        Task InitAsync(IGlobalServiceAggregator serviceAggregator, CancellationToken token);
    }
}
