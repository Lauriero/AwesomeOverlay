using System.Windows;
using System.Windows.Controls;

namespace AwesomeOverlay.AttachedPropertyOwners
{
    /// <summary>
    /// Manage properties to animate <see cref="ScrollViewer.HorizontalOffsetProperty"/> of scroll viewer
    /// </summary>
    class ScrollViewerAnimationHelper
    {
        public static readonly DependencyProperty AniHorizontalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "AniHorizontalOffset",
                typeof(double),
                typeof(ScrollViewerAnimationHelper),
                new PropertyMetadata(
                    default(double),
                    OnHorizontalOffsetChanged
                ));

        public static double GetAniHorizontalOffset(DependencyObject d) =>
          (double)d.GetValue(AniHorizontalOffsetProperty);

        public static void SetAniHorizontalOffset(DependencyObject d, double value) =>
          d.SetValue(AniHorizontalOffsetProperty, value);

        private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ScrollViewer scrollViewer))
                return;

            scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
        }
    }
}
