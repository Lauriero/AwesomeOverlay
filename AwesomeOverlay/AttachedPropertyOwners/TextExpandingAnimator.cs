
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AwesomeOverlay.AttachedPropertyOwners
{
    class TextExpandingAnimator : BaseAnimationTriggerAttachedProperty<TextExpandingAnimator>
    {
        public static bool GetTextCanExpand(DependencyObject d) =>
            (bool)d.GetValue(TextCanExpandProperty);

        public static void SetTextCanExpand(DependencyObject d, bool value) =>
            d.SetValue(TextCanExpandProperty, value);

        /// <summary>
        /// If text is too big to present in one string - sets to true
        /// </summary>
        public static readonly DependencyProperty TextCanExpandProperty =
            DependencyProperty.RegisterAttached(
                "TextCanExpand",
                typeof(bool),
                typeof(TextExpandingAnimator),
                new PropertyMetadata(
                    false
                ));

        private double _minHeight;

        private static Dictionary<WeakReference, bool> _propertiesAlreadyLoaded = new Dictionary<WeakReference, bool>();
        private static Dictionary<WeakReference, double> _expandedHeights = new Dictionary<WeakReference, double>();

        public override Storyboard GetAnimationStoryboard(DependencyObject d, bool firstLoad)
        {
            if (!(d is TextBlock textBlock))
                return null;

            double expandedHeight = _expandedHeights.FirstOrDefault(f => f.Key.Target == d).Value;
            if (expandedHeight == 0)
                return null;

            DoubleAnimation animation = new DoubleAnimation() {
                To = expandedHeight,
                Duration = firstLoad ? TimeSpan.Zero : TimeSpan.FromSeconds(0.3),
                EasingFunction = new PowerEase() { Power = 2, EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath("Height"));

            Storyboard animationStoryboard = new Storyboard();
            animationStoryboard.Children.Add(animation);

            return animationStoryboard;
        }

        public override Storyboard GetReverseAnimationStoryboard(DependencyObject d, bool firstLoad)
        {
            if (!(d is TextBlock textBlock))
                return null;

            KeyValuePair<WeakReference, bool> isAlreadyLoaded = _propertiesAlreadyLoaded.FirstOrDefault(f => f.Key.Target == d);
            if (isAlreadyLoaded.Key == null) {
                WeakReference newSenderReference = new WeakReference(d);
                _propertiesAlreadyLoaded[newSenderReference] = false;

                _minHeight = (new FormattedText("Й", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, textBlock.FontFamily.GetTypefaces().ElementAt(0), textBlock.FontSize, textBlock.Foreground).Height - 1.2) * 2;

                RoutedEventHandler elementLoaded = null;
                elementLoaded = (s, le) => {
                    double expandedHeight = textBlock.ActualHeight;
                    textBlock.Height = _minHeight;

                    if (expandedHeight > _minHeight) {
                        Console.WriteLine("can expand");
                        SetTextCanExpand(d, true);
                    }

                    _expandedHeights.Add(new WeakReference(d), expandedHeight);
                    textBlock.Loaded -= elementLoaded;
                };
                textBlock.Loaded += elementLoaded;

                return null;
            }


            DoubleAnimation animation = new DoubleAnimation() {
                To = _minHeight,
                Duration = firstLoad ? TimeSpan.Zero : TimeSpan.FromSeconds(0.3),
                EasingFunction = new PowerEase() { Power = 2, EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath("Height"));

            Storyboard animationStoryboard = new Storyboard();
            animationStoryboard.Children.Add(animation);

            return animationStoryboard;
        }
    }
}
