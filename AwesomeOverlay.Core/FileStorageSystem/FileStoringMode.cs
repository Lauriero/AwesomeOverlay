
namespace AwesomeOverlay.Core.FileStorageSystem
{
    public enum FileStoringMode
    {
        /// <summary>
        /// File will be saved until forever
        /// </summary>
        Eternal,

        /// <summary>
        /// File will be saved until next application launch
        /// </summary>
        Temp
    }
}
