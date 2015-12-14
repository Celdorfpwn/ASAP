using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.ModelFactories;
using JiraService;
using GitService;

namespace BL.Models
{
    /// <summary>
    /// The model representing a task
    /// </summary>
    public class TaskModel
    {
        private bool _isDone;

        private string ResolveMessage { get;set; }

        /// <summary>
        /// Represents the Jira issue informations
        /// </summary>
        /// <returns>The jira issue</returns>
        public Issue Issue { get;private set; }

        public Git Git
        {
            get
            {
                return Git.Instance;
            }
        }

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="issue">The issue</param>
        public TaskModel(Issue issue)
        {
            Issue = issue;

            if (Git.Checkout(Issue.Key) == ECommandStatus.OK)
            {
                var last = Git.GetLastCommit();
                _isDone = last.Contains(DoneCommitMessage);
            }
            else
            {
                _isDone = false;
            }
        }

        /// <summary>
        /// Returns the comments of the issue
        /// </summary>
        /// <returns>The comments of the issue</returns>
        public IEnumerable<Comments> Comments
        {
            get
            {
                return TasksFactory.Instance.Jira.AppendCommentsForIssue(Issue).IssueComments;
            }
        }

        public void SetResolveMessage(string message)
        {
            ResolveMessage = message;
            UpdateJiraStatusResolved();
        }


        /// <summary>
        /// Checks if the Issue is Open or Reopen
        /// </summary>
        /// <returns>True or false</returns>
        public bool IsToDo
        {
            get
            {
                if (IsDone)
                {
                    return false;
                }
                var status = Issue.Field.Status.Name;

                return status == "Open" || status == "Reopened";
            }
        }

        /// <summary>
        /// Adds a comment to Jira bug
        /// </summary>
        /// <param name="text"></param>
        public void AddComment(string text)
        {
        }


        /// <summary>
        /// Checks if a task is done
        /// </summary>
        /// <returns>True or false</returns>
        public bool IsDone
        {
            get
            {
                return _isDone || Issue.Field.Status.Id == (int)JiraItemStatus.Resolved;
            }
            set
            {
                _isDone = value;
            }
        }

        /// <summary>
        /// Updates the jira status to In Progress
        /// </summary>
        public void UpdateJiraStatusInProgress()
        {
            if (Issue.Field.Status.Id == (int)JiraItemStatus.Opened || Issue.Field.Status.Id == (int)JiraItemStatus.Reopened)
            {
                TasksFactory.Instance.Jira.SetStatus(Issue, JiraTransition.StartProgress);
                Issue.Field.Status.Id = (int)JiraItemStatus.InProgress;
            }
        }

        /// <summary>
        /// Updates the jira status to Open
        /// </summary>
        public void UpdateJiraStatusOpen()
        {
            if (Issue.Field.Status.Id == (int)JiraItemStatus.InProgress)
            {
                TasksFactory.Instance.Jira.SetStatus(Issue, JiraTransition.StopProgress);
                Issue.Field.Status.Id = (int)JiraItemStatus.Opened;
            }
        }

        /// <summary>
        /// Updates the jira status to Resolved
        /// </summary>
        public void UpdateJiraStatusResolved()
        {
            if (Issue.Field.Status.Id == (int)JiraItemStatus.InProgress)
            {
                TasksFactory.Instance.Jira.SetStatus(Issue, JiraTransition.Resolve,Issue.Key + " " + Issue.Field.Summary + ". "+ ResolveMessage);
                Issue.Field.Status.Id = (int)JiraItemStatus.Resolved;
            }
        }




        /// <summary>
        /// Checks if the Issue is in progress
        /// </summary>
        /// <returns>True or false</returns>
        public bool IsInProgress
        {
            get
            {
                if (IsDone)
                {
                    return false;
                }
                var status = Issue.Field.Status.Name;

                return status == "In Progress";
            }
        }

        private string DoneCommitMessage
        {
            get
            {
                return Issue.Key + " " + Issue.Field.Summary;
            }
        }


        /// <summary>
        /// Checks out the issue branch.If doesn't exists it will be created.
        /// </summary>
        public void CheckoutBranch()
        {
            CreateBranchIfDoesntExists();
            Git.Checkout(Issue.Key);
        }


        private void CreateBranchIfDoesntExists()
        {
            var checkoutResult = Git.Checkout(Issue.Key);
            if (checkoutResult == ECommandStatus.BranchNotFound)
            {
                Git.Branch(Issue.Key);
            }
        }

        /// <summary>
        /// Commits and push a branch in order to save the progress 
        /// </summary>
        public void CommitBranchInProgress()
        {
            CreateBranchIfDoesntExists();
            //Git.AddAll();
            Git.Commit(Issue.Key + "  work in progress " +DateTime.Today.ToShortDateString());
            Git.Push(Issue.Key);
            Git.Checkout("master");
        }


        /// <summary>
        /// Commits a branch done
        /// </summary>
        public bool CommitBranchDone()
        {
            CreateBranchIfDoesntExists();
            //Git.AddAll();
            var result = Git.Commit(DoneCommitMessage);

            if (result != ECommandStatus.OK)
            {
                return false;
            }
            //Git.PushToOrigin(Issue.Key);
            Git.Checkout("master");
            return true;
        }
    }
}
