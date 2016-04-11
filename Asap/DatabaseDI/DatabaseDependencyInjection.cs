using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkDb;
using Microsoft.Practices.Unity;
using Repository;

namespace DatabaseDI
{
    public class DatabaseDependencyInjection
    {
        private static IUnityContainer _container;

        static DatabaseDependencyInjection()
        {
            _container = new UnityContainer();

            ConfigureDIs();
        }

        private static void ConfigureDIs()
        {
            _container.RegisterType<IRepository, EntityFrameworkRepository>();
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
