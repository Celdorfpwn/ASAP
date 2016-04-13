using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IssuesTracking;
using SourceControl;

namespace BL
{
    public class TaskModel
    {
        private ISourceControl _sourceControl { get; set; }

        private IIssuesTracking _issuesTracking { get; set; }

        private Issue Issue { get; set; }

        private bool _isDone { get; set; }

        private string _resolveMessage { get; set; }


        public string Key
        {
            get
            {
                return Issue.Key;
            }
        }

        public string Reporter
        {
            get
            {
                return Issue.Field.Reporter.DisplayName;
            }
        }

        public string Description
        {
            get
            {
                return Issue.Field.Summary;
            }
        }

        public string Priority
        {
            get
            {
                return Issue.Field.Priority.Name;
            }
        }

        public ItVersion FixedVersion { get; set; }

        public IEnumerable<ItVersion> AvailableVersions { get; internal set; }



        public TaskModel(ISourceControl sourceControl, IIssuesTracking issuesTracking,Issue issue)
        {
            _sourceControl = sourceControl;
            _issuesTracking = issuesTracking;
            Issue = issue;

        }


        /// <summary>
        /// Returns the comments of the issue
        /// </summary>
        /// <returns>The comments of the issue</returns>
        public IEnumerable<Comments> Comments
        {
            get
            {
                return _issuesTracking.AppendCommentsForIssue(Issue).IssueComments;
            }
        }

        public void SetResolveMessage(string message)
        {
            _resolveMessage = message;
            UpdateStatusResolved();
        }


        /// <summary>
        /// Checks if the Issue is Open or Reopen
        /// </summary>
        /// <returns>True or false</returns>
        public bool IsToDo
        {
            get
            {
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
            _issuesTracking.AddMessage(Issue, text);
        }


        /// <summary>
        /// Checks if a task is done
        /// </summary>
        /// <returns>True or false</returns>
        public bool IsDone
        {
            get
            {
                return Issue.Field.Status.Id == (int)JiraItemStatus.Resolved;
            }
        }

        /// <summary>
        /// Updates the jira status to In Progress
        /// </summary>
        public void UpdateStatusInProgress()
        {
            if (Issue.Field.Status.Id == (int)JiraItemStatus.Open || Issue.Field.Status.Id == (int)JiraItemStatus.Reopened)
            {
                _issuesTracking.SetStatus(Issue, JiraTransition.StartProgress,null, FixedVersion);
                Issue.Field.Status.Id = (int)JiraItemStatus.InProgress;
            }
        }

        /// <summary>
        /// Updates the jira status to Open
        /// </summary>
        public void UpdateStatusOpen()
        {
            if (Issue.Field.Status.Id == (int)JiraItemStatus.InProgress)
            {
                _issuesTracking.SetStatus(Issue, JiraTransition.StopProgress,null, FixedVersion);
                Issue.Field.Status.Id = (int)JiraItemStatus.Open;
            }
        }

        /// <summary>
        /// Updates the jira status to Resolved
        /// </summary>
        public void UpdateStatusResolved()
        {
            if (Issue.Field.Status.Id == (int)JiraItemStatus.InProgress)
            {
                _issuesTracking.SetStatus(Issue, JiraTransition.Resolve, Issue.Key + " " + Issue.Field.Summary + ". " + _resolveMessage, FixedVersion);
                Issue.Field.Status.Id = (int)JiraItemStatus.Resolved;
                _sourceControl.Checkout("master");
                _sourceControl.Merge(Key);
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
            _sourceControl.Checkout(Issue.Key);
            _sourceControl.Merge("master");
        }


        private void CreateBranchIfDoesntExists()
        {
            var checkoutResult = _sourceControl.Checkout(Issue.Key);
            if (checkoutResult == ECommandStatus.BranchNotFound)
            {
                _sourceControl.Branch(Issue.Key);
            }
        }

        /// <summary>
        /// Commits and push a branch in order to save the progress 
        /// </summary>
        public void CommitBranchInProgress()
        {
            CreateBranchIfDoesntExists();
            _sourceControl.Commit(Issue.Key + "  work in progress " + DateTime.Today.ToShortDateString());
            _sourceControl.Push(Issue.Key);
            _sourceControl.Checkout("master");
        }


        /// <summary>
        /// Commits a branch done
        /// </summary>
        public bool CommitBranchDone()
        {
            CreateBranchIfDoesntExists();
            var result = _sourceControl.Commit(DoneCommitMessage);

            if (result != ECommandStatus.OK)
            {
                return false;
            }
            _sourceControl.Checkout("master");
            return true;
        }
    }
}
