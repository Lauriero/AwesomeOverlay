
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace AwesomeOverlay.AttachedPropertyOwners
{
    public abstract class BaseAnimationTriggerAttachedProperty<Parent>
        where Parent : BaseAnimationTriggerAttachedProperty<Parent>, new()
    {
        private static Parent _instance = new Parent();
        private static Dictionary<WeakReference, bool> _isElementAlreadyLoaded = new Dictionary<WeakReference, bool>();

        public static bool GetTrigger(DependencyObject d) =>
            (bool)d.GetValue(TriggerProperty);

        public static void SetTrigger(DependencyObject d, bool value) =>
            d.SetValue(TriggerProperty, value);

        public static readonly DependencyProperty TriggerProperty =
            DependencyProperty.RegisterAttached(
                "Trigger",
                typeof(bool),
                typeof(BaseAnimationTriggerAttachedProperty<Parent>),
                new PropertyMetadata(
                    false,
                    null,
                    new CoerceValueCallback(OnTriggerUpdated)
                ));


        private static object OnTriggerUpdated(DependencyObject d, object baseValue)
        {
            BaseAnimationTriggerAttachedProperty<Parent> instance = _instance as BaseAnimationTriggerAttachedProperty<Parent>;

            KeyValuePair<WeakReference, bool> isAlreadyLoaded = _isElementAlreadyLoaded.FirstOrDefault(f => f.Key.Target == d);
            if ((bool)baseValue) {
                if (d is FrameworkElement element) {
                    instance?.GetAnimationStoryboard(d, isAlreadyLoaded.Key == null)?.Begin(element);
                } else if (d is FrameworkContentElement contentElement) {
                    instance?.GetAnimationStoryboard(d, isAlreadyLoaded.Key == null)?.Begin(contentElement);
                }
            } else {
                if (d is FrameworkElement element) {
                    instance?.GetReverseAnimationStoryboard(d, isAlreadyLoaded.Key == null)?.Begin(element);
                } else if (d is FrameworkContentElement contentElement) {
                    instance?.GetReverseAnimationStoryboard(d, isAlreadyLoaded.Key == null)?.Begin(contentElement);
                }
            }

            if (isAlreadyLoaded.Key == null) {
                _isElementAlreadyLoaded.Add(new WeakReference(d), true);
            }

            return baseValue;
        }

        /// <summary>
        /// Returns stroyboard with animations to start them when trigger value sets to true
        /// </summary>
        /// <param name="parameter"></param>
        public abstract Storyboard GetAnimationStoryboard(DependencyObject d, bool firstLoad);

        /// <summary>
        /// Returns stroyboard with animations to start them when trigger value sets to false
        /// </summary>
        /// <param name="parameter"></param>
        public abstract Storyboard GetReverseAnimationStoryboard(DependencyObject d, bool firstLoad);
    }
}
