using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Repository;
using Security;

namespace GitJiraConfiguration
{
    public static class ConfigFactory
    {
        public static Config Create<Config>(IRepository repository) where Config : class
        {
            var config = Activator.CreateInstance<Config>();

            SetConfigProperties(config, repository);

            return config;
        }

        private static void SetConfigProperties<Config>(Config config, IRepository repository) where Config : class
        {
            foreach (var setting in typeof(Config).GetSettingProperties())
            {
                setting.SetValue(config, GetSetting(setting.Name, repository));
            }
        }

        private static Setting GetSetting(string settingName, IRepository repository)
        {

            var setting = repository.Get<Setting>(settingName.Encrypt());

            if (setting == null)
            {
                setting = InitializeSetting(settingName, repository);
            }

            return setting;

        }

        private static Setting InitializeSetting(string settingName, IRepository repository)
        {
            var setting = new Setting { Name = settingName.Encrypt() };
            repository.Add<Setting>(setting);
            repository.Save();
            return setting;
        }
    }
}
