
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace AwesomeOverlay.Core.Utilities.CollectionUtilities
{
    public static class CollectionExtensions
    {
        public static void SynchroniseWith<TInner, TOuter>(this ObservableCollection<TOuter> collectionToChange, ObservableCollection<TInner> collectionToSynchronizeWith, Converter<TInner, TOuter> itemConverter, Action<TOuter> elementAddedCallback = null)
        {
            collectionToChange.Clear();
            foreach (TInner item in collectionToSynchronizeWith) {
                collectionToChange.Add(itemConverter.Invoke(item));
            }

            collectionToSynchronizeWith.CollectionChanged += (o, e) => {
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Add:
                        TOuter newValue = itemConverter.Invoke((TInner)e.NewItems[0]);
                        collectionToChange.Insert(e.NewStartingIndex, newValue);

                        elementAddedCallback?.Invoke(newValue);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        collectionToChange.RemoveAt(e.OldStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        collectionToChange.RemoveAt(e.OldStartingIndex);
                        collectionToChange.Insert(e.NewStartingIndex, itemConverter.Invoke((TInner)e.NewItems[0]));
                        break;
                    case NotifyCollectionChangedAction.Move:
                        collectionToChange.Move(e.OldStartingIndex, e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        collectionToChange.Clear();
                        break;
                }
            };
        }

        public static void SynchroniseWith<TInner, TOuter>(this ObservableCollection<TOuter> collectionToChange, ObservableCollection<TInner> collectionToSynchronizeWith, Converter<TInner, IEnumerable<TOuter>> itemConverter)
        {
            collectionToSynchronizeWith.CollectionChanged += (o, e) => {
                IEnumerable<TOuter> newOuterElems = itemConverter.Invoke((TInner)e.NewItems[0]);
                int previousCount = collectionToSynchronizeWith.Take(e.NewStartingIndex).SelectMany(elem => itemConverter.Invoke(elem)).Count();
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Add:
                        for (int i = 0; i < newOuterElems.Count(); i++) {
                            collectionToChange.Insert(i + e.NewStartingIndex + (previousCount == 0 ? 0 : previousCount - 1), newOuterElems.ElementAt(i));
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        for (int i = 0; i < newOuterElems.Count(); i++) {
                            collectionToChange.RemoveAt(i + e.OldStartingIndex);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        IEnumerable<TOuter> oldOuterElems = itemConverter.Invoke((TInner)e.OldItems[0]);
                        for (int i = 0; i < oldOuterElems.Count(); i++) {
                            collectionToChange.RemoveAt(i + e.OldStartingIndex);
                        }
                        for (int i = 0; i < newOuterElems.Count(); i++) {
                            collectionToChange.Insert(i + e.NewStartingIndex + (previousCount == 0 ? 0 : previousCount - 1), newOuterElems.ElementAt(i));
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        for (int i = 0; i < newOuterElems.Count(); i++) {
                            collectionToChange.Move(i + e.OldStartingIndex, i + e.NewStartingIndex);
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        collectionToChange.Clear();
                        break;
                }
            };
        }

        /// <summary>
        /// Subsribes handler to specific action type of changing collection
        /// </summary>
        public static void SubscribeToChangedEvent<TInner>(this ObservableCollection<TInner> collection, NotifyCollectionChangedAction action, Action eventHandler) 
        {
            collection.CollectionChanged += (o, e) => {
                if (e.Action == action)
                    eventHandler?.Invoke();
            };
        }

        /// <summary>
        /// Setup menu controller with collection of menu elements. When one of the elements is selected, controller unselect other elements in collection
        /// </summary>
        public static void CreateMenu<MenuElement>(this IEnumerable<MenuElement> collection, Action<MenuElement> elementSelectedCallback = null)
            where MenuElement : IMenuElement
        {
            void BindElement(MenuElement item)
            {
                if (item.ElementController == null)
                    throw new Exception("Menu element controller of IMenuElement is undefined");

                item.ElementController.OnSelect += () => {
                    foreach (MenuElement _item in collection) {
                        if (!Object.Equals(_item, item)) {
                            _item.ElementController.Unselect();
                        }
                    }

                    elementSelectedCallback.Invoke(item);
                };
            }

            foreach (MenuElement item in collection) {
                BindElement(item);
            }

            if (collection is ObservableCollection<MenuElement> observable) {
                observable.CollectionChanged += (o, e) => {
                    if (e.Action == NotifyCollectionChangedAction.Add) {
                        foreach (MenuElement item in e.NewItems) {
                            BindElement(item);
                        }
                    }
                };
            }
        }
    }
}
