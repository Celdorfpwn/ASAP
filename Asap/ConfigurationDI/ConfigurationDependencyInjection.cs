using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseDI;
using GitJiraConfiguration;
using Microsoft.Practices.Unity;
using Repository;
using ToolsConfiguration;

namespace ConfigurationDI
{
    public class ConfigurationDependencyInjection
    {
        private static IUnityContainer _container;

        static ConfigurationDependencyInjection()
        {
            _container = new UnityContainer();

            ConfigureDIs();
        }

        private static void ConfigureDIs()
        {
            _container.RegisterType<IConfiguration, ConfigurationModel>(new InjectionConstructor(DatabaseDependencyInjection.Resolve<IRepository>()));
        }

        public static Dependency Resolve<Dependency>()
        {
            Dependency instance = default(Dependency);

            if (_container.IsRegistered(typeof(Dependency)))
            {
                instance = _container.Resolve<Dependency>();
            }

            return instance;

        }
    }
}
