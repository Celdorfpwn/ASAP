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

            Current = TaskViewModelsFactory.Instance.CurrentTask;
        }

        public void Update(TaskView item, IEnumerable itemsSource)
        {

            if (Done.Contains(item))
            {
                return;
            }

            if (itemsSource.Equals(Done))
            {
                if (item.Equals(Current))
                {
                    Current = null;
                    Done.AddInOrder(item);
                    item.PopUpLastComment();
                    item.ViewModel.CommitBranch();
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (item.Equals(Current))
                {
                    Current.ViewModel.SaveBranch();
                    Current = null;
                    RaisePropertyChanged("Current");
                }
                else
                {
                    RemoveItemFromCollections(item);
                }

                var source = (itemsSource as ObservableCollection<TaskView>);

                source.AddInOrder(item);
            }



        }


        /// <summary>
        /// Removes an items from the view model collections
        /// </summary>
        /// <param name="item"></param>
        private void RemoveItemFromCollections(TaskView item)
        {
            Collections.ForEach(collection => collection.Remove(item));
        }


        /// <summary>
        /// Updates the Current task view
        /// </summary>
        /// <param name="item">The Task View</param>
        public void Update(TaskView item)
        {
            if (Done.Contains(item))
            {
                return;
            }

            RemoveItemFromCollections(item);
            if (Current != null)
            {
                Current.ViewModel.SaveBranch();
                _inProgress.Add(Current);
            }
            Current = item;
            item.ViewModel.SwitchToBranch();
            RaisePropertyChanged("Current");
        }

    }


 
}
