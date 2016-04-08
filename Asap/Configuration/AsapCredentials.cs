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
    public class AsapCredentials : ICredentials
    {
        public string Username
        {
            get
            {
                return AsapUsername.Value.Decrypt();
            }
            set
            {
                AsapUsername.Value = value.Encrypt();
            }
        }

        public string Password
        {
            get
            {
                return AsapPassword.Value.Decrypt();
            }
            set
            {
                AsapPassword.Value = value.Encrypt();
            }
        }


        internal Setting AsapUsername { get; set; }

        internal Setting AsapPassword { get; set; }
    }
}
