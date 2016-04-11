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

    /// <summary>
    /// Jira Issue types Enum
    /// </summary>
    public enum JiraIssueType
    {
        None,
        Bug = 44,
        UserStory = 43
    }
}
