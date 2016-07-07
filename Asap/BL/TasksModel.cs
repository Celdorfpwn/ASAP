using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IssuesTracking;
using SourceControl;
using CodeReview;
using Repository;

namespace BL
{
    public class TasksModel
    {

        private ISourceControl _sourceControl { get;set; }

        private IIssuesTracking _issuesTracking { get;set; }

        private IRepositoryFactory _repositoryFactory { get; set; }
        private Issue[] Issues { get; set; }

        private List<TaskModel> Models { get;set; }

        private IEnumerable<ItVersion> Versions { get; set; }

        public TasksModel(ISourceControl sourceControl, IIssuesTracking issuesTracking, IRepositoryFactory repositoryFactory)
        {
            _sourceControl = sourceControl;
            _issuesTracking = issuesTracking;
            _repositoryFactory = repositoryFactory;
            LoadModels();
        }

        public IEnumerable<TaskModel> TaskModels
        {
            get
            {
                return Models;
            }
        }

        public TaskModel Current
        {
            get
            {
                var branchName = _sourceControl.GetCurrentBranch();
                var result = Models.FirstOrDefault(model => model.Key == branchName);
                if (result == null)
                {
                    _sourceControl.Checkout("master");
                    return null;
                }
                else
                {
                    return result;
                }
            }
        }


        private List<TaskModel> GetTaskModels()
        {
            var models = new List<TaskModel>();
            Issues = _issuesTracking.GetIssues().Issues;
            Versions = _issuesTracking.GetVersions().OrderByDescending(version => version.Name);
            foreach (var issue in Issues)
            {
                var model = new TaskModel(_sourceControl, _issuesTracking,_repositoryFactory ,issue);
                model.AvailableVersions = Versions;
                models.Add(model);
            }

            return models;
        }

        private void LoadModels()
        {
            var currentBranch = _sourceControl.GetCurrentBranch();
            Models = GetTaskModels();
            _sourceControl.Checkout(currentBranch);
        }

    }
}
