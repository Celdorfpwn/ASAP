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
    public class TaskViewModel
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
        }

        public void PopulateComments()
        {
            Comments = new ObservableCollection<Comments>(Model.Comments);
        }
    }
}
