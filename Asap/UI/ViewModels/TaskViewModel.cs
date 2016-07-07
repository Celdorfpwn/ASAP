using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using BL;
using IssuesTracking;
using SushiPikant.UI.SettigsViews;

namespace SushiPikant.UI.ViewModels
{
    public class TaskViewModel : ViewModel
    {

        public string Key
        {
            get
            {
                return Model.Key;
            }
        }

        public string Title
        {
            get
            {
                return Model.Key;
            }
        }

        public string Reporter
        {
            get
            {
                return Model.Issue.Field.Reporter.DisplayName;
            }
        }

        public string Summary
        {
            get
            {
                return Model.Issue.Field.Summary;
            }
        }

        public string Description
        {
            get
            {
                return Model.Issue.Field.Description;
            }
        }

        public string Severity
        {
            get
            {
                return TaskSeverity.ToString();
            }
        }

        public string CreateDate
        {
            get
            {
                return Model.Issue.Field.CreatedDate;
            }
        }

        public string Iteration
        {
            get
            {
                return Model.Issue.Field.Iteration;
            }
        }

        public string Status
        {
            get
            {
                return Model.Issue.Field.Status.Name;
            }
        }

        public string Resolution
        {
            get
            {
                if (Model.Issue.Field.Resolution != null)
                {
                    return Model.Issue.Field.Resolution.Name;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string IssueType
        {
            get
            {
                return Model.Issue.Field.IssueType.Name;
            }
        }

        public Brush Brush
        {
            get
            {
                switch (TaskSeverity)
                {
                    case SeverityEnum.Blocker:
                        return Brushes.Red;
                    case SeverityEnum.Critical:
                        return Brushes.Orange;
                    case SeverityEnum.Major:
                        return Brushes.Yellow;
                    case SeverityEnum.Minor:
                        return Brushes.Green;
                    default:
                        return Brushes.Green;
                }
            }
        }

        public string SwitchTo
        {
            get
            {
                if (Model.Current)
                {
                    return "Done";
                }
                else
                {
                    return "Current"; ;
                }
            }
        }

        public Visibility SwitchToVisibility
        {
            get
            {
                if (Model.IsDone)
                {
                    return Visibility.Hidden;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }


        public int SeverityValue
        {
            get
            {
                return (int)TaskSeverity;
            }
        }

        public string StatusMessage
        {
            get
            {

                if (Model.IsInCodeReview)
                {
                    return "Code Review";
                }
                return _statusMessage;
            }
            set
            {
                _statusMessage = value;
                RaisePropertyChanged("StatusMessage");
            }
        }

        public Visibility CurrentTask
        {
            get
            {
                if (Model.Current)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }

        public IEnumerable<ItVersion> AvailableVersions
        {
            get
            {
                return Model.AvailableVersions;
            }
        }

        public ItVersion FixedVersion
        {
            get
            {
                return Model.FixedVersion;
            }
            set
            {
                Model.FixedVersion = value;
            }
        }

        public IEnumerable<Attachment> Attachments
        {
            get
            {
                return Model.Attachments;
            }
        }

        public ObservableCollection<string> FixedVersions { get; private set; }

        public ObservableCollection<Comments> Comments
        {
            get
            {
                if (_comments == null)
                {
                    _comments = new ObservableCollection<Comments>(Model.Comments);
                }

                return _comments;
            }
        }


        private string _statusMessage { get; set; }


        private SeverityEnum TaskSeverity
        {
            get
            {
                return (SeverityEnum)Enum.Parse(typeof(SeverityEnum), Model.Issue.Field.Priority.Name);
            }
        }


        private ObservableCollection<Comments> _comments { get; set; }

        private TaskModel Model { get; set; }

        public TaskViewModel(TaskModel model)
        {
            Model = model;

            FixedVersions = new ObservableCollection<string>(model.FixedVersions.Select(version => version.Name));

        }

        public void SwitchToTask()
        {
            if (Model.Current)
            {
                DevView.Instance.ViewModel.ResolveCurrent();
            }
            else
            {
                DevView.Instance.ViewModel.SetCurrent(this);
            }
            RaisePropertyChangedForAll();
        }

        public void UpdateModel(TaskModel model)
        {
            Model = model;

            FixedVersions.Clear();

            _comments = null;

            foreach (var fixedVersion in Model.FixedVersions)
            {
                FixedVersions.Add(fixedVersion.Name);
            }

            RaisePropertyChangedForAll();

        }

        public void SetResolveMessage(string message)
        {
            Model.SetResolveMessage(message);
        }

        public void AddComment(string text)
        {
            Comments.Add(Model.AddComment(text));
        }

        public void SaveFile(string filepath, object fileId)
        {
            Model.SaveAttachemnt(filepath, fileId);
        }


        public void OpenFile(object fileId)
        {
            Model.OpenFile(fileId);
        }



        public void InProgress()
        {
            Model.UpdateStatusInProgress();
            RaisePropertyChanged("CurrentTask");
        }

        public void ToDo()
        {
            Model.UpdateStatusOpen();
            RaisePropertyChanged("CurrentTask");
        }

        /// <summary>
        /// Switch or creates to the issue branch
        /// </summary>
        public void SwitchToBranch()
        {
            Model.CheckoutBranch();
            RaisePropertyChanged("CurrentTask");
        }

        /// <summary>
        /// Saves a branch
        /// </summary>
        public void SaveBranch()
        {
            Model.CommitBranchInProgress();
            RaisePropertyChanged("CurrentTask");
        }

        public void Resolve()
        {
            Model.UpdateStatusResolved();
            if (FixedVersion != null)
            {
                FixedVersions.Add(FixedVersion.Name);
            }
        }


        /// <summary>
        /// Commits a branch for code review
        /// </summary>
        public bool CommitBranch()
        {
            Model.CommitBranchDone();
            Resolve();
            RaisePropertyChanged("CurrentTask");
            StatusMessage = "Code Review";

            return true;


        }

    }
}
