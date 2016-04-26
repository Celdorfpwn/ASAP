using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SushiPikant.UI.Factories;
using SushiPikant.UI.Schedulers;
using SushiPikant.UI.SettigsViews;

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

        public TaskView Current { get;set; }

        public DevViewModel()
        {
            Initialize();
            SchedulerStarter.StartTaskUpdateScheduler();
        }

        private void Initialize()
        {
            Current = TaskViewModelsFactory.Instance.CurrentTask;
            _inProgress = new ObservableCollection<TaskView>(TaskViewModelsFactory.Instance.InProgress);
            _toDo = new ObservableCollection<TaskView>(TaskViewModelsFactory.Instance.ToDoTaskViews);
            _done = new ObservableCollection<TaskView>(TaskViewModelsFactory.Instance.Done);


            if (Current != null)
            {
                RemoveItemFromCollections(Current);
                Current.ViewModel.SwitchToBranch();
            }
        }

        public void Update(TaskView item, IEnumerable itemsSource)
        {
            if (Done.Contains(item))
            {
                return;
            }

            if (itemsSource.Equals(_inProgress))
            {
                item.ViewModel.InProgress();
            }
            else if (itemsSource.Equals(_toDo))
            {
                item.ViewModel.ToDo();
            }
 
            if (itemsSource.Equals(Done))
            {
                if (item.Equals(Current))
                {
                    if (item.ViewModel.CommitBranch())
                    {
                        Current = null;
                        Done.AddInOrder(item);
                        item.PopUpLastComment();
                    }
                    else
                    {
                        item.SetTemporarWarningMessage("Nothing to commit");
                    }
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



        public bool IsCurrent(string key)
        {
            return Current != null && Current.ViewModel.Key.Equals(key);
        }

        /// <summary>
        /// Removes an items from the view model collections
        /// </summary>
        /// <param name="item"></param>
        private void RemoveItemFromCollections(TaskView item)
        {
            _toDo.RemoveByKey(item.ViewModel.Key);
            _inProgress.RemoveByKey(item.ViewModel.Key);
            _done.RemoveByKey(item.ViewModel.Key);
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
                _inProgress.AddInOrder(Current);
            }
            Current = item;
            item.ViewModel.InProgress();
            item.ViewModel.SwitchToBranch();
            RaisePropertyChanged("Current");
        }




    }


 
}
