using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraService
{
    /// <summary>
    /// Jira issue Severity. This gets read from the 1st comment
    /// </summary>
    public enum Severity
    {
        None,
        S1,
        S2,
        S3,
        S4,
        S5,
        S6
    }

    /// <summary>
    /// Jira item status Enum
    /// </summary>
    public enum JiraItemStatus
    {
        None,
        Opened = 1,
        InProgress = 3,
        Reopened = 4,
        Resolved = 5,
        Closed = 6
    }

    public enum JiraTransition
    {
        None,
        Close = 701,
        Reopen = 3,
        StartProgress = 4,
        Resolve = 5,
        StopProgress = 301
    }
}
