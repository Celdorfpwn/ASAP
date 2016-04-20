using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using BL;
using IssuesTracking;


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
                return Model.Reporter;
            }
        }

        public string Description
        {
            get
            {
                return Model.Description;
            }
        }

        public string Severity
        {
            get
            {
                return SeverityEnum.ToString();
            }
        }

        public Brush Brush
        {
            get
            {
                switch (SeverityEnum)
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

        public ObservableCollection<string> FixedVersions { get; private set; }


        private string _statusMessage { get; set; }

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

            FixedVersions = new ObservableCollection<string>(model.FixedVersions.Select(version => version.Name));

            SeverityEnum = (SeverityEnum)Enum.Parse(typeof(SeverityEnum), model.Priority);

        }

        public void UpdateModel(TaskModel model)
        {
            Model = model;

            FixedVersions.Clear();

            foreach (var fixedVersion in Model.FixedVersions)
            {
                FixedVersions.Add(fixedVersion.Name);
            }

            SeverityEnum = (SeverityEnum)Enum.Parse(typeof(SeverityEnum), model.Priority);

            foreach (var property in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                RaisePropertyChanged(property.Name);
            }
        }

        public void AddComment(string text)
        {
            var model = new Comments();
            model.Body = text;


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
            if (Model.CommitBranchDone())
            {
                Resolve();
                RaisePropertyChanged("CurrentTask");
                StatusMessage = "Code Review";

                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
