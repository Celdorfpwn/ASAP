﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IssuesTracking;
using SourceControl;
using Repository;
using Entities;

namespace BL
{
    public class TaskModel
    {
        private ISourceControl _sourceControl { get; set; }

        private IIssuesTracking _issuesTracking { get; set; }

        private IRepositoryFactory _repositoryFactory { get; set; }

        public Issue Issue { get; set; }

        private bool _isDone { get; set; }

        private string _resolveMessage { get; set; }

        private IEnumerable<ItVersion> _availableVersions { get; set; }

        public string Key
        {
            get
            {
                return Issue.Key;
            }
        }

        public bool Current
        {
            get
            {
                return Issue.Key == _sourceControl.GetCurrentBranch();
            }
        }

        public ItVersion FixedVersion { get; set; }

        public IEnumerable<ItVersion> FixedVersions
        {
            get
            {
                return Issue.Field.FixVersions;
            }
        }

        public IEnumerable<ItVersion> AvailableVersions
        {
            get
            {
                return _availableVersions.Where(version => Issue.Field.FixVersions.FirstOrDefault(fixedVersion => fixedVersion.Id == version.Id) == null);
            }
            set
            {
                _availableVersions = value;
            }
        }

        public IEnumerable<Attachment> Attachments
        {
            get
            {
                if (Issue.Field.Attachment == null)
                {
                    Issue.Field.Attachment = _issuesTracking.AppendAttachmentForIssue(Issue);
                }
                return Issue.Field.Attachment;

            }
        }

        public TaskModel(ISourceControl sourceControl, IIssuesTracking issuesTracking, IRepositoryFactory repositoryFactory, Issue issue)
        {
            _sourceControl = sourceControl;
            _issuesTracking = issuesTracking;
            _repositoryFactory = repositoryFactory;
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
        /// Adds a comment to Jira bug
        /// </summary>
        /// <param name="text"></param>
        public Comments AddComment(string text)
        {
            var comment = new Comments();
            comment.Author = Issue.Field.Assignee;
            comment.Body = text;
            _issuesTracking.AddMessage(Issue, text);
            return comment;
        }


        /// <summary>
        /// Checks if the Issue is Open or Reopen
        /// </summary>
        /// <returns>True or false</returns>
        public bool IsToDo
        {
            get
            {

                return Issue.Field.Status.Id == (int)JiraItemStatus.Open || Issue.Field.Status.Id == (int)JiraItemStatus.Reopened;
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
                return Issue.Field.Status.Id == (int)JiraItemStatus.InProgress;
            }
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


        public void SaveAttachemnt(string filepath, object fileId)
        {
            if (Issue.Field.Attachment != null)
            {
                var file = Issue.Field.Attachment.FirstOrDefault(attachment => attachment.Id.Equals(fileId));
                if (file != null)
                {
                    File.WriteAllText(filepath, file.Content);
                }
            }
        }

        public void OpenFile(object fileId)
        {
            if (Issue.Field.Attachment != null)
            {
                var file = Issue.Field.Attachment.FirstOrDefault(attachment => attachment.Id.Equals(fileId));
                if (file != null)
                {
                    Process.Start(file.Content);
                }
            }
        }

        /// <summary>
        /// Updates the jira status to In Progress
        /// </summary>
        public void UpdateStatusInProgress()
        {
            if (Issue.Field.Status.Id == (int)JiraItemStatus.Open || Issue.Field.Status.Id == (int)JiraItemStatus.Reopened)
            {
                Task.Run(() => _issuesTracking.SetStatus(Issue, JiraTransition.StartProgress, null, FixedVersion));
                Issue.Field.Status.Id = (int)JiraItemStatus.InProgress;
                Issue.Field.Status.Name = "In Progress";
            }
        }

        /// <summary>
        /// Updates the jira status to Open
        /// </summary>
        public void UpdateStatusOpen()
        {
            if (Issue.Field.Status.Id == (int)JiraItemStatus.InProgress)
            {
                Task.Run(() => _issuesTracking.SetStatus(Issue, JiraTransition.StopProgress, null, FixedVersion));
                Issue.Field.Status.Id = (int)JiraItemStatus.Open;
                Issue.Field.Status.Name = "In Open";
            }
        }

        /// <summary>
        /// Updates the jira status to Resolved
        /// </summary>
        public void UpdateStatusResolved()
        {
            if (Issue.Field.Status.Id == (int)JiraItemStatus.InProgress)
            {
                Task.Run(() => _issuesTracking.SetStatus(Issue, JiraTransition.Resolve, Issue.Key + " " + Issue.Field.Summary + ". " + _resolveMessage, FixedVersion));
                Issue.Field.Status.Id = (int)JiraItemStatus.Resolved;
                Issue.Field.Status.Name = "Done";
                //_sourceControl.Merge(Key);
            }
        }

        private void SaveIssue()
        {
            var asapTask = new AsapTask();
            asapTask.JiraId = Issue.Key;
            asapTask.CommitId = _sourceControl.GetCommitId(Key);
            asapTask.ReviewFinished = false;
            using(var repository = _repositoryFactory.NewRepository)
            {
                repository.Add<AsapTask>(asapTask);
                repository.Save();
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
            Task.Run(() => CheckoutCurrentBranch());
        }


        private void CheckoutCurrentBranch()
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
            _sourceControl.Push("origin",Issue.Key);
            _sourceControl.Checkout("master");
        }


        /// <summary>
        /// Commits a branch done
        /// </summary>
        public bool CommitBranchDone()
        {
            Task.Run(() => CommitCurrentBranchDone());
            return true;
        }

        private void CommitCurrentBranchDone()
        {
            CreateBranchIfDoesntExists();

            var result = _sourceControl.Commit(DoneCommitMessage);
            _sourceControl.Push("origin", Issue.Key);
            if (result == ECommandStatus.OK)
            {
                SaveIssue();
                _sourceControl.Checkout("master");
            }
        }

        public bool IsInCodeReview
        {
            get
            {
                using(var repository = _repositoryFactory.NewRepository)
                {
                    var dbTask = repository.Get<AsapTask>(Issue.Key);

                    if(dbTask!= null)
                    {
                        return !dbTask.ReviewFinished;
                    }
                }

                return false;
            }
        }
    }
}
