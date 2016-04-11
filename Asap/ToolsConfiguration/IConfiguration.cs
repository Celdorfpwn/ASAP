using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolsConfiguration
{
    public interface IConfiguration : IDisposable
    {
        ICredentials Credentials { get; }

        ISourceControlConfig SourceControlConfig { get; }

        IIssuesTrackingConfig IssuesTrackingConfig { get; }

        void Save();
    }
}
