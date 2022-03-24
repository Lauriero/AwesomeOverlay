using AwesomeOverlay.Core.Model.Notifications.Attachments.Abstractions;
using AwesomeOverlay.ViewModels.Notifications.Attachments.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AwesomeOverlay.AttachedPropertyOwners
{
    /// <summary>
    /// Attach to <see cref="ScrollViewer"/>
    /// and control scrolling when elements select
    /// </summary>
    class CarouselScrollController
    {
        #region Dependancy properties
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached(
                "ItemsSource",
                typeof(IEnumerable<object>),
                typeof(CarouselScrollController),
                new PropertyMetadata(
                    new List<object>(),
                    new PropertyChangedCallback(OnItemsSourceChanged)
                ));

        public static readonly DependencyProperty ElementsOffsetProperty =
            DependencyProperty.RegisterAttached(
                "ElementsOffset",
                typeof(double),
                typeof(CarouselScrollController),
                new PropertyMetadata(
                    0.0
                ));

        public static readonly DependencyProperty SelectedItemProperty =
          DependencyProperty.RegisterAttached(
              "SelectedItem",
              typeof(object),
              typeof(CarouselScrollController),
              new PropertyMetadata(
                  default(object)
              ));
        #endregion

        #region Getters and setters
        public static IEnumerable<object> GetItemsSource(DependencyObject d) =>
            (IEnumerable<object>)d.GetValue(ItemsSourceProperty);
        public static void SetItemsSource(DependencyObject d, IEnumerable<object> value) =>
            d.SetValue(ItemsSourceProperty, value);

        public static double GetElementsOffset(DependencyObject d) =>
            (double)d.GetValue(ElementsOffsetProperty);
        public static void SetElementsOffset(DependencyObject d, double value) =>
            d.SetValue(ElementsOffsetProperty, value);

        public static object GetSelectedItem(DependencyObject d) =>
          d.GetValue(SelectedItemProperty);
        public static void SetSelectedItem(DependencyObject d, object value) =>
          d.SetValue(SelectedItemProperty, value);
        #endregion

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ScrollViewer scrollViewer) || !(scrollViewer.Content is ItemsControl itemsControl))
                return;

            itemsControl.Items.Clear();

            IEnumerable<object> collection = (IEnumerable<object>)e.NewValue;
            for (int i = 0; i < collection.Count(); i++) {
                ContentPresenter contentPresenter = new ContentPresenter();
                contentPresenter.Content = collection.ElementAt(i);
                contentPresenter.Margin = i == collection.Count() - 1 ? new Thickness(0) : new Thickness(0, 0, GetElementsOffset(d), 0);
                contentPresenter.MouseUp += (o, margs) => {
                    if (((ContentPresenter)o).Content != GetSelectedItem(d)) {
                        if (((ContentPresenter)o).Content is AttachmentBaseVM attachment)
                            attachment.OnSelect();
                        if (GetSelectedItem(d) is AttachmentBaseVM lastAttachment)
                            lastAttachment.OnUnselect();
                    }

                    double endValue = 0.0;
                    if (itemsControl.Items[itemsControl.Items.Count - 2] == o) {
                        endValue = scrollViewer.ScrollableWidth;
                    } else if (itemsControl.Items[1] == o) {
                        endValue = 0;
                    } else {
                        double sumWidth = itemsControl.Items.Cast<FrameworkElement>().TakeWhile(elem => elem != (FrameworkElement)o).Select(f => f.ActualWidth + f.Margin.Right).Sum();
                        endValue = sumWidth - scrollViewer.ActualWidth * 0.5 + ((FrameworkElement)o).ActualWidth / 2;
                    }

                    DoubleAnimation scrollAnimation = new DoubleAnimation() {
                        To = endValue,
                        Duration = TimeSpan.FromMilliseconds(500),
                        EasingFunction = new PowerEase()
                    };

                    scrollViewer.BeginAnimation(ScrollViewerAnimationHelper.AniHorizontalOffsetProperty, scrollAnimation);
                    SetSelectedItem(d, ((ContentPresenter)o).Content);
                };

                itemsControl.Items.Add(contentPresenter);
            }

            EventHandler loadedEventHandler = null;
            loadedEventHandler = (o, args) => {
                SetSelectedItem(d, collection.ElementAt(0));
                if (collection.ElementAt(0) is AttachmentBaseVM attachment) {
                    attachment.OnSelect();
                }


                itemsControl.Items.Cast<FrameworkElement>().ToList().ForEach(elem => {
                    elem.MaxWidth = scrollViewer.ActualWidth - (GetElementsOffset(d) * 2 + 30 * 2);
                });

                ApplyTempAreas(scrollViewer, itemsControl);
                itemsControl.LayoutUpdated -= loadedEventHandler;
            };

            itemsControl.LayoutUpdated += loadedEventHandler;
        }

        public static void ApplyTempAreas(ScrollViewer scrollViewer, ItemsControl itemsControl)
        {
            ContentPresenter firstPresenter = itemsControl.Items[0] as ContentPresenter;
            ContentPresenter lastPresenter = itemsControl.Items[itemsControl.Items.Count - 1] as ContentPresenter;

            Border tempArea1 = new Border();
            if (firstPresenter.ActualWidth > firstPresenter.MaxWidth) {
                tempArea1.Width = (scrollViewer.ActualWidth - firstPresenter.MaxWidth) / 2;
            } else {
                tempArea1.Width = (scrollViewer.ActualWidth - firstPresenter.ActualWidth) / 2;
            }

            Border tempArea2 = new Border();
            if (lastPresenter.ActualWidth > lastPresenter.MaxWidth) {
                tempArea2.Width = (scrollViewer.ActualWidth - lastPresenter.MaxWidth) / 2;
            } else {
                tempArea2.Width = (scrollViewer.ActualWidth - lastPresenter.ActualWidth) / 2;
            }

            itemsControl.Items.Insert(0, tempArea1);
            itemsControl.Items.Add(tempArea2);
        }
    }
}
