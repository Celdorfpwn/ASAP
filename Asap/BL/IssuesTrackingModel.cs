using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IssuesTracking;
using SourceControl;

namespace BL
{
    public class IssuesTrackingModel
    {
        private IIssuesTracking _issuesTracking { get; set; }

        private ISourceControl _sourceControl { get; set; }

        public IssuesTrackingModel(ISourceControl sourceControl,IIssuesTracking issuesTracking)
        {
            _issuesTracking = issuesTracking;
            _sourceControl = sourceControl;
        }


        public IEnumerable<TaskModel> Models
        {
            get
            {
                return GetTaskModels();
            }
        }

        private List<TaskModel> GetTaskModels()
        {
            var models = new List<TaskModel>();
            var issues = _issuesTracking.GetIssues().Issues;
            var versions = _issuesTracking.GetVersions().OrderByDescending(version => version.Name);
            foreach (var issue in issues)
            {
                var model = new TaskModel(_sourceControl, _issuesTracking, issue);
                model.AvailableVersions = versions;
                models.Add(model);
            }

            return models;
        }
    }
}
