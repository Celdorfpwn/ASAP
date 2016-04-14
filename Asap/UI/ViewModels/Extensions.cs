using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using SushiPikant.UI.SettigsViews;

namespace SushiPikant.UI.ViewModels
{
    public static class Extensions
    {
        public static void AddInOrder(this ObservableCollection<TaskView> list, TaskView item)
        {
            list.Add(item);

            var order = list.OrderByDescending(elem => elem.ViewModel.SeverityValue).ToList();

            list.Clear();

            order.ForEach(orderItem => list.Add(orderItem));
        }

        public static void RemoveByKey(this ObservableCollection<TaskView> list, string key)
        {
            var removeView = list.FirstOrDefault(view => view.ViewModel.Key == key);
            if (removeView != null)
            {
                list.Remove(removeView);
            }
        }
    }
}
