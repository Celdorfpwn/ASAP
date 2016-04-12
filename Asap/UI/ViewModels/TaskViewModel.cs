using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using BL;
using IssuesTracking;


namespace SushiPikant.UI.ViewModels
{
    public class TaskViewModel : ViewModel
    {
 
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

            SeverityEnum = (SeverityEnum)Enum.Parse(typeof(SeverityEnum), model.Priority);

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
        }

        internal void ToDo()
        {
            Model.UpdateStatusOpen();
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
            Model.UpdateStatusResolved();
        }


        /// <summary>
        /// Commits a branch for code review
        /// </summary>
        public bool CommitBranch()
        {
            if (Model.CommitBranchDone())
            {
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
