using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Repository;
using Security;
using ToolsConfiguration;

namespace GitJiraConfiguration
{
    public static class ConfigFactory
    {

        internal static void InitializeConfigProperties(ConfigurationModel model)
        {
            foreach (var property in model.GetType().GetConfigProperties())
            {
                property.SetValue(model, Create(property.GetCustomAttribute<ConcreteType>().Value, model.Repository));
            }
        }

        private static object Create(Type type, IRepository repository)
        {
            var config = Activator.CreateInstance(type);

            SetConfigProperties(config, repository);

            return config;
        }

        private static void SetConfigProperties(object config, IRepository repository)
        {
            foreach (var setting in config.GetType().GetSettingProperties())
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
