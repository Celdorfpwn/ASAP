using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReview
{
    public interface ICodeReview
    {
        ReviewData CreateReview(string commit, string name, string description, string summary, string jiraKey);
        int CountReviewers(string id);
    }
}
