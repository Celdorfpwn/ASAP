using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssuesTracking
{
    public enum JiraItemStatus
    {
        None,
        Opened = 1,
        InProgress = 3,
        Reopened = 4,
        Resolved = 5,
        Closed = 6
    }
}
