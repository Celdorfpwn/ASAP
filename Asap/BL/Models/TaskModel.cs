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

        /// <summary>
        /// Represents the Jira issue informations
        /// </summary>
        /// <returns>The jira issue</returns>
        public Issue Issue { get;private set; }


        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="issue">The issue</param>
        public TaskModel(Issue issue)
        {
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
                return TasksFactory.Instance.Jira.AppendCommentsForIssue(Issue).IssueComments;
            }
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
                return Issue.Key + " " + Issue.Field.Description;
            }

        }


        /// <summary>
        /// Checks out the issue branch.If doesn't exists it will be created.
        /// </summary>
        public void CheckoutBranch()
        {
            var checkoutResult = Git.Checkout(Issue.Key);
            if (checkoutResult == CommandStatus.BranchNotFound)
            {
                Git.Branch(Issue.Key);
                Git.Checkout(Issue.Key);
            }
        }


        /// <summary>
        /// Commits and push a branch in order to save the progress 
        /// </summary>
        public void CommitBranchInProgress()
        {
            Git.Commit(Issue.Key + "  work in progress " +DateTime.Today.ToShortDateString());
            Git.Push(Issue.Key);
            Git.Checkout("master");
        }

        /// <summary>
        /// Commits a branch done
        /// </summary>
        public void CommitBranchDone()
        {
            Git.Commit(DoneCommitMessage);
            Git.Push(Issue.Key);
            Git.Checkout("master");
        }
    }
}
