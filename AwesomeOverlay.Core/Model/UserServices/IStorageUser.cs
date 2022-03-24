using AwesomeOverlay.Core.FileStorageSystem;

namespace AwesomeOverlay.Core.Model.UserServices
{
    public interface IStorageUser
    {
        /// <summary>
        /// Name of storage
        /// </summary>
        string StorageName { get; }
    }
}
