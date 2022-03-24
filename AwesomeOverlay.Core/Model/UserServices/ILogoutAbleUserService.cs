using AwesomeOverlay.Core.Model.Users;

namespace AwesomeOverlay.Core.Model.UserServices
{
    /// <summary>
    /// Interface for user services whose users are able to logout without removing storing data
    /// </summary>
    public interface ILogoutAbleUserService<out UserModelType> : IUserService<UserModelType>
        where UserModelType : LogoutAbleUser
    {

    }
}