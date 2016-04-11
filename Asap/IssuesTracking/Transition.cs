using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssuesTracking
{
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
