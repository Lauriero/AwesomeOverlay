using System;

namespace AwesomeOverlay.Core.KeyboardHookerService
{
    public interface IKeyboardHooker
    {
        /// <summary>
        /// Initialize the service
        /// </summary>
        void Init();

        /// <summary>
        /// Set new combination handler
        /// </summary>
        /// <param name="handlerId">Unique handler id</param>
        /// <param name="combination">Key combination to handle</param>
        /// <param name="eventHandler">Function to handle combination</param>
        void AddCombinationHandler(int handlerId, KeyCombination combination, KeyCombinationPressedHandler eventHandler);

        /// <summary>
        /// Updates the key combination belonging to the handler
        /// </summary>
        /// <param name="handlerId">Unique handler id</param>
        /// <param name="combination">New key combination to handle</param>
        void ChangeCombinationHandler(int handlerId, KeyCombination combination);
    }
}
