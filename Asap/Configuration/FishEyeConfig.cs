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
    class FishEyeConfig : ICodeReviewConfig
    {
        public string BaseUrl
        {
            get
            {
                return FisheyeBaseUrl.Value.Decrypt();
            }

            set
            {
                FisheyeBaseUrl.Value = value.Encrypt();
            }
        }

        public string Password
        {
            get
            {
                return FisheyePassword.Value.Decrypt();
            }

            set
            {
                FisheyePassword.Value = value.Encrypt();
            }
        }

        public string Project
        {
            get
            {
                return FisheyeProject.Value.Decrypt();
            }

            set
            {
                FisheyeProject.Value = value.Encrypt();
            }
        }

        public string Username
        {
            get
            {
                return FisheyeUsername.Value.Decrypt();
            }

            set
            {
                FisheyeUsername.Value = value.Encrypt();
            }
        }

        public string Repository
        {
            get
            {
                return FisheyeRepository.Value.Decrypt();
            }
            set
            {
                FisheyeRepository.Value = value.Encrypt();
            }
        }

        public string Committer
        {
            get
            {
                return FisheyeCommitter.Value.Decrypt();
            }
            set
            {
                FisheyeCommitter.Value = value.Encrypt();
            }
        }

        internal Setting FisheyeUsername { get; set; }

        internal Setting FisheyePassword { get; set; }

        internal Setting FisheyeProject { get; set; }

        internal Setting FisheyeBaseUrl { get; set; }

        internal Setting FisheyeRepository { get; set; }

        internal Setting FisheyeCommitter { get; set; }

    }
}
