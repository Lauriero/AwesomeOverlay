using AwesomeOverlay.Core.FileStorageSystem;
using AwesomeOverlay.Core.Model.UserServices;
using AwesomeOverlay.Core.Utilities.ColorUtilities;
using AwesomeOverlay.Model.Users;

using System.Windows.Media;

namespace AwesomeOverlay.Model.UserServices
{
    public class TgUserService : IUserService<TgUser>
    {
        /// <inheritdoc />
        public string StorageName { get; } = "telegram";
        
        /// <inheritdoc />
        public Color ServiceColor { get; } = "#40A7E3".TurnHexIntoColor();

        // <inheritdoc />
        public string ServiceIconResourceKey { get; } = "TgIcon";

    }
}
