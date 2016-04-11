using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolsConfiguration
{
    public interface IIssuesTrackingConfig
    {
        string Project { get; set; }

        string BaseUrl { get; set; }

        string BrowserUrl { get; set; }

        string Username { get; set; }

        string Password { get; set; }
    }
}
