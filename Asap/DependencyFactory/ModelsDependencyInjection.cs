using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BL;
using ExternalDI;
using IssuesTracking;
using Microsoft.Practices.Unity;
using SourceControl;


namespace ModelsDI
{
    public static class ModelsDependencyInjection
    {

        private static IUnityContainer _container;

        static ModelsDependencyInjection()
        {
            _container = new UnityContainer();

            ConfigureDIs();
        }

        private static void ConfigureDIs()
        {

            _container.RegisterType<TasksModel>(new InjectionConstructor(ExternalDependencyInjection.Resolve<ISourceControl>(), ExternalDependencyInjection.Resolve<IIssuesTracking>()));
            _container.RegisterType<IssuesTrackingModel>(new InjectionConstructor(ExternalDependencyInjection.Resolve<ISourceControl>(), ExternalDependencyInjection.Resolve<IIssuesTracking>()));
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
