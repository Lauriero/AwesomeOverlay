using AwesomeOverlay.Core.KeyboardHookerService;
using AwesomeOverlay.Core.Services.Abstractions;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AwesomeOverlay.Core.Services
{
    public class HotKeyService : IHotKeyService
    {
        /// <inheritdoc />
        public event Action MainWindowStateChanged;

        private IKeyboardHooker _keyboardHooker;
        public HotKeyService(IKeyboardHooker keyboardHooker)
        {
            _keyboardHooker = keyboardHooker;
        }

        /// <inheritdoc />q
        public async Task InitAsync(IGlobalServiceAggregator serviceAggregator, CancellationToken token = default)
        {
            _keyboardHooker.AddCombinationHandler(MAIN_WINDOW_STATE_CHANGE_HOOK_ID, new KeyCombination(Key.LeftAlt, Key.Q), () => { 
                MainWindowStateChanged?.Invoke();
            });

            _keyboardHooker.Init();
        }

        #region Hooks

        private const int MAIN_WINDOW_STATE_CHANGE_HOOK_ID = 0; 

        #endregion
    }
}
