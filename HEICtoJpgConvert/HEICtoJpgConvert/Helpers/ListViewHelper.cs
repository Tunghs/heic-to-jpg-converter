using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace HEICtoJpgConvert.Helpers
{
    public class ListViewHelper
    {
        public static INotifyCollectionChanged GetSelectedItems(ListView obj)
        {
            return (INotifyCollectionChanged)obj.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(ListView obj, object value)
        {
            obj.SetValue(SelectedItemsProperty, (INotifyCollectionChanged)value);
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached("SelectedItems", typeof(INotifyCollectionChanged),
            typeof(ListViewHelper), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemsChanged)));

        public static void OnSelectedItemsChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs e)
        {
            if (Sender is ListView == false) return;

            ListView Target = Sender as ListView;
            SelectedItemsHelper Helper = SelectedItemsHelper.GetSelectedItemsHelper(Target);

            if (e.NewValue == null && Helper != null)
            {
                Helper.StopListener(Helper.SelectedItemsSource);
                Helper.StopListener(Helper.SelectedItemsTarget);
            }
            else if (e.NewValue != null && Helper == null)
            {
                Helper = new SelectedItemsHelper(Target.SelectedItems as IList, e.NewValue as IList);
                Target.SetValue(SelectedItemsHelper.SelectedItemsHelperProperty, Helper);

                Helper.StartListener(Target.SelectedItems as IList);
                Helper.StartListener(e.NewValue as IList);
            }
        }
    }
}
