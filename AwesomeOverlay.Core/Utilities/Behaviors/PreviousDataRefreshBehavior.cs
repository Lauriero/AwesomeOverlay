using Microsoft.Xaml.Behaviors;

using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Data;

namespace AwesomeOverlay.Core.Utilities.Behaviors
{
    /// <summary>
    /// Behavior that refresh previous data when item is deleted
    /// </summary>
    public class PreviousDataRefreshBehavior : Behavior<ItemsControl>
    {
        protected override void OnAttached()
        {
            (AssociatedObject.Items as INotifyCollectionChanged).CollectionChanged += CollectionChanged;
        }

        protected override void OnDetaching()
        {
            (AssociatedObject.Items as INotifyCollectionChanged).CollectionChanged -= CollectionChanged;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
                CollectionViewSource.GetDefaultView(AssociatedObject.ItemsSource).Refresh();
        }
    }
}
