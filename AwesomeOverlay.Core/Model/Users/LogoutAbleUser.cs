using AwesomeOverlay.Core.FileStorageSystem;

using System.Threading.Tasks;

namespace AwesomeOverlay.Core.Model.Users
{
    /// <summary>
    /// Base class for users which are able to logout without removing storing data
    /// </summary>
    public class LogoutAbleUser : UserBase
    {
        public event UserUnauthorizedEventHandler UserUnauthorized;

        public LogoutAbleUser(UserFileStoragesAggregator userFileStorages) : base(userFileStorages) { }

        /// <summary>
        /// Invokes when user should be unauthorized
        /// </summary>
        public async Task UnauthorizeAsync()
        {
            await UserUnauthorized?.Invoke(this);
        }
    }

    /// <summary>
    /// Delegate for user unauthorized event
    /// </summary>
    public delegate Task UserUnauthorizedEventHandler(LogoutAbleUser user);
}