using AwesomeOverlay.Decorators;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Diagnostics;
using System.Windows.Media.Animation;

namespace AwesomeOverlay.AttachedPropertyOwners
{
    class ExpandingAnimation
    {
        private static Dictionary<WeakReference, double> _elementHeights = new Dictionary<WeakReference, double>();

        private static void UpdateExpandingContent(DependencyObject d)
        {
            if (!(d is FrameworkElement element))
                return;

            double elementHeight = _elementHeights.FirstOrDefault(f => f.Key.Target == d).Value;
            if (elementHeight == 0) {
                RoutedEventHandler loadedHandler = null;
                loadedHandler = (o, e) => {
                    elementHeight = element.ActualHeight;
                    _elementHeights[new WeakReference(d)] = elementHeight;

                    DoubleAnimation animation1 = null;
                    if (GetBinding(d) == GetValue(d)) {
                        animation1 = new DoubleAnimation() {
                            To = GetMaxHeight(d) / elementHeight,
                            Duration = TimeSpan.FromMilliseconds(200)
                        };
                    } else {
                        animation1 = new DoubleAnimation() {
                            To = GetMinHeight(d) / elementHeight,
                            Duration = TimeSpan.FromMilliseconds(200)
                        };
                    }

                    element.BeginAnimation(Clipper.HeightFractionProperty, animation1);
                    element.Loaded -= loadedHandler;
                };

                element.Loaded += loadedHandler;
                return;
            }

            DoubleAnimation animation = null;
            if (GetBinding(d) == GetValue(d)) {
                Console.WriteLine("open");
                animation = new DoubleAnimation() {
                    To = GetMaxHeight(d) / elementHeight,
                    Duration = TimeSpan.FromMilliseconds(200)
                };
            } else {
                animation = new DoubleAnimation() {
                    To = GetMinHeight(d) / elementHeight,
                    Duration = TimeSpan.FromMilliseconds(200)
                };
            }

            Console.WriteLine(animation.To);
            Console.WriteLine(elementHeight);

            element.BeginAnimation(Clipper.HeightFractionProperty, animation);
        }

        #region Properties
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.RegisterAttached("Binding", typeof(bool), typeof(ExpandingAnimation), new PropertyMetadata(false, (d, e) => UpdateExpandingContent(d)));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(bool), typeof(ExpandingAnimation), new PropertyMetadata(false, (d, e) => UpdateExpandingContent(d)));

        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.RegisterAttached("MinHeight", typeof(double), typeof(ExpandingAnimation), new PropertyMetadata(default(double), (d, e) => UpdateExpandingContent(d)));

        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.RegisterAttached("MaxHeight", typeof(double), typeof(ExpandingAnimation), new PropertyMetadata(default(double), (d, e) => UpdateExpandingContent(d)));
        #endregion

        #region Getters and setters
        public static double GetMinHeight(DependencyObject d) =>
          (double)d.GetValue(MinHeightProperty);

        public static void SetMinHeight(DependencyObject d, double value) =>
          d.SetValue(MinHeightProperty, value);


        public static double GetMaxHeight(DependencyObject d) =>
          (double)d.GetValue(MaxHeightProperty);

        public static void SetMaxHeight(DependencyObject d, double value) =>
          d.SetValue(MaxHeightProperty, value);


        public static bool GetValue(DependencyObject d) =>
          (bool)d.GetValue(ValueProperty);

        public static void SetValue(DependencyObject d, bool value) =>
          d.SetValue(ValueProperty, value);


        public static bool GetBinding(DependencyObject d) =>
          (bool)d.GetValue(BindingProperty);

        public static void SetBinding(DependencyObject d, bool value) =>
          d.SetValue(BindingProperty, value);
        #endregion
    }
}
