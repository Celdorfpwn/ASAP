using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolsConfiguration
{
    public interface ISourceControlConfig
    {
        string Username { get; set; }

        string Password { get; set; }

        string Email { get; set; }

        string Path { get; set; }
    }
}
