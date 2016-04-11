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
    class JiraConfig : IIssuesTrackingConfig
    {
        public string BaseUrl
        {
            get
            {
                return JiraBaseUrl.Value.Decrypt();
            }
            set
            {
                JiraBaseUrl.Value = value.Encrypt();
            }
        }

        public string BrowserUrl
        {
            get
            {
                return JiraBrowserUrl.Value.Decrypt();
            }
            set
            {
                JiraBrowserUrl.Value = value.Encrypt();
            }
        }

        public string Username
        {
            get
            {
                return JiraUsername.Value.Decrypt();
            }
            set
            {
                JiraUsername.Value = value.Encrypt();
            }
        }

        public string Password
        {
            get
            {
                return JiraPassword.Value.Decrypt();
            }
            set
            {
                JiraPassword.Value = value.Encrypt();
            }
        }

        public string Project
        {
            get
            {
                return JiraProject.Value.Decrypt();
            }
            set
            {
                JiraProject.Value = value.Encrypt();
            }
        }


        internal Setting JiraBaseUrl { get;set; }

        internal Setting JiraBrowserUrl { get; set; }

        internal Setting JiraUsername { get; set; }

        internal Setting JiraPassword { get; set; }

        internal Setting JiraProject { get; set; }
    }
}
