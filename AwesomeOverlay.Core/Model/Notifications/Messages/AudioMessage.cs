using System;
using System.Collections.Generic;

namespace AwesomeOverlay.Model.Notifications.Messages
{
    public class AudioMessage
    {
        public AudioMessage(Uri audioSource, List<int> waveForm, TimeSpan duration)
        {
            AudioSource = audioSource;
            AudioDuration = duration;
            WaveForm = waveForm;
        }

        public Uri AudioSource { get; }
        public TimeSpan AudioDuration { get; }
        public List<int> WaveForm { get; }
    }
}
