using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IssuesTracking;
using SourceControl;

namespace BL
{
    public class TasksModel
    {

        private ISourceControl _sourceControl { get;set; }

        private IIssuesTracking _issuesTracking { get;set; }

        private Issue[] Issues { get; set; }

        private List<TaskModel> Models { get;set; }

        public TasksModel(ISourceControl sourceControl,IIssuesTracking issuesTracking)
        {
            _sourceControl = sourceControl;
            _issuesTracking = issuesTracking;
            LoadModels();
        }

        public IEnumerable<TaskModel> TaskModels
        {
            get
            {
                return Models;
            }
        }


        private void LoadModels()
        {
            var currentBranch = _sourceControl.GetCurrentBranch();
            Models = new List<TaskModel>();
            Issues = _issuesTracking.GetIssues().Issues;

            foreach (var issue in Issues)
            {
                var model = new TaskModel(_sourceControl, _issuesTracking, issue);
                Models.Add(model);
            }
            _sourceControl.Checkout(currentBranch);
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
    }
}
