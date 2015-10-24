using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraService;

namespace BL.Models
{
    public class TaskModel
    {

        public Issue Issue { get;set; }

        public TaskModel(Issue issue)
        {
            Issue = issue;
        }


        public bool IsToDo
        {
            get
            {
                var status = Issue.Field.Status.Name;

                return status == "Open" || status == "Reopened";
            }
        }

        public bool IsInProgress
        {
            get
            {
                var status = Issue.Field.Status.Name;

                return status == "In Progress";
            }
        }
    }
}
