using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL;
using ModelsDI;
using SushiPikant.UI.SettigsViews;
using SushiPikant.UI.ViewModels;

namespace SushiPikant.UI.Factories
{
    public class TaskViewModelsFactory
    {


        private TasksModel TaskModels { get ; set; }

        public TaskViewModelsFactory()
        {
            TaskModels = ModelsDependencyInjection.Resolve<TasksModel>();
        }



        public IEnumerable<TaskView> ToDoTaskViews
        {
            get
            {
                foreach (var issue in TaskModels.TaskModels.Where(model => model.IsToDo))
                {
                    yield return new TaskView(new TaskViewModel(issue));
                }
            }
        }

        public IEnumerable<TaskView> InProgress
        {
            get
            {
                foreach (var issue in TaskModels.TaskModels.Where(model => model.IsInProgress))
                {
                    yield return new TaskView(new TaskViewModel(issue));
                }
            }
        }

        public IEnumerable<TaskView> Done
        {
            get
            {
                foreach (var issue in TaskModels.TaskModels.Where(model => model.IsDone))
                {
                    yield return new TaskView(new TaskViewModel(issue));
                }
            }

        }

        public TaskView CurrentTask
        {
            get
            {
                var current = TaskModels.Current;

                if (current != null)
                {
                    return new TaskView(new TaskViewModel(current));
                }
                else
                {
                    return null;
                }
                
            }
        }


        private static TaskViewModelsFactory Singleton { get;set; }


        public static TaskViewModelsFactory Instance
        {
            get
            {
                if (Singleton == null)
                {
                    Singleton = new TaskViewModelsFactory();
                }

                return Singleton;
            }
        }

    }
}
