using System;
using System.Collections.Generic;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository;
using System.Linq;
using GitJiraConfiguration;
using Security;
using System.Diagnostics;
using System.Reflection;

namespace GitJiraConfigurationUnitTests
{
    [TestClass]
    public class ConfigurationModelUT
    {
        [TestMethod]
        public void Initialize_Configuration_Model_When_Settings_Is_Empty()
        {
            var settingsList = new List<Setting>();
            var repository = MockRepositoryForIntialize(settingsList).Object;

            var configurationModel = new ConfigurationModel(repository);

            Assert.AreEqual(GetTreeSettingProperties(configurationModel).Count, settingsList.Count);
        }



        [TestMethod]
        public void Configuration_Model_Credentials_Not_Null()
        {
            var configurationModel = GetInitializedConfigurationModel();

            Assert.IsNotNull(configurationModel.Credentials.Username);
            Assert.IsNotNull(configurationModel.Credentials.Password);
        }


        [TestMethod]
        public void Configuration_Model_JiraConfig_Not_Null()
        {
            var configurationModel = GetInitializedConfigurationModel();

            AssertPublicPropertiesNotNull(configurationModel.IssuesTrackingConfig);
        }

        [TestMethod]
        public void Configuration_Model_GitConfig_Not_Null()
        {
            var configurationModel = GetInitializedConfigurationModel();

            AssertPublicPropertiesNotNull(configurationModel.SourceControlConfig);
        }

        [TestMethod]
        public void Configuraiton_Model_Set_Credentials_Properties()
        {
            var credentials = GetInitializedConfigurationModel().Credentials;
            SetAndAssertGetPublicStringProperties(credentials, "value", typeof(string));
        }

        [TestMethod]
        public void Configuraiton_Model_Set_GitConfig_Properties()
        {
            var gitconfig = GetInitializedConfigurationModel().SourceControlConfig;
            SetAndAssertGetPublicStringProperties(gitconfig, "value", typeof(string));
        }

        [TestMethod]
        public void Configuration_Model_Set_JiraConfig_Properties()
        {
            var jiraconfig = GetInitializedConfigurationModel().IssuesTrackingConfig;
            SetAndAssertGetPublicStringProperties(jiraconfig, "value", typeof(string));
        }

        [TestMethod]
        public void Configuration_Model_Can_Save()
        {
            var editList = new List<Setting>();
            var configurationModel = new ConfigurationModel(MockRepositoryForSave(editList).Object);

            configurationModel.Save();

            var settings = GetTreeSettingProperties(configurationModel);

            foreach (var editedSetting in editList)
            {
                Assert.IsTrue(settings.Contains(editedSetting));
            }
        }

        [TestMethod]
        public void Configuration_Model_Can_Dispose()
        {
            bool disposed = false;
            var mockRepository = MockRepositoryForIntialize(GetFilledSettingsList());

            mockRepository.Setup(repository => repository.Dispose())
                .Callback(() => disposed = true);

            using (var configurationModel = new ConfigurationModel(mockRepository.Object)) { }

            Assert.IsTrue(disposed);
        }

        private ConfigurationModel GetInitializedConfigurationModel()
        {
            var settingsList = GetFilledSettingsList();
            var repository = MockRepositoryForIntialize(settingsList).Object;

            return new ConfigurationModel(repository);
        }

        private void SetAndAssertGetPublicStringProperties(object instance, object value, Type type)
        {
            foreach (var propertyInfo in instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(property => property.PropertyType.Equals(type)))
            {
                propertyInfo.SetValue(instance, value);
                Assert.AreEqual(value, propertyInfo.GetValue(instance));
            }
        }

        private void AssertPublicPropertiesNotNull(object instance)
        {
            foreach (var propertyInfo in instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                Assert.IsNotNull(propertyInfo.GetValue(instance));
            }
        }

        private List<Setting> GetFilledSettingsList()
        {
            return new List<Setting>
            {
                new Setting { Name = ("AsapUsername").Encrypt() , Value = ("val").Encrypt() },
                new Setting { Name = ("AsapPassword").Encrypt(), Value = ("val").Encrypt()  },
                new Setting { Name = ("GitUsername").Encrypt(), Value = ("val").Encrypt()  },
                new Setting { Name = ("GitPassword").Encrypt(), Value = ("val").Encrypt()  },
                new Setting { Name = ("GitPath").Encrypt() , Value = ("val").Encrypt() },
                new Setting { Name = ("GitEmail").Encrypt() , Value = ("val").Encrypt() },
                new Setting { Name = ("JiraBaseUrl").Encrypt(), Value = ("val").Encrypt()  },
                new Setting { Name = ("JiraBrowserUrl").Encrypt() , Value = ("val").Encrypt() },
                new Setting { Name = ("JiraUsername").Encrypt() , Value = ("val").Encrypt() },
                new Setting { Name = ("JiraPassword").Encrypt() , Value = ("val").Encrypt() },
                new Setting { Name = ("JiraProject").Encrypt() , Value = ("val").Encrypt() },
            };
        }

        private List<Setting> GetTreeSettingProperties(object instance)
        {
            var settings = new List<Setting>();
            foreach (var config in instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var configValue = config.GetValue(instance);

                foreach (var setting in configValue.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(property => property.PropertyType.Equals(typeof(Setting))))
                {
                    var settingValue = setting.GetValue(configValue) as Setting;
                    settings.Add(settingValue);
                }
            }

            return settings;
        }

        private Mock<IRepository> MockRepositoryForIntialize(List<Setting> settingsList)
        {
            var mockRepository = new Mock<IRepository>();

            mockRepository.Setup(mock => mock.All<Setting>())
                .Returns(settingsList.AsQueryable());

            mockRepository.Setup(mock => mock.Get<Setting>(It.IsAny<Object[]>()))
                .Returns<Object[]>((keys) => settingsList.FirstOrDefault(setting => setting.Name.Equals(keys[0].ToString())));

            mockRepository.Setup(mock => mock.Add<Setting>(It.IsAny<Setting>()))
                .Callback<Setting>((setting) => settingsList.Add(setting));

            return mockRepository;
        }

        private Mock<IRepository> MockRepositoryForSave(List<Setting> savedSettings)
        {
            var mockRepository = new Mock<IRepository>();
            mockRepository.Setup(mock => mock.All<Setting>())
               .Returns(GetFilledSettingsList().AsQueryable());

            mockRepository.Setup(mock => mock.Edit<Setting>(It.IsAny<Setting>()))
                .Callback<Setting>((setting) => savedSettings.Add(setting));

            return mockRepository;
        }


    }
}
