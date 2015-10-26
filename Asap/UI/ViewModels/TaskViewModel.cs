using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Models;
using JiraService;

namespace SushiPikant.UI.ViewModels
{
    public class TaskViewModel : ViewModel
    {
 
        public string Title
        {
            get
            {
                return Model.Issue.Key;
            }
        }

        public string Reporter
        {
            get
            {
                return Model.Issue.Field.Reporter.DisplayName;
            }
        }

        public string Description
        {
            get
            {
                return Model.Issue.Field.Summary;
            }
        }

        public string Severity
        {
            get
            {
                return SeverityEnum.ToString();
            }
        }

        public int SeverityValue
        {
            get
            {
                return (int)SeverityEnum;
            }
        }

        public string StatusMessage
        {
            get
            {
                if (Model.IsDone)
                {
                    return "Code Review";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public void SetResolveMessage(string message)
        {
            Model.SetResolveMessage(message);
        }

        public ObservableCollection<Comments> Comments { get;private set; }

        private SeverityEnum SeverityEnum { get; set; }

        private TaskModel Model { get;set; }

        public TaskViewModel(TaskModel model)
        {
            Model = model;

            SeverityEnum = (SeverityEnum)Enum.Parse(typeof(SeverityEnum), model.Issue.Field.Priority.Name);

        }

        public void AddComment(string text)
        {
            var model = new Comments();
            model.Body = text;
            model.Author = new Person { DisplayName = "ionut.apostol" };

            Comments.Add(model);

            Model.AddComment(text);
        }

        /// <summary>
        /// Populates the issue comments
        /// </summary>
        public void PopulateComments()
        {
            Comments = new ObservableCollection<Comments>(Model.Comments);
        }

        public void InProgress()
        {
            Model.UpdateJiraStatusInProgress();
        }

        internal void ToDo()
        {
            Model.UpdateJiraStatusOpen();
        }



        /// <summary>
        /// Switch or creates to the issue branch
        /// </summary>
        public void SwitchToBranch()
        {
            Model.CheckoutBranch();
        }

        /// <summary>
        /// Saves a branch
        /// </summary>
        public void SaveBranch()
        {
            Model.CommitBranchInProgress();
        }

        internal void Resolve()
        {
            Model.UpdateJiraStatusResolved();
        }


        /// <summary>
        /// Commits a branch for code review
        /// </summary>
        public bool CommitBranch()
        {
            if (Model.CommitBranchDone())
            {
                Model.IsDone = true;
                RaisePropertyChanged("StatusMessage");
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
