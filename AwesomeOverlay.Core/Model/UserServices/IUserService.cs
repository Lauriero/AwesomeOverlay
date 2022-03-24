using AwesomeOverlay.Core.Model.Users;

using System.Windows.Controls;
using System.Windows.Media;

namespace AwesomeOverlay.Core.Model.UserServices
{
    /// <summary>
    /// Interface for user services
    /// </summary>
    /// <typeparam name="UserModelType">Type of user model</typeparam>
    public interface IUserService<out UserModelType> : IStorageUser
        where UserModelType : UserBase
    {
        /// <summary>
        /// Main color of user service
        /// </summary>
        Color ServiceColor { get; }

        /// <summary>
        /// Resource key of service icon
        /// </summary>
        string ServiceIconResourceKey { get; }
    }
}
