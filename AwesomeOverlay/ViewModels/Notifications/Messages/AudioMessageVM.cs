using AwesomeOverlay.Model.Notifications.Messages;

using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace AwesomeOverlay.ViewModels.Notifications.Messages
{
    public class AudioMessageVM : BindableBase
    {
        AudioMessage _audioMessage;
        public AudioMessageVM(AudioMessage audioMessage)
        {
            _audioMessage = audioMessage;

            MouseEnterCommand = new DelegateCommand(() => IsBlackerLayerVisible = true);
            MouseLeaveCommand = new DelegateCommand(() => IsBlackerLayerVisible = false);

            PlayCommand = new DelegateCommand(() => {
                IsAudioPlaying = true;
            });
            PauseCommand = new DelegateCommand(() => {
                IsAudioPlaying = false;
            });
        }

        public Uri AudioSource => _audioMessage.AudioSource;
        public List<int> WaveForm => _audioMessage.WaveForm;
        public string AudioDuration => _audioMessage.AudioDuration.ToString(@"mm\:ss");

        public ICommand PlayCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand MouseEnterCommand { get; private set; }
        public ICommand MouseLeaveCommand { get; private set; }

        private bool _isAudioPlaying = false;
        public bool IsAudioPlaying
        {
            get => _isAudioPlaying;
            set {
                SetProperty(ref _isAudioPlaying, value);
            }
        }

        private bool _isBlackerLayerVisible = false;
        public bool IsBlackerLayerVisible
        {
            get => _isBlackerLayerVisible;
            set {
                SetProperty(ref _isBlackerLayerVisible, value);
            }
        }

        private TimeSpan _audioPosition;
        public TimeSpan AudioPosition
        {
            get => _audioPosition;
            set {
                SetProperty(ref _audioPosition, value);
            }
        }
    }
}
