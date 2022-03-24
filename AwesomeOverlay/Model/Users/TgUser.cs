using AwesomeOverlay.Core.FileStorageSystem;
using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Model.UserServices;

namespace AwesomeOverlay.Model.Users
{
    public class TgUser : UserBase
    {
        public TgUser(UserFileStoragesAggregator userFileStorages) : base(userFileStorages) {}
    }
}
