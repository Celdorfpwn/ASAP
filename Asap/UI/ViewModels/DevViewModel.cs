using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SushiPikant.UI.TaskViews;

namespace SushiPikant.UI.ViewModels
{
    public class DevViewModel : ViewModel
    {
        public ObservableCollection<TaskView> ToDo { get; private set; }

        public ObservableCollection<TaskView> InProgress { get; private set; }

        public ObservableCollection<TaskView> Done { get;private set; }

        private List<ObservableCollection<TaskView>> Collections { get; set; }

        public TaskView Current { get;set; }

        public DevViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            InProgress = BuildCollection(2);
            ToDo = BuildCollection(5);
            Done = BuildCollection(3);

            Collections = new List<ObservableCollection<TaskView>>() { InProgress, ToDo,Done };

            Current = new TaskView(new TaskViewModel());
        }

        public void Update(TaskView item, IEnumerable itemsSource)
        {
            if (item.Equals(Current))
            {
                Current = null;
                RaisePropertyChanged("Current");
            }
            else
            {
                RemoveItemFromCollections(item);
            }

            (itemsSource as ObservableCollection<TaskView>).Add(item);
        }

        private void RemoveItemFromCollections(TaskView item)
        {
            Collections.ForEach(collection => collection.Remove(item));
        }


        public void Update(TaskView item)
        {
            RemoveItemFromCollections(item);
            if (Current != null)
            {
                InProgress.Add(Current);
            }
            Current = item;
            RaisePropertyChanged("Current");
        }



        private static ObservableCollection<TaskView> BuildCollection(int elements)
        {
            var result = new ObservableCollection<TaskView>();

            for (int number = 0; number < elements;number++)
            {
                result.Add(new TaskView(new TaskViewModel()));
            }

            return result;
        }
    }
}
