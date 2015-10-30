using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Configuration;
using BL.Models;
using GitService;
using JiraService;

namespace BL.ModelFactories
{
    public class TasksFactory
    {

        public Jira Jira { get;private set; }


        private Issue[] Issues { get; set; }

        private List<TaskModel> Models { get;set; }

        public TasksFactory()
        {
            Jira = new Jira(ConfigurationModel.Instance.Credentials.Credentials64);

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
            var currentBranch = Git.GetCurrentBranch();
            Models = new List<TaskModel>();
            Issues = Jira.GetIssuesForPerson("ionut.apostol").Issues;

            foreach (var issue in Issues)
            {
                Models.Add(new TaskModel(issue));
            }
            Git.Checkout(currentBranch);
        }

        public TaskModel Current
        {
            get
            {
                var branchName = Git.GetCurrentBranch();
                var result = Models.FirstOrDefault(model => model.Issue.Key == branchName);
                if (result == null)
                {
                    Git.Checkout("master");
                    return null;
                }
                else
                {
                    return result;
                }
            }            
        }

        private static TasksFactory Singleton { get;set; }

        public static TasksFactory Instance
        {
            get
            {
                if (Singleton == null)
                {
                    Singleton = new TasksFactory();
                }

                return Singleton;
            }
        }

        public void SwitchToMaster()
        {
            throw new NotImplementedException();
        }
    }
}
