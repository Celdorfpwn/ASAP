using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BL;
using ModelsDI;
using Quartz;
using SushiPikant.UI.SettigsViews;
using SushiPikant.UI.ViewModels;

namespace SushiPikant.UI.Schedulers
{
    public class TasksUpdateJob : IJob
    {
        private IssuesTrackingModel IssuesTrackingModel { get; set; }

        private DevView View { get; set; }

        public TasksUpdateJob()
        {
            IssuesTrackingModel = ModelsDependencyInjection.Resolve<IssuesTrackingModel>();
            View = DevView.Instance;
        }

        public void Execute(IJobExecutionContext context)
        {
            var keys = View.ViewModel.ToDo.Concat(View.ViewModel.Done).Concat(View.ViewModel.InProgress)
                .Select(view => view.ViewModel.Key);

            var models = IssuesTrackingModel.Models;

            var newKeys = models.Select(model => model.Key);

            var newModels = models
                .Where(model => !keys.Contains(model.Key)).ToList();

            var removeModelKeys = keys.Where(key => !newKeys.Contains(key));

            foreach (var model in newModels)
            {
                var action = new Action(() => UpdateUI(model));
                View.Dispatcher.Invoke(action);
            }

            foreach (var key in removeModelKeys)
            {
                var action = new Action(() => RemoveByKey(key));
                View.Dispatcher.Invoke(action);
            }
        }

        private void RemoveByKey(string key)
        {
            View.ViewModel.ToDo.RemoveByKey(key);
            View.ViewModel.InProgress.RemoveByKey(key);
            View.ViewModel.Done.RemoveByKey(key);
        }

        private void UpdateUI(TaskModel model)
        {
            var taskView = new TaskView(new TaskViewModel(model));

            if (model.IsToDo)
            {
                View.ViewModel.ToDo.AddInOrder(taskView);
            }
            else if (model.IsInProgress)
            {
                View.ViewModel.InProgress.AddInOrder(taskView);
            }
            else if (model.IsDone)
            {
                View.ViewModel.Done.AddInOrder(taskView);
            }
        }
    }
}
