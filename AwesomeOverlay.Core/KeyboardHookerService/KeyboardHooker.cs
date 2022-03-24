using AwesomeOverlay.Core.Utilities;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace AwesomeOverlay.Core.KeyboardHookerService
{
    public delegate void KeyCombinationPressedHandler();

    public class KeyboardHooker : IKeyboardHooker, IDisposable
    {
        private Dictionary<int, KeyValuePair<KeyCombination, KeyCombinationPressedHandler>> _subscribers = new Dictionary<int, KeyValuePair<KeyCombination, KeyCombinationPressedHandler>>();

        private IntPtr _hookPtr;
        private InteropHelper.HookProc _hookProc;

        /// <inheritdoc />
        public void Init()
        {
            _hookProc = KeyboardListener;

            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule) {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    _hookPtr = InteropHelper.SetWindowsHookEx(InteropHelper.HookType.WH_KEYBOARD_LL, _hookProc, InteropHelper.GetModuleHandle(curModule.ModuleName), 0);
                }), DispatcherPriority.ApplicationIdle);
            }
        }

        /// <inheritdoc />
        public void AddCombinationHandler(int handlerId, KeyCombination combination, KeyCombinationPressedHandler eventHandler)
        {
            if (_subscribers.ContainsKey(handlerId))
                throw new Exception("This handler id is already registered");

            if (combination == null || !combination.IsValid())
                throw new Exception("Combination was not provided or contains too few keys to handle (min 2)");

            if (eventHandler == null)
                throw new Exception("Event handler cannot be null");

            _subscribers.Add(handlerId, new KeyValuePair<KeyCombination, KeyCombinationPressedHandler>(combination, eventHandler));
        }

        /// <inheritdoc />
        public void ChangeCombinationHandler(int handlerId, KeyCombination combination)
        {
            if (!_subscribers.ContainsKey(handlerId))
                throw new Exception("This handler id is not registered");

            if (combination == null || !combination.IsValid())
                throw new Exception("Combination was not provided or contains too few keys to handle (min 2)");

            _subscribers[handlerId]= new KeyValuePair<KeyCombination, KeyCombinationPressedHandler>(combination, _subscribers[handlerId].Value);
        }

        private List<Key> _keyBuffer = new List<Key>();
        private IntPtr KeyboardListener(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0) {
                return InteropHelper.CallNextHookEx(_hookPtr, code, wParam, lParam);
            } else {
                InteropHelper.KeyboardHookStruct keyboardHookStruct = Marshal.PtrToStructure<InteropHelper.KeyboardHookStruct>(lParam);

                switch ((InteropHelper.WM)wParam) {
                    case InteropHelper.WM.KEYUP:
                        _keyBuffer.Clear();
                        break;

                    case InteropHelper.WM.KEYDOWN:
                        OnKeyDown(KeyInterop.KeyFromVirtualKey(keyboardHookStruct.VirtualKeyCode));
                        break;

                    case InteropHelper.WM.SYSKEYDOWN:
                        OnKeyDown(KeyInterop.KeyFromVirtualKey(keyboardHookStruct.VirtualKeyCode));
                        break;
                }

                return InteropHelper.CallNextHookEx(_hookPtr, code, wParam, lParam);
            }
        }

        public void OnKeyDown(Key key)
        {
            _keyBuffer.Add(key);

            if (_keyBuffer.Count >= 2) {
                foreach (var pair in _subscribers.Values) {
                    if (pair.Key.Equals(new KeyCombination(_keyBuffer))) {
                        pair.Value.Invoke();
                    }
                }
            }
        }

        public void Dispose()
        {
            InteropHelper.UnhookWindowsHookEx(_hookPtr);
        }
    }
}
