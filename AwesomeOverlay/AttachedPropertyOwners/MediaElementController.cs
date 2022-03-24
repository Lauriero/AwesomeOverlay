using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AwesomeOverlay.AttachedPropertyOwners
{
    class MediaElementController
    {
        public static bool GetVideoPlaying(DependencyObject d) =>
            (bool)d.GetValue(VideoPlayingProperty);

        public static void SetVideoPlaying(DependencyObject d, bool value) =>
            d.SetValue(VideoPlayingProperty, value);

        /// <summary>
        /// Control video playing for media element
        /// </summary>
        public static readonly DependencyProperty VideoPlayingProperty =
            DependencyProperty.RegisterAttached(
                "VideoPlaying",
                typeof(bool),
                typeof(MediaElementController),
                new PropertyMetadata(
                    false,
                    OnVideoPlayingUpdated
                ));

        public static bool GetPlayingPosition(DependencyObject d) =>
            (bool)d.GetValue(PlayingPositionProperty);

        public static void SetPlayingPosition(DependencyObject d, TimeSpan value) =>
            d.SetValue(PlayingPositionProperty, value);

        /// <summary>
        /// If video is playing, each second current video playing position will set to this property
        /// </summary>
        public static readonly DependencyProperty PlayingPositionProperty =
            DependencyProperty.RegisterAttached(
                "PlayingPosition",
                typeof(TimeSpan),
                typeof(MediaElementController),
                new PropertyMetadata(
                    default(TimeSpan)
                ));

        private static Dictionary<WeakReference, Timer> _mediaElementTimers = new Dictionary<WeakReference, Timer>();


        private static void OnVideoPlayingUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MediaElement mediaElement))
                return;

            Timer mediaElementPositionTimer;

            KeyValuePair<WeakReference, Timer> isAlreadyLoaded = _mediaElementTimers.FirstOrDefault(f => f.Key.Target == d);
            if (isAlreadyLoaded.Key == null) {
                mediaElementPositionTimer = new Timer(1000);
                mediaElementPositionTimer.Enabled = false;
                mediaElementPositionTimer.Elapsed += (o, args) => {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        SetPlayingPosition(d, mediaElement.Position);
                    }), DispatcherPriority.ApplicationIdle);
                };

                _mediaElementTimers.Add(new WeakReference(d), mediaElementPositionTimer);
            } else {
                mediaElementPositionTimer = isAlreadyLoaded.Value;
            }


            if ((bool)e.NewValue) {
                mediaElement.Play();
                mediaElementPositionTimer.Start();
            } else {
                mediaElement.Pause();
                mediaElementPositionTimer.Stop();
            }
        }
    }
}
