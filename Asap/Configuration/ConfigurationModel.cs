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

        private IRepository _repository { get; set; }

        public ICredentials Credentials { get; private set; }

        public ISourceControlConfig SourceControlConfig { get; private set; }

        public IIssuesTrackingConfig IssuesTrackingConfig { get; private set; }

        public ConfigurationModel(IRepository repository)
        {
            _repository = repository;
            Credentials = ConfigFactory.Create<AsapCredentials>(_repository);
            SourceControlConfig = ConfigFactory.Create<GitConfig>(_repository);
            IssuesTrackingConfig = ConfigFactory.Create<JiraConfig>(_repository);
        }



        public void Save()
        {
            foreach (var config in typeof(ConfigurationModel).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var configValue = config.GetValue(this);

                foreach (var setting in configValue.GetType().GetSettingProperties())
                {
                    var settingValue = setting.GetValue(configValue) as Setting;
                    _repository.Edit<Setting>(settingValue);
                }
            }

            _repository.Save();
        }




        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
