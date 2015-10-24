using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Models;
using JiraService;

namespace BL.ModelFactories
{
    public class TasksFactory
    {

        private Jira Jira { get;set; }


        private Issue[] Issues { get; set; }

        private List<TaskModel> Models { get;set; }

        public TasksFactory()
        {
            Jira = new Jira();

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
            Models = new List<TaskModel>();
            Issues = Jira.GetIssuesForPerson("ionut.apostol").Issues;

            var statuses = Issues.Select(i => i.Field.Status.Name).ToList();
            foreach (var issue in Issues)
            {
                Models.Add(new TaskModel(issue));
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

        
    }
}
