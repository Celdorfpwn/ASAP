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
}
