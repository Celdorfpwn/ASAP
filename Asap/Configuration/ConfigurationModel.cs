using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DatabaseDI;
using Entities;

using Repository;
using Security;
using ToolsConfiguration;

namespace GitJiraConfiguration
{
    public class ConfigurationModel : IConfiguration
    {

        internal IRepository Repository { get; set; }

        [ConcreteType(typeof(AsapCredentials))]
        public ICredentials Credentials { get; private set; }

        [ConcreteType(typeof(GitConfig))]
        public ISourceControlConfig SourceControlConfig { get; private set; }

        [ConcreteType(typeof(JiraConfig))]
        public IIssuesTrackingConfig IssuesTrackingConfig { get; private set; }

        public ConfigurationModel(IRepository repository)
        {
            Repository = repository;

            ConfigFactory.InitializeConfigProperties(this);
        }

        public void Save()
        {
            foreach (var config in this.GetType().GetConfigProperties())
            {
                var configValue = config.GetValue(this);

                foreach (var setting in configValue.GetType().GetSettingProperties())
                {
                    var settingValue = setting.GetValue(configValue) as Setting;
                    Repository.Edit<Setting>(settingValue);
                }
            }

            Repository.Save();
        }




        public void Dispose()
        {
            Repository.Dispose();
        }
    }
}
