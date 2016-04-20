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

            var models = IssuesTrackingModel.Models;

            var keys = View.ViewModel.ToDo
                            .Concat(View.ViewModel.Done)
                            .Concat(View.ViewModel.InProgress)
                            .Select(view => view.ViewModel.Key);

            UpdateNewModels(models,keys);

            RemoveModels(models,keys);

            var views = View.ViewModel.ToDo
                            .Concat(View.ViewModel.Done)
                            .Concat(View.ViewModel.InProgress);

            View.Dispatcher.Invoke(new Action(() => UpdateModelsStatus(models, views)));

        }

        private void UpdateModelsStatus(IEnumerable<TaskModel> models, IEnumerable<TaskView> taskViews)
        {
            foreach (var taskView in taskViews)
            {
                var jiraModel = models.FirstOrDefault(model => model.Key == taskView.ViewModel.Key);
                if (jiraModel != null)
                {

                    if (jiraModel.IsToDo && !View.ViewModel.ToDo.Contains(taskView))
                    {
                        RemoveByKey(taskView.ViewModel.Key);
                        View.ViewModel.ToDo.AddInOrder(taskView);
                        taskView.ViewModel.UpdateModel(jiraModel);
                    }
                    else if (jiraModel.IsInProgress && !View.ViewModel.InProgress.Contains(taskView))
                    {
                        RemoveByKey(taskView.ViewModel.Key);
                        View.ViewModel.InProgress.AddInOrder(taskView);
                        taskView.ViewModel.UpdateModel(jiraModel);
                    }
                    else if (jiraModel.IsDone && !View.ViewModel.Done.Contains(taskView))
                    {
                        RemoveByKey(taskView.ViewModel.Key);
                        View.ViewModel.Done.AddInOrder(taskView);
                        taskView.ViewModel.UpdateModel(jiraModel);
                    }

                }
            }
        }

        private void RemoveModels(IEnumerable<TaskModel> models,IEnumerable<string> keys)
        {
            var newKeys = models.Select(model => model.Key);

            var removeModelKeys = keys.Where(key => !newKeys.Contains(key));

            foreach (var key in removeModelKeys)
            {
                var action = new Action(() => RemoveByKey(key));
                View.Dispatcher.Invoke(action);
            }
        }

        private void UpdateNewModels(IEnumerable<TaskModel> models,IEnumerable<string> keys)
        {
            var newModels = models
                    .Where(model => !keys.Contains(model.Key)).ToList();

            foreach (var model in newModels)
            {
                var action = new Action(() => UpdateUI(model));
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

            if (!View.ViewModel.IsCurrent(model.Key))
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
}
