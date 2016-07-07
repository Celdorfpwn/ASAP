using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolsConfiguration
{
    public interface ICodeReviewConfig
    {
        string Username { get; set; }

        string Password { get; set; }

        string Project { get; set; }

        string BaseUrl { get; set; }

        string Repository { get; set; }

        string Committer { get; set; }
    }
}
