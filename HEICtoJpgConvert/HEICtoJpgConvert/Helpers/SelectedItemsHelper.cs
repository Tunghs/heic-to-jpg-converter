using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace HEICtoJpgConvert.Helpers
{
    internal class SelectedItemsHelper : IWeakEventListener
    {
        public static readonly DependencyProperty SelectedItemsHelperProperty =
            DependencyProperty.RegisterAttached("SelectedItemsHelper", typeof(SelectedItemsHelper), typeof(SelectedItemsHelper), new UIPropertyMetadata(null));


        public static SelectedItemsHelper GetSelectedItemsHelper(ListView TargetListView)
        {
            return TargetListView.GetValue(SelectedItemsHelperProperty) as SelectedItemsHelper;
        }

        public IList SelectedItemsSource { get; set; }
        public IList SelectedItemsTarget { get; set; }

        public SelectedItemsHelper(IList Source, IList Target)
        {
            this.SelectedItemsSource = Source;
            this.SelectedItemsTarget = Target;
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            CollectionChanged(sender, e as NotifyCollectionChangedEventArgs);
            return true;
        }

        public void StartListener(IList List)
        {
            CollectionChangedEventManager.AddListener(List as INotifyCollectionChanged, this);
        }
        public void StopListener(IList List)
        {
            CollectionChangedEventManager.RemoveListener(List as INotifyCollectionChanged, this);
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList SourceList = sender as IList;
            IList TargetList = SourceList == SelectedItemsSource ? SelectedItemsTarget : SelectedItemsSource;

            StopListener(TargetList);

            if (e.Action == NotifyCollectionChangedAction.Add) AddItems(TargetList, e);
            else if (e.Action == NotifyCollectionChangedAction.Move) ReplaceItems(TargetList, e);
            else if (e.Action == NotifyCollectionChangedAction.Replace) ReplaceItems(TargetList, e);
            else if (e.Action == NotifyCollectionChangedAction.Remove) RemoveItems(TargetList, e);
            else if (e.Action == NotifyCollectionChangedAction.Reset) UpdateItems(SourceList, TargetList);

            StartListener(TargetList);
        }

        private void RemoveItems(IList Target, NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < e.OldItems.Count; i++)
            {
                Target.RemoveAt(e.OldStartingIndex);
            }
        }

        private void AddItems(IList Target, NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < e.NewItems.Count; i++)
            {
                if (e.NewStartingIndex + i > Target.Count) Target.Add(e.NewItems[i]);
                else Target.Insert(e.NewStartingIndex + i, e.NewItems[i]);
            }
        }

        private void ReplaceItems(IList Target, NotifyCollectionChangedEventArgs e)
        {
            RemoveItems(Target, e);
            AddItems(Target, e);
        }

        private void UpdateItems(IList Source, IList Target)
        {
            Target.Clear();

            (Target as INotifyCollectionChanged).PasueNotifyCollectionChanged();

            foreach (object Item in Source)
            {
                Target.Add(Item);
            }

            (Target as INotifyCollectionChanged).ResumeNotifyCollectionChaged();
            (Target as INotifyCollectionChanged).Refresh();
        }
    }
}
