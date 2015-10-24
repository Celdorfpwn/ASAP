using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SushiPikant.UI.Factories;
using SushiPikant.UI.TaskViews;

namespace SushiPikant.UI.ViewModels
{
    public class DevViewModel : ViewModel
    {


        private ObservableCollection<TaskView> _toDo { get; set; }

        private ObservableCollection<TaskView> _inProgress { get; set; }

        private ObservableCollection<TaskView> _done { get; set; }

        public ObservableCollection<TaskView> ToDo
        {
            get
            {
                return _toDo;
            }
        }

        public ObservableCollection<TaskView> InProgress
        {
            get
            {
                return _inProgress;
            }
        }

        public ObservableCollection<TaskView> Done
        {
            get
            {
                return _done;
            }
        }

        private List<ObservableCollection<TaskView>> Collections { get; set; }

        public TaskView Current { get;set; }

        public DevViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            _inProgress = new ObservableCollection<TaskView>(TaskViewModelsFactory.Instance.InProgress);
            _toDo = new ObservableCollection<TaskView>(TaskViewModelsFactory.Instance.ToDoTaskViews);
            _done = new ObservableCollection<TaskView>();

            Collections = new List<ObservableCollection<TaskView>>() { _inProgress, _toDo,_done };

            //Current = new TaskView(new TaskViewModel());
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

            var source = (itemsSource as ObservableCollection<TaskView>);

            source.AddInOrder(item);

            AfterDrop(item, itemsSource);

        }

        private void AfterDrop(TaskView item, IEnumerable itemsSource)
        {
            if (itemsSource.Equals(_done))
            {
                item.PopUpLastComment();
            }
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
                _inProgress.Add(Current);
            }
            Current = item;
            RaisePropertyChanged("Current");
        }

    }


 
}
