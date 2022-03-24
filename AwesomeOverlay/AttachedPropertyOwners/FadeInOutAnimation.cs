using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace AwesomeOverlay.AttachedPropertyOwners
{
    class FadeInOutAnimation : BaseAnimationTriggerAttachedProperty<FadeInOutAnimation>
    {
        public static bool GetChangeVisibility(DependencyObject d) =>
            (bool)d.GetValue(ChangeVisibilityProperty);

        public static void SetChangeVisibility(DependencyObject d, bool value) =>
            d.SetValue(ChangeVisibilityProperty, value);

        public static readonly DependencyProperty ChangeVisibilityProperty =
            DependencyProperty.RegisterAttached(
                "ChangeVisibility",
                typeof(bool),
                typeof(FadeInOutAnimation),
                new PropertyMetadata(
                    false
                ));

        public override Storyboard GetReverseAnimationStoryboard(DependencyObject d, bool firstLoad)
        {
            DoubleAnimation animation = new DoubleAnimation() {
                To = 0.0,
                Duration = firstLoad ? TimeSpan.Zero : TimeSpan.FromSeconds(0.3)
            };

            if (GetChangeVisibility(d)) {
                animation.Completed += (s, a) => {
                    if (!(d is FrameworkElement element))
                        return;

                    if (!GetTrigger(d))
                        element.Visibility = Visibility.Collapsed;
                };
            }

            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            Storyboard animationStoryboard = new Storyboard();
            animationStoryboard.Children.Add(animation);

            return animationStoryboard;
        }

        public override Storyboard GetAnimationStoryboard(DependencyObject d, bool firstLoad)
        {
            if (GetChangeVisibility(d) && d is FrameworkElement element) {
                element.Visibility = Visibility.Visible;
            }

            DoubleAnimation animation = new DoubleAnimation() {
                To = 1.0,
                Duration = firstLoad ? TimeSpan.Zero : TimeSpan.FromSeconds(0.3)
            };

            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            Storyboard animationStoryboard = new Storyboard();
            animationStoryboard.Children.Add(animation);

            return animationStoryboard;
        }
    }
}
