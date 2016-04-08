using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Security;
using ToolsConfiguration;

namespace GitJiraConfiguration
{
    public class GitConfig : ISourceControlConfig
    {
        public string Username
        {
            get
            {
                return GitUsername.Value.Decrypt();
            }
            set
            {
                GitUsername.Value = value.Encrypt();
            }
        }

        public string Password
        {
            get
            {
                return GitPassword.Value.Decrypt();
            }
            set
            {
                GitPassword.Value = value.Encrypt();
            }
        }

        public string Path
        {
            get
            {
                return GitPath.Value.Decrypt();
            }
            set
            {
                GitPath.Value = value.Encrypt();
            }
        }

        public string Email
        {
            get
            {
                return GitEmail.Value.Decrypt();
            }
            set
            {
                GitEmail.Value = value.Encrypt();
            }
        }

        internal Setting GitUsername { get; set; }

        internal Setting GitPassword { get; set; }

        internal Setting GitPath { get; set; }

        internal Setting GitEmail { get; set; }
    }
}
